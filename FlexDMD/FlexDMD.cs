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
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using static FlexDMD.DMDDevice;
using NAudio.MediaFoundation;
using System.Reflection;
using System.IO;
using NLog.Config;
using System.Text.RegularExpressions;

namespace FlexDMD
{
    [Guid("766e10d3-dfe3-4e1b-ac99-c4d2be16e91f"), ComVisible(true), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IDMDObjectEvents))]
    public class FlexDMD : IFlexDMD
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly List<System.Action> _runnables = new List<System.Action>();
        private readonly Group _stage = new Group() { Name = "Stage" };
        private readonly int _frameRate = 60;
        private readonly Mutex _renderMutex = new Mutex();
        private DMDDevice _dmd = null;
        private Thread _processThread = null;
        private bool _run = false;
        private bool _show = true;
        private ushort _width = 128;
        private ushort _height = 32;
        private string _gameName = "";
        private Bitmap _frame = null;
        private object[] _pixels = null;
        private object[] _coloredPixels = null;
        private RenderMode _renderMode = RenderMode.GRAY_4;
        private Color _dmdColor = Color.FromArgb(0xFF, 0x58, 0x20);

        public event OnDMDChangedDelegate OnDMDChanged;
        public delegate void OnDMDChangedDelegate();

        public IGroupActor Stage { get => _stage; }
        public IGroupActor NewGroup(string name) { var g = new Group() { Name = name }; return g; }
        public IFrameActor NewFrame(string name) { var g = new Frame() { Name = name }; return g; }
        public ILabelActor NewLabel(string name, Font font, string text) { var g = new Label(font, text) { Name = name }; return g; }
        public IVideoActor NewVideo(string name, string video) { var g = (IVideoActor)ResolveImage(video); g.Name = name; return g; }
        public IImageActor NewImage(string name, string image) { var g = (IImageActor)ResolveImage(image); g.Name = name; return g; }
        public Font NewFont(string font, Color tint, Color borderTint, int borderSize) => AssetManager.Load<Font>(new FontDef(font, tint, borderTint, borderSize)).Load();
        public IUltraDMD NewUltraDMD() => new UltraDMD.UltraDMD(this);

        public Graphics Graphics { get; private set; } = null;

        public bool Run
        {
            get => _run;
            set
            {
                if (_run == value) return;
                _run = value;
                if (_run)
                {
                    ShowDMD(_show);
                    log.Info("Starting render thread for game '{0}'", _gameName);
                    _processThread = new Thread(new ThreadStart(RenderLoop)) { IsBackground = true };
                    _processThread.Start();
                }
                else
                {
                    log.Info("Stopping render thread");
                    if (Thread.CurrentThread != _processThread) _processThread?.Join();
                    _processThread = null;
                    ShowDMD(false);
                }
            }
        }

        public bool Show
        {
            get => _show;
            set
            {
                if (_show == value) return;
                _show = value;
                ShowDMD(_show);
            }
        }

        private void ShowDMD(bool show)
        {
            LockRenderThread();
            if (show && _dmd == null)
            {
                log.Info("Show DMD");
                _dmd = new DMDDevice();
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
                if (_gameName == "")
                {
                    WindowHandle editor = WindowHandle.FindWindow(wh => wh.GetClassName().Equals("VPinball") && wh.GetWindowText().StartsWith("Visual Pinball - ["));
                    if (editor != null)
                    {
                        // Used filename taken from window title, removing version pattern:
                        // - First separator: space, _, v, V
                        // - Then version: [0..9], ., _
                        // - Then subversion as letter: optional final char
                        // - Then optional qualifier: optional appended "-DOF"
                        // - Then optional * marker (modified in editor)
                        // - Then end of name
                        string title = editor.GetWindowText();
                        int trailer = "Visual Pinball - [".Length;
                        _gameName = title.Substring(trailer, title.Length - trailer - 1).Trim();
                        _gameName = new Regex("[\\s_vV][\\d_\\.]+[a-z]?(-DOF)?\\*?$").Replace(_gameName, "").Trim();
                        log.Info("GameName was not set, using file name instead: '" + _gameName + "'");
                    }
                }
                _dmd.GameSettings(_gameName, 0, options);
            }
            else if (!show && _dmd != null)
            {
                log.Info("Hide DMD");
                _dmd.Close();
                _dmd.Dispose();
                _dmd = null;
            }
            UnlockRenderThread();
        }

        public string GameName
        {
            get => _gameName;
            set
            {
                if (value == null)
                {
                    GameName = "";
                    return;
                }
                if (_gameName == value) return;
                bool wasVisible = Run;
                Run = false;
                log.Info("Game name set to {0}", value);
                _gameName = value;
                Run = wasVisible;
            }
        }

        public ushort Width
        {
            get => _width;
            set
            {
                if (_width < 1) return;
                if (_width == value) return;
                bool wasVisible = Run;
                Run = false;
                _width = value;
                Run = wasVisible;
            }
        }

        public ushort Height
        {
            get => _height;
            set
            {
                if (_height < 1) return;
                if (_height == value) return;
                bool wasVisible = Run;
                Run = false;
                _height = value;
                Run = wasVisible;
            }
        }

        public Color Color
        {
            get => _dmdColor;
            set
            {
                if (_dmdColor == value) return;
                bool wasVisible = Run;
                Run = false;
                _dmdColor = value;
                Run = wasVisible;
            }
        }

        public RenderMode RenderMode
        {
            get => _renderMode;
            set
            {
                if (_renderMode == value) return;
                bool wasVisible = Run;
                Run = false;
                log.Info("Render mode set to {0}", value);
                _renderMode = value;
                Run = wasVisible;
            }
        }

        public string ProjectFolder
        {
            get => AssetManager.BasePath;
            set
            {
                log.Info("SetProjectFolder {0}", value);
                AssetManager.BasePath = value;
            }
        }

        public string TableFile
        {
            get => AssetManager.TableFile;
            set
            {
                log.Info("Table File defined to {0}", value);
                AssetManager.TableFile = value;
            }
        }

        public bool Clear { get; set; } = false;

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

        public AssetManager AssetManager { get; } = new AssetManager();

        public FlexDMD()
        {
            MediaFoundationApi.Startup();

            var assembly = Assembly.GetCallingAssembly();
            var assemblyPath = Path.GetDirectoryName(new Uri(assembly.CodeBase).LocalPath);
            var logConfigPath = Path.Combine(assemblyPath, "FlexDMD.log.config");
            if (File.Exists(logConfigPath))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(logConfigPath, true);
                LogManager.ReconfigExistingLoggers();
            }
            
            log.Info("FlexDMD version {0}", assembly.GetName().Version);
        }

        ~FlexDMD()
        {
            if (_run)
            {
                log.Error("Destructor called before Uninit");
                Show = false;
            }
            MediaFoundationApi.Shutdown();
        }

        public void LockRenderThread()
        {
            _renderMutex.WaitOne();
        }

        public void UnlockRenderThread()
        {
            _renderMutex.ReleaseMutex();
        }

        public void Post(System.Action runnable)
        {
            lock (_runnables)
            {
                _runnables.Add(runnable);
            }
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
        public Actor ResolveImage(string filename)
        {
            try
            {
                if (AssetManager.FileExists(filename))
                {
                    switch (AssetManager.GetFileType(filename))
                    {
                        case FileType.Image:
                            return new Image(AssetManager.Load<Bitmap>(filename).Load());
                        case FileType.Gif:
                            return new GIFImage(AssetManager.Load<Bitmap>(filename).Load());
                        case FileType.Video:
                            return new Video(Path.Combine(AssetManager.BasePath, filename), false);
                    }
                }
                else if (filename.Contains(","))
                {
                    var def = new ImageSequenceDef(filename, 25, true);
                    return AssetManager.Load<ImageSequence>(def).Load();
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Exception while resolving image: '{0}'", filename);
            }
            log.Error("Missing resource '{0}'", filename);
            return null;
        }

        public void RenderLoop()
        {
            log.Info("RenderThread start");
            _frame = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
            Graphics = Graphics.FromImage(_frame);
            _stage.SetSize(_width, _height);
            _stage.InStage = true;
            Stopwatch stopWatch = new Stopwatch();
            WindowHandle visualPinball = null;
            IntPtr _bpFrame = _renderMode != RenderMode.RGB ? _bpFrame = Marshal.AllocHGlobal(_width * _height) : IntPtr.Zero;
            double elapsedMs = 0.0;
            while (Run)
            {
                stopWatch.Restart();
                if (visualPinball == null)
                {
                    visualPinball = WindowHandle.FindWindow(wh => wh.IsVisible() && wh.GetClassName().Equals("VPPlayer") && wh.GetWindowText().StartsWith("Visual Pinball Player"));
                    if (visualPinball != null)
                        log.Info("Attaching to Visual Pinball Player window lifecycle");
                }
                else if (!visualPinball.IsWindow())
                {
                    log.Info("Closing FlexDMD since Visual Pinball Player window was closed");
                    Run = false;
                    break;
                }
                if (Clear) Graphics.Clear(Color.Black);
                _renderMutex.WaitOne();
                lock (_runnables)
                {
                    _runnables.ForEach(item => item());
                    _runnables.Clear();
                }
                _stage.Update((float)(elapsedMs / 1000.0));
                _stage.Draw(Graphics);
                _renderMutex.ReleaseMutex();
                Rectangle rect = new Rectangle(0, 0, _frame.Width, _frame.Height);
                BitmapData data = _frame.LockBits(rect, ImageLockMode.ReadWrite, _frame.PixelFormat);
                switch (_renderMode)
                {
                    case RenderMode.GRAY_2:
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
                            _dmd?.RenderGray2(_width, _height, _bpFrame);
                        }
                        catch (Exception) { }
                        break;

                    case RenderMode.GRAY_4:
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
                            _dmd?.RenderGray4(_width, _height, _bpFrame);
                        }
                        catch (Exception) { }
                        break;

                    case RenderMode.RGB:
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
                            _dmd?.RenderRgb24(_width, _height, data.Scan0);
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
            _stage.InStage = false;
            if (_bpFrame != IntPtr.Zero) Marshal.FreeHGlobal(_bpFrame);
            Graphics.Dispose();
            Graphics = null;
            _frame.Dispose();
            _frame = null;
            _processThread = null;
            log.Info("RenderThread end");
        }
    }
}
