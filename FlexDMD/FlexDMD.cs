/* Copyright 2019 Vincent Bousquet

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
   */
using Glide;
using FlexDMD.Actors;
using FlexDMD.Scenes;
using MediaFoundation;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using static FlexDMD.DMDDevice;
using MediaFoundation.Misc;

namespace FlexDMD
{
    [Guid("766e10d3-dfe3-4e1b-ac99-c4d2be16e91f"), ComVisible(true), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IDMDObjectEvents))]
    public class DMDObject : IDMDObject
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly List<Action> _runnables = new List<Action>();
        private readonly DMDDevice _dmd = new DMDDevice();
        private readonly AssetManager _assets = new AssetManager();
        private readonly SceneQueue _queue = new SceneQueue();
        private readonly Tweener _tweener = new Tweener();
        private readonly Group _stage = new Group();
        private readonly int _frameRate = 60;
        private ScoreBoard _scoreBoard;
        private FontDef _scoreFontText, _scoreFontNormal, _scoreFontHighlight, _twoLinesFontTop, _twoLinesFontBottom;
        private FontDef[] _singleLineFont;
        private Thread _processThread = null;
        private bool _visible = false;
        private bool _running = false;
        private ushort _width = 128;
        private ushort _height = 32;
        private string _gameName = "";
        private int _stretchMode = 0;
        private int _nextId = 1;
        private Bitmap _frame = null;
        private Graphics _graphics = null;
        private object[] _pixels = null;
        private object[] _coloredPixels = null;
        private event OnDMDChangedDelegate OnDMDChanged;
        public delegate void OnDMDChangedDelegate();

        public ushort DmdWidth
        {
            get => _width;
            set
            {
                if (_width == value) return;
                if (_processThread != null)
                {
                    log.Error("Width changed after initialization.");
                }
                else
                {
                    log.Info("DMD width set to {0}", value);
                    _width = value;
                }
            }
        }

        public ushort DmdHeight
        {
            get => _height;
            set
            {
                if (_height == value) return;
                if (_processThread != null)
                {
                    log.Error("Height changed after initialization.");
                }
                else
                {
                    log.Info("DMD height set to {0}", value);
                    _height = value;
                }
            }
        }

        object IDMDObject.DmdColoredPixels
        {
            get
            {
                if (_coloredPixels == null) _coloredPixels = new object[_width * _height];
                return _coloredPixels;
            }
        }

        object IDMDObject.DmdPixels
        {
            get
            {
                if (_pixels == null) _pixels = new object[_width * _height];
                return _pixels;
            }
        }

        ~DMDObject()
        {
            if (_processThread != null)
            {
                log.Error("Destructor called before Uninit");
                Uninit();
            }
        }

        public void Init()
        {
            InitForGame("");
        }

        public void InitForGame(string gameName)
        {
            if (_running) return;
            _running = true;
            log.Info("Init {0}", gameName);
            _gameName = gameName;
            HResult hr = MFExtern.MFStartup(0x10070, MFStartup.Lite);
            if (MFError.Failed(hr)) log.Error("Failed to initialize Microsoft Media Foundation: {0}", hr);
            _frame = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
            _graphics = Graphics.FromImage(_frame);
            _scoreFontText = new FontDef(PathType.Resource, "FlexDMD.Resources.font-5.fnt", 0.66f);
            _scoreFontNormal = new FontDef(PathType.Resource, "FlexDMD.Resources.font-7.fnt", 0.66f);
            _scoreFontHighlight = new FontDef(PathType.Resource, "FlexDMD.Resources.font-12.fnt");
            // UltraDMD uses f14by26 or f12by24 or f7by13 to fit in
            _singleLineFont = new FontDef[] { new FontDef(PathType.Resource, "FlexDMD.Resources.font-12.fnt") };
            // UltraDMD uses f5by7 / f6by12 for top / bottom line
            _twoLinesFontTop = new FontDef(PathType.Resource, "FlexDMD.Resources.font-7.fnt");
            _twoLinesFontBottom = new FontDef(PathType.Resource, "FlexDMD.Resources.font-12.fnt");
            _scoreBoard = new ScoreBoard(
                _assets.Load<Actors.Font>(_scoreFontNormal).Load(),
                _assets.Load<Actors.Font>(_scoreFontHighlight).Load(),
                _assets.Load<Actors.Font>(_scoreFontText).Load()
                );
            _scoreBoard.SetSize(_width, _height);
            _stage.AddActor(_scoreBoard);
            _stage.AddActor(_queue);
            SetVisibleVirtualDMD(true);
            Clear();
            _processThread = new Thread(new ThreadStart(RenderLoop))
            {
                IsBackground = true
            };
            _processThread.Start();
        }

        public void Uninit()
        {
            if (!_running) return;
            _running = false;
            log.Info("Uninit");
            CancelRendering();
            if (_processThread != null)
            {
                _processThread.Join();
                _processThread = null;
            }
            SetVisibleVirtualDMD(false);
            _graphics.Dispose();
            _graphics = null;
            _frame.Dispose();
            _frame = null;
            HResult hr = MFExtern.MFShutdown();
            if (hr < 0) log.Error("Failed to dispose Microsoft Media Foundation: {0}", hr);
        }

        public void RenderLoop()
        {
            Stopwatch stopWatch = new Stopwatch();
            double elapsedMs = 0.0;
            double memDump = 1.0;
            WindowHandle visualPinball = null;
            while (_running)
            {
                stopWatch.Restart();
                if (visualPinball == null)
                {
                    visualPinball = WindowHandle.FindWindow(wh => wh.IsVisible() && wh.GetWindowText().StartsWith("Visual Pinball Player"));
                }
                else if (!visualPinball.IsWindow())
                {
                    log.Info("Closing FlexDMD since Visual Pinball Player window was closed");
                    _processThread = null;
                    Uninit();
                    break;
                }
                float elapsedS = (float)(elapsedMs / 1000.0);
                memDump -= elapsedS;
                if (memDump < 0)
                {
                    memDump = 1.0;
                    log.Debug("Memory used: {0}Mo", GC.GetTotalMemory(false) / (1024 * 1024));
                }
                _stage.SetSize(_width, _height);
                lock (_runnables)
                {
                    _runnables.ForEach(item => item());
                    _runnables.Clear();
                }
                _tweener.Update(elapsedS);
                _stage.Update(elapsedS);
                _scoreBoard.Visible &= !_queue.IsRendering();
                if (_visible)
                {
                    _stage.Draw(_graphics);
                    Rectangle rect = new Rectangle(0, 0, _frame.Width, _frame.Height);
                    BitmapData data = _frame.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, _frame.PixelFormat);
                    _dmd.RenderRgb24(_width, _height, data.Scan0);
                    if (_pixels != null)
                    {
                        unsafe
                        {
                            byte* ptr = ((byte*)data.Scan0.ToPointer());
                            int pos = 0;
                            for (int y = 0; y < _height; y++)
                            {
                                for (int x = 0; x < _width; x++)
                                {
                                    byte r = *ptr;
                                    ptr++;
                                    byte g = *ptr;
                                    ptr++;
                                    byte b = *ptr;
                                    ptr++;
                                    // _pixels[y * _width + x] = (byte)((r + g + b) / 3);
                                    float v = 0.2126f * r + 0.7152f * g + 0.0722f * b;
                                    if (v > 0.99f) v = 0.99f;
                                    _pixels[pos] = (byte)(v);
                                    pos++;
                                }
                            }
                        }
                    }
                    if (_coloredPixels != null)
                    {
                        unsafe
                        {
                            byte* ptr = ((byte*)data.Scan0.ToPointer());
                            int pos = 0;
                            for (int y = 0; y < _height; y++)
                            {
                                for (int x = 0; x < _width; x++)
                                {
                                    byte r = *ptr;
                                    ptr++;
                                    byte g = *ptr;
                                    ptr++;
                                    byte b = *ptr;
                                    ptr++;
                                    _coloredPixels[pos] = (uint)((b << 16) + (g << 8) + r);
                                    pos++;
                                }
                            }
                        }
                    }
                    _frame.UnlockBits(data);
                    OnDMDChanged?.Invoke();
                }
                double renderingDuration = stopWatch.Elapsed.TotalMilliseconds;

                int sleepMs = (1000 / _frameRate) - (int)renderingDuration;
                if (sleepMs > 0) Thread.Sleep(sleepMs);
                elapsedMs = stopWatch.Elapsed.TotalMilliseconds;
                // log.Info("Elapsed: {0}ms", elapsedMs);
            }
        }

        public int GetMajorVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileMajorPart;
        }

        public int GetMinorVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileMinorPart;
        }

        public int GetBuildNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileBuildPart * 10000 + fvi.FilePrivatePart;
        }

        public bool SetVisibleVirtualDMD(bool bVisible)
        {
            // log.Info("SetVisibleVirtualDMD({0})", bVisible);
            bool wasVisible = _visible;
            _visible = bVisible;
            if (!wasVisible && _visible)
            {
                _dmd.Open();
                var options = new PMoptions();
                _dmd.GameSettings(_gameName, 0, options);
            }
            else if (wasVisible && !_visible)
            {
                _dmd.Close();
            }
            return wasVisible;
        }

        public bool SetFlipY(bool flipY)
        {
            log.Error("SetFlipY is not yet supported in FlexDMD");
            return false;
        }

        public bool IsRendering()
        {
            return _queue.IsRendering();
        }

        public void CancelRendering()
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // log.Info("CancelRendering");
                    _queue.CancelRendering();
                });
            }
        }

        public void CancelRenderingWithId(string sceneId)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    log.Info("CancelRenderingWithId {0}", sceneId);
                    _queue.CancelRendering(sceneId);
                });
            }
        }

        public void Clear()
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // log.Info("Clear");
                    _graphics.Clear(Color.Black);
                    _scoreBoard.Visible = false;
                });
            }
        }

        public void SetProjectFolder(string basePath)
        {
            log.Info("SetProjectFolder {0}", basePath);
            _assets.BasePath = basePath;
        }

        public void SetVideoStretchMode(int mode)
        {
            log.Info("SetVideoStretchMode {0}", mode);
            _stretchMode = mode;
        }

        public int CreateAnimationFromImages(int fps, bool loop, string imagelist)
        {
            var id = _nextId;
            _nextId++;
            // FIXME TODO, preload using the asset manager and create an Id reconized by the asset manager for the data, not the actor
            return id;
        }

        public int RegisterVideo(int videoStretchMode, bool loop, string videoFilename)
        {
            var id = _nextId;
            _nextId++;
            // FIXME TODO, preload using the asset manager and create an Id reconized by the asset manager for the data, not the actor
            return id;
        }

        private Actor ResolveImage(string filename)
        {
            // filename can be a preloaded id, or a comma separated image list, or a filename to an image, gif or video file.
            // FIXME if (_preloads.ContainsKey(filename)) return _preloads[filename];
            if (filename.Trim().Length == 0) return null;
            var fullPath = System.IO.Path.Combine(_assets.BasePath, filename);
            if (File.Exists(fullPath))
            {
                string extension = Path.GetExtension(filename).ToLowerInvariant();
                if (extension.Equals(".png") || extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".bmp"))
                {
                    return new Image(_assets.Load<Bitmap>(filename).Load());
                }
                else if (extension.Equals(".gif"))
                {
                    return new GIFImage(_assets.Load<Bitmap>(filename).Load());
                }
                else if (extension.Equals(".wmv") || extension.Equals(".avi") || extension.Equals(".mp4"))
                {
                    return new Video(fullPath, false, _stretchMode);
                }
            }
            else if (filename.Contains(","))
            {
                return new AnimatedImage(_assets.BasePath, filename, 25, false);
            }
            log.Error("Missing resource '{0}'", filename);
            return new Actor();
        }

        public void DisplayVersionInfo()
        {
            // No version info (this is an implementation choice to avoid delaying game startup and displaying again and again the same scene)
        }

        public void DisplayScene00(string background, string toptext, int topBrightness, string bottomtext, int bottomBrightness, int animateIn, int pauseTime, int animateOut)
        {
            DisplayScene00ExWithId("", false, background, toptext, topBrightness, -15, bottomtext, bottomBrightness, -15, animateIn, pauseTime, animateOut);
        }

        public void DisplayScene00Ex(string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut)
        {
            DisplayScene00ExWithId("", false, background, toptext, topBrightness, topOutlineBrightness, bottomtext, bottomBrightness, bottomOutlineBrightness, animateIn, pauseTime, animateOut);
        }

        // Used by Metal Slug when entering high score, or quite frequently by amh
        public void DisplayScene00ExWithId(string sceneId, bool cancelPrevious, string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    //log.Info("DisplayScene00ExWithId sceneId='{0}', cancelPrevious='{1}', background={2}, toptext='{3}', topBrightness={4}, topOutlineBrightness={5}, bottomtext='{6}', bottomBrightness={7}, bottomOutlineBrightness={8}, animateIn={9}, pauseTime={10}, animateOut={11}",
                    //    sceneId, cancelPrevious, background, toptext, topBrightness, topOutlineBrightness, bottomtext, bottomBrightness, bottomOutlineBrightness, animateIn, pauseTime, animateOut);
                    _scoreBoard.Visible = false;
                    if (cancelPrevious && sceneId != null && sceneId.Length > 0)
                    {
                        var s = _queue.GetActiveScene();
                        if (s.Id == sceneId) _queue.CancelRendering(sceneId);
                    }
                    if (toptext != null && toptext.Length > 0 && bottomtext != null && bottomtext.Length > 0)
                    {
                        var font7 = _assets.Load<Actors.Font>(new FontDef(PathType.Resource, "FlexDMD.Resources.font-7.fnt", topBrightness / 15f, topOutlineBrightness / 15f)).Load();
                        var font12 = _assets.Load<Actors.Font>(new FontDef(PathType.Resource, "FlexDMD.Resources.font-12.fnt", bottomBrightness / 15f, bottomOutlineBrightness / 15f)).Load();
                        var scene = new TwoLineScene(ResolveImage(background), toptext, font7, bottomtext, font12, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, sceneId);
                        _queue.Enqueue(scene);
                    }
                    else if (toptext != null && toptext.Length > 0)
                    {
                        var font12 = _assets.Load<Actors.Font>(new FontDef(PathType.Resource, "FlexDMD.Resources.font-12.fnt", topBrightness / 15f, topOutlineBrightness / 15f)).Load();
                        var scene = new SingleLineScene(ResolveImage(background), toptext, font12, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, sceneId);
                        _queue.Enqueue(scene);
                    }
                    else if (bottomtext != null && bottomtext.Length > 0)
                    {
                        var font12 = _assets.Load<Actors.Font>(new FontDef(PathType.Resource, "FlexDMD.Resources.font-12.fnt", bottomBrightness / 15f, bottomOutlineBrightness / 15f)).Load();
                        var scene = new SingleLineScene(ResolveImage(background), bottomtext, font12, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, sceneId);
                        _queue.Enqueue(scene);
                    }
                    else
                    {
                        var scene = new BackgroundScene(ResolveImage(background), (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, sceneId);
                        _queue.Enqueue(scene);
                    }
                });
            }
        }

        public void ModifyScene00(string id, string toptext, string bottomtext)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // log.Info("ModifyScene00 '{0}', '{1}', '{2}'", id, toptext, bottomtext);
                    var scene = _queue.GetActiveScene();
                    if (scene != null && id != null && id.Length > 0 && scene.Id == id)
                    {
                        if (scene is TwoLineScene s2) s2.SetText(toptext, bottomtext);
                        if (scene is SingleLineScene s1) s1.SetText(toptext);
                    }
                });
            }
        }

        public void ModifyScene00Ex(string id, string toptext, string bottomtext, int pauseTime)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // log.Info("ModifyScene00Ex '{0}', '{1}', '{2}', {3}", id, toptext, bottomtext, pauseTime);
                    var scene = _queue.GetActiveScene();
                    if (scene != null && id != null && id.Length > 0 && scene.Id == id)
                    {
                        if (scene is TwoLineScene s2) s2.SetText(toptext, bottomtext);
                        if (scene is SingleLineScene s1) s1.SetText(toptext);
                        scene.SetPause(scene.Time + pauseTime / 1000f);
                    }
                });
            }
        }

        public void DisplayScene01(string sceneId, string background, string text, int textBrightness, int textOutlineBrightness, int animateIn, int pauseTime, int animateOut)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // log.Error("DisplayScene01 '{0}', '{1}', '{2}', {3}, {4}, {5}, {6}, {7}", sceneId, background, text, textBrightness, textOutlineBrightness, animateIn, pauseTime, animateOut);
                    _scoreBoard.Visible = false;
                    var font12 = _assets.Load<Actors.Font>(new FontDef(PathType.Resource, "FlexDMD.Resources.font-12.fnt", textBrightness / 15f, textOutlineBrightness / 15f)).Load();
                    var scene = new SingleLineScene(ResolveImage(background), text, font12, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, sceneId);
                    scene.ScrollX = _width;
                    // Not sure about the timing; UltraDMD moves text by 1.2 pixel per frame (no delta time) and seems to render based on the frame rate at 60FPS. Hence 3 * 128 / (60 * 1.2) = 5.333
                    _tweener.Tween(scene, new { ScrollX = -_width }, 5.333f, 0f); 
                    _queue.Enqueue(scene);
                });
            }
        }

        public void SetScoreboardBackgroundImage(string filename, int selectedBrightness, int unselectedBrightness)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    _scoreBoard.SetBackground(ResolveImage(filename));
                    _scoreBoard.SetFonts(
                        _assets.Load<Actors.Font>(new FontDef(_scoreFontNormal.PathType, _scoreFontNormal.Path, unselectedBrightness)).Load(),
                        _assets.Load<Actors.Font>(new FontDef(_scoreFontHighlight.PathType, _scoreFontHighlight.Path, selectedBrightness)).Load(),
                        _assets.Load<Actors.Font>(new FontDef(_scoreFontText.PathType, _scoreFontText.Path, unselectedBrightness)).Load());
                });
            }
        }

        public void DisplayScoreboard(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // Direct rendering: render only if the scene queue is empty, and no direct rendering has happened (managed by scoreboard visibility in render loop)
                    // log.Info("Scoreboard for {0} players, {1} is playing", cPlayers, highlightedPlayer);
                    _scoreBoard.Visible = true;
                    _scoreBoard.SetNPlayers(cPlayers);
                    _scoreBoard.SetHighlightedPlayer(highlightedPlayer);
                    _scoreBoard.SetScore(score1, score2, score3, score4);
                    _scoreBoard._lowerLeft.Text = lowerLeft;
                    _scoreBoard._lowerRight.Text = lowerRight;
                });
            }
        }

        // From KissDMDv2.vbs, an undocumented function as far as I know. I do not have a clue on the difference between this one and DisplayScoreboard.
        // UltraDMD.DisplayScoreboard00 PlayersPlayingGame, 0, Score(1), Score(2), Score(3), Score(4), "credits " & Credits, ""
        public void DisplayScoreboard00(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight)
        {
            DisplayScoreboard(cPlayers, highlightedPlayer, score1, score2, score3, score4, lowerLeft, lowerRight);
        }

        public void DisplayText(string text, int textBrightness, int textOutlineBrightness)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    log.Error("DisplayText [untested, missing correct font size] '{0}', {1}, {2}", text, textBrightness, textOutlineBrightness);
                    _scoreBoard.Visible = false;
                    foreach (FontDef f in _singleLineFont)
                    {
                        var font = _assets.Load<Actors.Font>(new FontDef(f.PathType, f.Path, textBrightness, textOutlineBrightness)).Load();
                        var label = new Label(font, text);
                        label.SetPosition((_width - label.Width) / 2, (_height - label.Height) / 2);
                        if (label.X >= 0 && label.Y >= 0)
                        {
                            label.Draw(_graphics);
                            break;
                        }
                    }
                });
            }
        }

        public void ScrollingCredits(string background, string text, int textBrightness, int animateIn, int pauseTime, int animateOut)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    log.Error("ScrollingCredits [unsupported] '{0}', '{1}', {2}", background, text, textBrightness);
                    _scoreBoard.Visible = false;
                    string[] lines = text.Split(new Char[] { '\n', '|' });
                    var font12 = _assets.Load<Actors.Font>(new FontDef(PathType.Resource, "FlexDMD.Resources.font-12.fnt", textBrightness / 15f, -1)).Load();
                    var scene = new ScrollingCreditsScene(ResolveImage(background), lines, font12, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut);
                    scene.ScrollY = _height;
                    // There is nothing obvious in UltraDMD that gives hint on the timing, so I choosed a default speed
                    _tweener.Tween(scene, new { ScrollY = -scene.ContentHeight }, 3f + lines.Length * 0.4f, 0f);
                    _queue.Enqueue(scene);
                });
            }
        }
    }
}
