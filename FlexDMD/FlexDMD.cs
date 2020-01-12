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
using MediaFoundation;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using MediaFoundation.Misc;
using static FlexDMD.DMDDevice;

namespace FlexDMD
{
    [Guid("766e10d3-dfe3-4e1b-ac99-c4d2be16e91f"), ComVisible(true), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IDMDObjectEvents))]
    public class FlexDMD : IFlexDMD
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly List<System.Action> _runnables = new List<System.Action>();
        private readonly AssetManager _assets = new AssetManager();
        private readonly Group _stage = new Group();
        private readonly int _frameRate = 60;
        private DMDDevice _dmd = null;
        private Mutex _renderMutex = new Mutex();
        private Thread _processThread = null;
        private bool _running = false;
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
        public IGroupActor NewGroup() => new Group();
        public IFrameActor NewFrame() => new Frame();
        public ILabelActor NewLabel(Font font, string text) => new Label(font, text);
        public IVideoActor NewVideo(string video) => (IVideoActor)ResolveImage(video);
        public IImageActor NewImage(string image) => (IImageActor)ResolveImage(image);
        public Font NewFont(string font, float brightness, float outline) => _assets.Load<Font>(new FontDef(font, brightness, outline)).Load();

        public IUltraDMD NewUltraDMD()
        {
            if (_processThread == null)
            {
                log.Error("UltraDMD object may only be created after FlexDMD initialization. Call Init First.");
                return null;
            }
            return new UltraDMD.UltraDMD(this);
        }

        public Graphics Graphics { get; private set; } = null;

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

        public ushort Width
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

        public ushort Height
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

        public Color Color
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

        public RenderMode RenderMode
        {
            get => _renderMode;
            set
            {
                RenderMode mode = value;
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

        public string ProjectFolder
        {
            get => _assets.BasePath;
            set
            {
				log.Info("SetProjectFolder {0}", value);
				_assets.BasePath = value;
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

        public FlexDMD()
        {
            HResult hr = MFExtern.MFStartup(0x10070, MFStartup.Lite);
            if (MFError.Failed(hr)) log.Error("Failed to initialize Microsoft Media Foundation: {0}", hr);
        }

        ~FlexDMD()
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
            _dmd.Close();
            _dmd.Dispose();
            _dmd = null;
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
                if (_assets.FileExists(filename))
                {
                    switch (_assets.GetFileType(filename))
                    {
                        case FileType.Image:
                            return new Image(_assets.Load<Bitmap>(filename).Load());
                        case FileType.Gif:
                            return new GIFImage(_assets.Load<Bitmap>(filename).Load());
                        case FileType.Video:
                            return new Video(System.IO.Path.Combine(_assets.BasePath, filename), false);
                    }
                }
                else if (filename.Contains(","))
                {
                    var def = new AnimatedImageDef(filename, 25, true);
                    // TODO return _assets.Load<AnimatedImage>(def).Load().newInstance();
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
            _frame = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
            Graphics = Graphics.FromImage(_frame);
            _stage.SetSize(_width, _height);
			_stage.InStage = true;
            Stopwatch stopWatch = new Stopwatch();
            WindowHandle visualPinball = null;
            IntPtr _bpFrame = _renderMode != RenderMode.RGB ? _bpFrame = Marshal.AllocHGlobal(_width * _height) : IntPtr.Zero;
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
                            _dmd.RenderGray2(_width, _height, _bpFrame);
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
                            _dmd.RenderGray4(_width, _height, _bpFrame);
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
                            _dmd.RenderRgb24(_width, _height, data.Scan0);
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
        }
    }
}
