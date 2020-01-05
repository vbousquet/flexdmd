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
using FlexDMD.Actors;
using FlexDMD.Scenes;
using MediaFoundation;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using static FlexDMD.DMDDevice;
using MediaFoundation.Misc;

namespace FlexDMD
{

    public enum RenderMode
    {
        GRAY_2, GRAY_4, RGB
    }

    [Guid("766e10d3-dfe3-4e1b-ac99-c4d2be16e91f"), ComVisible(true), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IDMDObjectEvents))]
    public class DMDObject : IDMDObject
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly List<Action> _runnables = new List<Action>();
        private DMDDevice _dmd = null;
        private readonly AssetManager _assets = new AssetManager();
        private readonly SceneQueue _queue = new SceneQueue();
        private readonly Group _stage = new Group();
        private readonly int _frameRate = 60;
        private readonly Dictionary<int, object> _preloads = new Dictionary<int, object>();
        private ScoreBoard _scoreBoard;
        private FontDef _scoreFontText, _scoreFontNormal, _scoreFontHighlight;
        private FontDef _twoLinesFontTop, _twoLinesFontBottom;
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
        private RenderMode _renderMode = FlexDMD.RenderMode.GRAY_4;
        private Color _dmdColor = Color.FromArgb(0xFF, 0x58, 0x20);

        public delegate void OnDMDChangedDelegate();

        public string GameName
        {
            get => _gameName;
            set
            {
                if (_gameName == value) return;
                if (_processThread != null)
                {
                    log.Warn("Game name after initialization. Reinitialization performed.");
                    Uninit();
                    _gameName = value;
                    Init();
                }
                else
                {
                    log.Info("Game name set to {0}", value);
                    _gameName = value;
                }
            }
        }

        public ushort DmdWidth
        {
            get => _width;
            set
            {
                if (_width == value) return;
                if (_processThread != null)
                {
                    log.Warn("Width changed after initialization. Reinitialization performed.");
                    Uninit();
                    _width = value;
                    Init();
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
                    log.Warn("Height changed after initialization. Reinitialization performed.");
                    Uninit();
                    _height = value;
                    Init();
                }
                else
                {
                    log.Info("DMD height set to {0}", value);
                    _height = value;
                }
            }
        }

        public int RenderMode
        {
            get => (int)_renderMode;
            set
            {
                RenderMode mode = (RenderMode)value;
                if (_renderMode == mode) return;
                if (_processThread != null)
                {
                    log.Warn("Render mode changed after initialization. Reinitialization performed.");
                    Uninit();
                    _renderMode = mode;
                    Init();
                }
                else
                {
                    log.Info("Render mode set to {0}", value);
                    _renderMode = mode;
                }
            }
        }

        public string TableFile
        {
            get => _assets.TableFile;
            set
            {
                log.Info("Table File defined to {0}", value);
                _assets.TableFile = value;
            }
        }

        public Color DmdColor
        {
            get => _dmdColor;
            set
            {
                if (_dmdColor == value) return;
                if (_processThread != null)
                {
                    log.Warn("Color changed after initialization. Reinitialization performed.");
                    Uninit();
                    _dmdColor = value;
                    Init();
                }
                else
                {
                    log.Info("Color set to {0}", value);
                    _dmdColor = value;
                }
            }
        }

        public object DmdColoredPixels
        {
            get
            {
                if (_coloredPixels == null || _coloredPixels.Length != _width * _height) _coloredPixels = new object[_width * _height];
                return _coloredPixels;
            }
        }

        public object DmdPixels
        {
            get
            {
                if (_pixels == null || _pixels.Length != _width * _height) _pixels = new object[_width * _height];
                return _pixels;
            }
        }

        public DMDObject()
        {
            HResult hr = MFExtern.MFStartup(0x10070, MFStartup.Lite);
            if (MFError.Failed(hr)) log.Error("Failed to initialize Microsoft Media Foundation: {0}", hr);
            // UltraDMD uses f4by5 / f5by7 / f6by12
            _scoreFontText = new FontDef("FlexDMD.Resources.f4by5.fnt", 0.66f);
            _scoreFontNormal = new FontDef("FlexDMD.Resources.f5by7.fnt", 0.66f);
            _scoreFontHighlight = new FontDef("FlexDMD.Resources.f6by12.fnt");
            // UltraDMD uses f14by26 or f12by24 or f7by13 to fit in
            _singleLineFont = new FontDef[] {
                new FontDef("FlexDMD.Resources.f14by26.fnt"),
                new FontDef("FlexDMD.Resources.f12by24.fnt"),
                new FontDef("FlexDMD.Resources.f7by13.fnt")
            };
            // UltraDMD uses f5by7 / f6by12 for top / bottom line
            _twoLinesFontTop = new FontDef("FlexDMD.Resources.f5by7.fnt");
            _twoLinesFontBottom = new FontDef("FlexDMD.Resources.f6by12.fnt");
            // Core rendering tree
            _scoreBoard = new ScoreBoard(
                _assets.Load<Font>(_scoreFontNormal).Load(),
                _assets.Load<Font>(_scoreFontHighlight).Load(),
                _assets.Load<Font>(_scoreFontText).Load()
                );
            _stage.AddActor(_scoreBoard);
            _stage.AddActor(_queue);
        }

        ~DMDObject()
        {
            if (_running)
            {
                log.Error("Destructor called before Uninit");
                Uninit();
            }
            HResult hr = MFExtern.MFShutdown();
            if (hr < 0) log.Error("Failed to dispose Microsoft Media Foundation: {0}", hr);
        }

        public void Init()
        {
            if (_running) return;
            log.Info("Init {0}", _gameName);
            _running = true;
            _dmd = new DMDDevice();
            SetVisibleVirtualDMD(true);
            _processThread = new Thread(new ThreadStart(RenderLoop));
            _processThread.IsBackground = true;
            _processThread.Start();
        }

        public void Uninit()
        {
            if (!_running) return;
            log.Info("Uninit");
            _running = false;
            _processThread?.Join();
            SetVisibleVirtualDMD(false);
            _dmd.Dispose();
            _dmd = null;
        }

        public void RenderLoop()
        {
            _frame = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
            _graphics = Graphics.FromImage(_frame);
            _stage.SetSize(_width, _height);
            _scoreBoard.SetSize(_width, _height);
            Stopwatch stopWatch = new Stopwatch();
            WindowHandle visualPinball = null;
            IntPtr _bpFrame = _renderMode != FlexDMD.RenderMode.RGB ? _bpFrame = Marshal.AllocHGlobal(_width * _height) : IntPtr.Zero;
            double elapsedMs = 0.0;
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
                    Uninit();
                    break;
                }
                lock (_runnables)
                {
                    _runnables.ForEach(item => item());
                    _runnables.Clear();
                }
                _stage.Update((float)(elapsedMs / 1000.0));
                _scoreBoard.Visible &= !_queue.IsRendering();
                _stage.Draw(_graphics);
                Rectangle rect = new Rectangle(0, 0, _frame.Width, _frame.Height);
                BitmapData data = _frame.LockBits(rect, ImageLockMode.ReadWrite, _frame.PixelFormat);
                switch (_renderMode)
                {
                    case FlexDMD.RenderMode.GRAY_2:
                        unsafe
                        {
                            byte* dst = (byte*)_bpFrame.ToPointer();
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
                                    int v = (int)(0.2126f * r + 0.7152f * g + 0.0722f * b);
                                    if (v > 255) v = 255;
                                    dst[pos] = (byte)(v >> 6);
                                    if (_pixels != null) _pixels[pos] = (byte)(v);
                                    pos++;
                                }
                                ptr += data.Stride - 3 * _width;
                            }
                        }
                        try
                        {
                            if (_visible) _dmd.RenderGray2(_width, _height, _bpFrame);
                        }
                        catch (Exception) { }
                        break;

                    case FlexDMD.RenderMode.GRAY_4:
                        unsafe
                        {
                            byte* dst = (byte*)_bpFrame.ToPointer();
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
                                    int v = (int)(0.2126f * r + 0.7152f * g + 0.0722f * b);
                                    if (v > 255) v = 255;
                                    dst[pos] = (byte)(v >> 4);
                                    if (_pixels != null) _pixels[pos] = (byte)(v);
                                    pos++;
                                }
                                ptr += data.Stride - 3 * _width;
                            }
                        }
                        try
                        {
                            if (_visible) _dmd.RenderGray4(_width, _height, _bpFrame);
                        }
                        catch (Exception) { }
                        break;

                    case FlexDMD.RenderMode.RGB:
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
                                        float v = 0.2126f * r + 0.7152f * g + 0.0722f * b;
                                        if (v > 255.0f) v = 255.0f;
                                        _pixels[pos] = (byte)(v);
                                        pos++;
                                    }
                                }
                            }
                        }
                        try
                        {
                            if (_visible) _dmd.RenderRgb24(_width, _height, data.Scan0);
                        }
                        catch (Exception) { }
                        break;
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
                double renderingDuration = stopWatch.Elapsed.TotalMilliseconds;

                int sleepMs = (1000 / _frameRate) - (int)renderingDuration;
                if (sleepMs > 0) Thread.Sleep(sleepMs);
                elapsedMs = stopWatch.Elapsed.TotalMilliseconds;
                // log.Info("Elapsed: {0}ms", elapsedMs);
            }
            if (_bpFrame != IntPtr.Zero) Marshal.FreeHGlobal(_bpFrame);
            _graphics.Dispose();
            _graphics = null;
            _frame.Dispose();
            _frame = null;
            _processThread = null;
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
            log.Info("SetVisibleVirtualDMD({0})", bVisible);
            bool wasVisible = _visible;
            _visible = bVisible;
            if (!wasVisible && _visible)
            {
                _dmd.Open();
                var options = new PMoptions
                {
                    Red = _dmdColor.R,
                    Green = _dmdColor.G,
                    Blue = _dmdColor.B,
                    Perc66 = 66,
                    Perc33 = 33,
                    Perc0 = 0
                };
                options.Green0 = options.Perc0 * options.Green / 100;
                options.Green33 = options.Perc33 * options.Green / 100;
                options.Green66 = options.Perc66 * options.Green / 100;
                options.Blue0 = options.Perc0 * options.Blue / 100;
                options.Blue33 = options.Perc33 * options.Blue / 100;
                options.Blue66 = options.Perc66 * options.Blue / 100;
                options.Red0 = options.Perc0 * options.Red / 100;
                options.Red33 = options.Perc33 * options.Red / 100;
                options.Red66 = options.Perc66 * options.Red / 100;
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
                    // log.Info("CancelRenderingWithId {0}", sceneId);
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
            var def = new AnimatedImageDef(imagelist, fps, loop);
            _preloads[id] = _assets.Load<AnimatedImage>(def).Load();
            return id;
        }

        public int RegisterVideo(int videoStretchMode, bool loop, string videoFilename)
        {
            var id = _nextId;
            _nextId++;
            var video = new Video(System.IO.Path.Combine(_assets.BasePath, videoFilename), loop, videoStretchMode);
            _preloads[id] = video;
            return id;
        }

        /// Resolve image files.
        /// For file resolving rules (resources, VPX files, path search,...), see AssetManager.OpenStream.
        ///
        /// File name is interpreted to be either:
        /// <list type="bullet">
        /// <item>
        /// <description>a preloaded id (converted to a string),</description>
        /// </item>
        /// <item>
        /// <description>a comma separated list of images filenames, which will be played as a looping animation at 25 FPS,</description>
        /// </item>
        /// <item>
        /// <description>a filename resolving to an image, gif or video file.</description>
        /// </item>
        /// </list>
        private Actor ResolveImage(string filename)
        {
            try
            {
                int preloadId = 0;
                if (Int32.TryParse(filename, out preloadId) && _preloads.ContainsKey(preloadId))
                {
                    var preload = _preloads[preloadId];
                    if (preload is Video v)
                    {
                        return v.newInstance();
                    }
                    else if (preload is AnimatedImage ai)
                    {
                        return ai.newInstance();
                    }
                }
                else if (_assets.FileExists(filename))
                {
                    switch (_assets.GetFileType(filename))
                    {
                        case FileType.Image:
                            return new Image(_assets.Load<Bitmap>(filename).Load());
                        case FileType.Gif:
                            return new GIFImage(_assets.Load<Bitmap>(filename).Load());
                        case FileType.Video:
                            return new Video(System.IO.Path.Combine(_assets.BasePath, filename), false, _stretchMode);
                    }
                }
                else if (filename.Contains(","))
                {
                    var def = new AnimatedImageDef(filename, 25, true);
                    return _assets.Load<AnimatedImage>(def).Load().newInstance();
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Exception while resolving image: '{0}'", filename);
            }
            // TODO Returns a 2 pixel wide frame in this situation (for some scene only to be compatible with UltraDMD)
            log.Error("Missing resource '{0}'", filename);
            // return new Actor();
            return new Frame();
        }

        private Label GetFittedLabel(string text, float fillBrightness, float outlineBrightness)
        {
            foreach (FontDef f in _singleLineFont)
            {
                var font = _assets.Load<Font>(new FontDef(f.Path, fillBrightness, outlineBrightness)).Load();
                var label = new Label(font, text);
                label.SetPosition((_width - label.Width) / 2, (_height - label.Height) / 2);
                if ((label.X >= 0 && label.Y >= 0) || f == _singleLineFont[_singleLineFont.Length - 1]) return label;
            }
            return null;
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
                        var fontTop = _assets.Load<Font>(new FontDef(_twoLinesFontTop.Path, topBrightness / 15f, topOutlineBrightness / 15f)).Load();
                        var fontBottom = _assets.Load<Font>(new FontDef(_twoLinesFontBottom.Path, bottomBrightness / 15f, bottomOutlineBrightness / 15f)).Load();
                        var scene = new TwoLineScene(ResolveImage(background), toptext, fontTop, bottomtext, fontBottom, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, sceneId);
                        _queue.Enqueue(scene);
                    }
                    else if (toptext != null && toptext.Length > 0)
                    {
                        var font = GetFittedLabel(toptext, topBrightness / 15f, topOutlineBrightness / 15f).Font;
                        var scene = new SingleLineScene(ResolveImage(background), toptext, font, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, false, sceneId);
                        _queue.Enqueue(scene);
                    }
                    else if (bottomtext != null && bottomtext.Length > 0)
                    {
                        var font = GetFittedLabel(bottomtext, bottomBrightness / 15f, bottomOutlineBrightness / 15f).Font;
                        var scene = new SingleLineScene(ResolveImage(background), bottomtext, font, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, false, sceneId);
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
                    // log.Info("DisplayScene01 '{0}', '{1}', '{2}', {3}, {4}, {5}, {6}, {7}", sceneId, background, text, textBrightness, textOutlineBrightness, animateIn, pauseTime, animateOut);
                    _scoreBoard.Visible = false;
                    var font = _assets.Load<Font>(new FontDef(_singleLineFont[0].Path, textBrightness / 15f, textOutlineBrightness / 15f)).Load();
                    var scene = new SingleLineScene(ResolveImage(background), text, font, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, true, sceneId);
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
                        _assets.Load<Font>(new FontDef(_scoreFontNormal.Path, unselectedBrightness)).Load(),
                        _assets.Load<Font>(new FontDef(_scoreFontHighlight.Path, selectedBrightness)).Load(),
                        _assets.Load<Font>(new FontDef(_scoreFontText.Path, unselectedBrightness)).Load());
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
                    log.Error("DisplayText [untested] '{0}', {1}, {2}", text, textBrightness, textOutlineBrightness);
                    _scoreBoard.Visible = false;
                    GetFittedLabel(text, textBrightness / 15f, textOutlineBrightness / 15).Draw(_graphics);
                });
            }
        }

        public void ScrollingCredits(string background, string text, int textBrightness, int animateIn, int pauseTime, int animateOut)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // log.Info("ScrollingCredits '{0}', '{1}', {2}", background, text, textBrightness);
                    _scoreBoard.Visible = false;
                    string[] lines = text.Split(new Char[] { '\n', '|' });
                    var font12 = _assets.Load<Font>(new FontDef("FlexDMD.Resources.font-12.fnt", textBrightness / 15f, -1)).Load();
                    var scene = new ScrollingCreditsScene(ResolveImage(background), lines, font12, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut);
                    _queue.Enqueue(scene);
                });
            }
        }

        public void SetScoreFonts(string textFont, string normalFont, string highlightFont, int selectedBrightness, int unselectedBrightness)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    _scoreFontText = new FontDef(textFont);
                    _scoreFontNormal = new FontDef(normalFont);
                    _scoreFontHighlight = new FontDef(highlightFont);
                    _scoreBoard.SetFonts(
                        _assets.Load<Font>(new FontDef(_scoreFontNormal.Path, unselectedBrightness)).Load(),
                        _assets.Load<Font>(new FontDef(_scoreFontHighlight.Path, selectedBrightness)).Load(),
                        _assets.Load<Font>(new FontDef(_scoreFontText.Path, unselectedBrightness)).Load());
                });
            }
        }

        public void SetTwoLineFonts(string topFont, string bottomFont)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    _twoLinesFontTop = new FontDef(topFont);
                    _twoLinesFontBottom = new FontDef(bottomFont);
                });
            }
        }

        public void SetSingleLineFonts(object fonts)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    var fts = (object[])fonts;
                    _singleLineFont = new FontDef[fts.Length];
                    for (int i = 0; i < fts.Length; i++)
                        _singleLineFont[i] = new FontDef(fts[i].ToString());
                });
            }
        }

        public void DisplayJPSScene(string id, string background, object top, object bottom, Int32 animateIn, Int32 pauseTime, Int32 animateOut)
        {
            lock (_runnables)
            {
                _runnables.Add(() =>
                {
                    // log.Info("DisplayJPScene [alpha stuff] id={0} background={1}, {2}, {3}", id, background, top, bottom);
                    _scoreBoard.Visible = false;
                    JPSScene scene = null;
                    if (id != null && id.Length > 0)
                    {
                        var s = _queue.GetSceneById(id);
                        if (s != null && s is JPSScene sc)
                        {
                            scene = sc;
                            if (scene.Background == null || !background.Equals(scene.Background.Info)) scene.Background = ResolveImage(background);
                        }
                    }
                    if (scene == null)
                    {
                        scene = new JPSScene(ResolveImage(background), (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, id);
                        scene.Background.Info = background;
                        _queue.Enqueue(scene);
                    }
                    for (int col = 0; col < 16; col++)
                        scene.SetImage(0, col, ResolveImage(((object[])top)[col].ToString()));
                    for (int col = 0; col < 20; col++)
                        scene.SetImage(1, col, ResolveImage(((object[])bottom)[col].ToString()));
                });
            }
        }
    }

}
