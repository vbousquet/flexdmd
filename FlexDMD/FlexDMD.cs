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

namespace FlexDMD
{
    [Guid("766e10d3-dfe3-4e1b-ac99-c4d2be16e91f"), ComVisible(true), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IDMDObjectEvents))]
    public class DMDObject : IDMDObject
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly Group _stage = new Group();
        private readonly Dictionary<String, Actor> _preloads = new Dictionary<String, Actor>();
        private readonly Dictionary<String, Scene> _scenes = new Dictionary<String, Scene>();
        private readonly SceneQueue _queue = new SceneQueue();
        private readonly List<Action> _runnables = new List<Action>();
        private readonly DMDDevice _dmd = new DMDDevice();
        private readonly AssetManager _assets = new AssetManager();
        private readonly Tweener _tweener = new Tweener();
        private readonly int _frameRate = 60;
        private ushort _width = 128;
        private ushort _height = 32;
        private string _basePath = "./";
        private bool _visible = false;
        private bool _running = false;
        private Thread _processThread;
        private int _stretchMode = 0;
        private Bitmap _frame = null;
        private Graphics _graphics = null;
        private int _nextId = 1;
        private ScoreBoard _scoreBoard;
        public delegate void OnDMDChangedDelegate();
        private event OnDMDChangedDelegate OnDMDChanged;
        private object[] _pixels, _coloredPixels;
        private string _gameName;

        object IDMDObject.RawDmdColoredPixels
        {
            get
            {
                if (_coloredPixels == null) _coloredPixels = new object[_width * _height];
                return _coloredPixels;
            }
        }

        object IDMDObject.RawDmdPixels
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
            /* if (_processThread != null)
            {
                log.Error("Init called on an already initialized DMD. Please call it once, or call Uninit first.");
                Uninit();
            }*/
            HResult hr = MFExtern.MFStartup(0x10070, MFStartup.Lite);
            if (hr < 0) log.Error("Failed to initialize Microsoft Media Foundation: {0}", hr);
            _frame = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
            _graphics = Graphics.FromImage(_frame);
            _scoreBoard = new ScoreBoard(
                _assets.Load<Actors.Font>("FlexDMD.Resources.font-7.fnt:10:-1").Load(),
                _assets.Load<Actors.Font>("FlexDMD.Resources.font-12.fnt:15:-1").Load(),
                _assets.Load<Actors.Font>("FlexDMD.Resources.font-5.fnt:10:-1").Load()
                );
            _scoreBoard.SetSize(_width, _height);
            _stage.AddActor(_scoreBoard);
            _stage.AddActor(_queue);
            SetVisibleVirtualDMD(true);
            Clear();
            _processThread = new Thread(new ThreadStart(RenderLoop));
            _processThread.IsBackground = true;
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
                _stage.SetSize(_width, _height);
                lock (_runnables)
                {
                    _runnables.ForEach(item => item());
                    _runnables.Clear();
                }
                _tweener.Update(elapsedS);
                _stage.Update(elapsedS);
                _scoreBoard._visible &= !_queue.IsRendering();
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

        public int RawDmdWidth()
        {
            return _width;
        }

        public int RawDmdHeight()
        {
            return _height;
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
            var scene = _scenes[sceneId];
            if (scene != null)
            {
                lock (_runnables)
                {
                    _runnables.Add(() =>
                    {
                        log.Info("CancelRenderingWithId {0}", sceneId);
                        _queue.CancelRendering(scene);
                    });
                }
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
                    _scoreBoard._visible = false;
                });
            }
        }

        public void SetProjectFolder(string basePath)
        {
            log.Info("SetProjectFolder {0}", basePath);
            _basePath = basePath;
            _assets.SetBasePath(basePath);
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
            var video = new AnimatedImage(_basePath, imagelist, fps, loop);
            _preloads.Add(id.ToString(), video);
            return id;
        }

        public int RegisterVideo(int videoStretchMode, bool loop, string videoFilename)
        {
            var id = _nextId;
            _nextId++;
            var video = new Video(videoFilename, loop, videoStretchMode);
            _preloads.Add(id.ToString(), video);
            return id;
        }

        private Actor ResolveImage(string filename)
        {
            // filename can be a preloaded id, or a comma separated image list, or a filename to an image, gif or video file.
            if (_preloads.ContainsKey(filename)) return _preloads[filename];
            if (filename.Trim().Length == 0) return null;
            var fullPath = System.IO.Path.Combine(_basePath, filename);
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
                return new AnimatedImage(_basePath, filename, 25, false);
            }
            log.Error("Missing ressource '{0}'", filename);
            return new Actor();
        }

        public void DisplayVersionInfo()
        {
            // No version info (this is an implementation choice to avoid delaying game startup and displaying again and again the same scene)
        }

        public void DisplayScene00(string background, string toptext, int topBrightness, string bottomtext, int bottomBrightness, int animateIn, int pauseTime, int animateOut)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // log.Info("DisplayScene00 '{0}', '{1}', {2}, '{3}', {4}, {5}, {6}, {7}", background, toptext, topBrightness, bottomtext, bottomBrightness, animateIn, pauseTime, animateOut);
                    _scoreBoard._visible = false;
                    var font7 = _assets.Load<Actors.Font>(string.Format("FlexDMD.Resources.font-7.fnt:{0}:-1", topBrightness)).Load();
                    var font12 = _assets.Load<Actors.Font>(string.Format("FlexDMD.Resources.font-12.fnt:{0}:-1", bottomBrightness)).Load();
                    // Two lines => f5by7 / f6by12
                    // Single lines => f14by26 or f12by24 or f7by13 to fit in, only evaluated at creation (not if text is changed afterward)
                    var scene = new Scene00(ResolveImage(background), toptext, font7, bottomtext, font12, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, "");
                    _queue.Enqueue(scene);
                });
            }
        }

        public void DisplayScene00Ex(string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut)
        {
            DisplayScene00ExWithId("", false, background, toptext, topBrightness, topOutlineBrightness, bottomtext, bottomBrightness, bottomOutlineBrightness, animateIn, pauseTime, animateOut);
        }

        public void DisplayScene00ExWithId(string sceneId, bool cancelPrevious, string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // log.Info("DisplayScene00Ex '{0}', '{1}', {2}, '{3}', {4}, {5}, {6}, {7}, {8}, {9}", background, toptext, topBrightness, topOutlineBrightness, bottomtext, bottomBrightness, bottomOutlineBrightness, animateIn, pauseTime, animateOut);
                    if (cancelPrevious)
                    {
                        // _queue.CancelRendering(sceneId);
                    }
                    // Used by Metal Slug when entering high score, or quite frequently by amh
                    _scoreBoard._visible = false;
                    // Two lines => f5by7 / f6by12
                    // Single lines => f14by26 or f12by24 or f7by13 to fit in
                    var font7 = _assets.Load<Actors.Font>(string.Format("FlexDMD.Resources.font-7.fnt:{0}:{1}", topBrightness, topOutlineBrightness)).Load();
                    var font12 = _assets.Load<Actors.Font>(string.Format("FlexDMD.Resources.font-12.fnt:{0}:{1}", bottomBrightness, bottomOutlineBrightness)).Load();
                    var scene = new Scene00(ResolveImage(background), toptext, font7, bottomtext, font12, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, sceneId);
                    _queue.Enqueue(scene);
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
                    if (scene != null && scene.Id == id && scene is Scene00 s)
                    {
                        s.SetText(toptext, bottomtext);
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
                    if (scene != null && scene.Id == id && scene is Scene00 s)
                    {
                        s.SetText(toptext, bottomtext);
                        s.SetPause(s.Time + pauseTime / 1000f);
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
                    log.Error("DisplayScene01 [unsupported] '{0}', '{1}', '{2}', {3}, {4}, {5}, {6}, {7}", sceneId, background, text, textBrightness, textOutlineBrightness, animateIn, pauseTime, animateOut);
                    _scoreBoard._visible = false;
                    // scene.SetSize(_width, _height);
                    // _queue.Enqueue(scene);
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
                        _assets.Load<Actors.Font>(string.Format("FlexDMD.Resources.font-7.fnt:{0}:-1", unselectedBrightness)).Load(),
                        _assets.Load<Actors.Font>(string.Format("FlexDMD.Resources.font-12.fnt:{0}:-1", selectedBrightness)).Load(),
                        _assets.Load<Actors.Font>(string.Format("FlexDMD.Resources.font-5.fnt:{0}:-1", unselectedBrightness)).Load());
                });
            }
        }

        public void DisplayScoreboard(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // Direct rendering: render only if the scene queue is empty, and no direct rendering has happened
                    // log.Info("Scoreboard for {0} players, {1} is playing", cPlayers, highlightedPlayer);
                    _scoreBoard._visible = true;
                    _scoreBoard.SetNPlayers(cPlayers);
                    _scoreBoard.SetHighlightedPlayer(highlightedPlayer);
                    _scoreBoard.SetScore(score1, score2, score3, score4);
                    _scoreBoard._lowerLeft.Text = lowerLeft;
                    _scoreBoard._lowerRight.Text = lowerRight;
                });
            }
        }

        // From KissDMDv2.vbs, an undocumented function. Not a clue for the difference between this one and DisplayScoreboard.
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
                    log.Error("DisplayText [unsupported] '{0}', {1}, {2}", text, textBrightness, textOutlineBrightness);
                    _scoreBoard._visible = false;
                    // Direct rendering
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
                    _scoreBoard._visible = false;
                    // scene.SetSize(_width, _height);
                    // _queue.Enqueue(scene);
                });
            }
        }
    }
}
