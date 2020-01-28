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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace FlexDMD
{

    public enum FileType
    {
        Unknow, Image, Gif, Video
    }

    public interface IBitmapFilter
    {
        Bitmap Filter(Bitmap src);
    }

    public class DotFilter : IBitmapFilter
    {
        public int _dotSize = 2;
        public int _offset = 0;

        public Bitmap Filter(Bitmap src)
        {
            var dst = new Bitmap(src.Width / _dotSize, src.Height / _dotSize, src.PixelFormat);
            for (int y = 0; y < dst.Height; y++)
            {
                for (int x = 0; x < dst.Width; x++)
                {
                    dst.SetPixel(x, y, src.GetPixel(x * _dotSize + (_dotSize - 1) - _offset, y * _dotSize + (_dotSize - 1)));
                }
            }
            return dst;
        }
    }

    public class AdditiveFilter : IBitmapFilter
    {
        public Bitmap Filter(Bitmap src)
        {
            var dst = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
            for (int y = 0; y < dst.Height; y++)
            {
                for (int x = 0; x < dst.Width; x++)
                {
                    var pixel = src.GetPixel(x, y);
                    // var alpha = (int)(4 * ((0.21 * pixel.R + 0.72 * pixel.G + 0.07 * pixel.B) * pixel.A) / 255);
                    // if (alpha > 255) alpha = 255;
                    if (pixel.R < 64 && pixel.G < 64 && pixel.B < 64)
                        dst.SetPixel(x, y, Color.FromArgb(0, pixel));
                    else
                        dst.SetPixel(x, y, pixel);
                }
            }
            return dst;
        }
    }
	
	public class VideoDef
	{
		public string VideoFilename { get; set; } = "";
        public Scaling Scaling { get; set; } = Scaling.Stretch;
        public Alignment Alignment { get; set; } = Alignment.Center;
		public bool Loop { get; set; } = false;

        public override bool Equals(object obj)
        {
            return obj is VideoDef def &&
                   VideoFilename == def.VideoFilename &&
                   Scaling == def.Scaling &&
                   Alignment == def.Alignment &&
                   Loop == def.Loop;
        }

        public override int GetHashCode()
        {
            var hashCode = 96768724;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(VideoFilename);
            hashCode = hashCode * -1521134295 + Scaling.GetHashCode();
            hashCode = hashCode * -1521134295 + Alignment.GetHashCode();
            hashCode = hashCode * -1521134295 + Loop.GetHashCode();
            return hashCode;
        }
    }

    public class ImageSequenceDef
    {
        public List<string> _images;
        public Scaling Scaling { get; set; } = Scaling.Stretch;
        public Alignment Alignment { get; set; } = Alignment.Center;
        public int Fps { get; set; } = 25;
        public bool Loop { get; set; } = true;

        public ImageSequenceDef(string images, int fps, bool loop)
        {
            _images = new List<string>();
            foreach (string image in images.Split(','))
                _images.Add(image.Trim());
            Fps = fps;
            Loop = loop;
        }

        public override bool Equals(object obj)
        {
            return obj is ImageSequenceDef def &&
                   EqualityComparer<List<string>>.Default.Equals(_images, def._images) &&
                   Fps == def.Fps &&
                   Loop == def.Loop;
        }

        public override int GetHashCode()
        {
            var hashCode = -2035125405;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(_images);
            hashCode = hashCode * -1521134295 + Fps.GetHashCode();
            hashCode = hashCode * -1521134295 + Loop.GetHashCode();
            return hashCode;
        }
    }

    public class FontDef
    {
        public float FillBrightness { get; set; } = 1f;
        public float OutlineBrightness { get; set; } = -1f;
        public string Path { get; set; } = "";

        public FontDef(string path, float fillBrightness = 1f, float outlineBrightness = -1f)
        {
            Path = path;
            FillBrightness = fillBrightness;
            OutlineBrightness = outlineBrightness;
        }

        public override bool Equals(object obj)
        {
            return obj is FontDef def &&
                   FillBrightness == def.FillBrightness &&
                   OutlineBrightness == def.OutlineBrightness &&
                   Path == def.Path;
        }

        public override int GetHashCode()
        {
            var hashCode = -1876634251;
            hashCode = hashCode * -1521134295 + FillBrightness.GetHashCode();
            hashCode = hashCode * -1521134295 + OutlineBrightness.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
            return hashCode;
        }

        public override string ToString()
        {
            return string.Format("FontDef [path={0}, fill={1}, outline={2}]", Path, FillBrightness, OutlineBrightness);
        }

    }

    public class Asset<T>
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly AssetManager _assets;
        private readonly object _id;
        private T _value;
        public bool _loaded;
        public int _refCount;

        public Asset(AssetManager assets, object id)
        {
            _id = id;
            _assets = assets;
            _refCount = 1;
        }

        public T Value
        {
            get
            {
                if (!_loaded) throw new InvalidOperationException("Asset must be loaded before accessing its value.");
                return _value;
            }
        }

        public T Load()
        {
            if (!_loaded)
            {
                if (typeof(T) == typeof(Bitmap) && _id.GetType() == typeof(string))
                {
                    log.Info("New bitmap added to asset manager: {0}", _id);
                    // TODO stream should closed on unload (but kept open for the lifetime of the Bitmap)
                    Bitmap image = new Bitmap(_assets.OpenStream((string)_id));
                    foreach (IBitmapFilter filter in _assets.GetFilters((string)_id))
                        image = filter.Filter(image);
                    if (!Array.Exists(image.FrameDimensionsList, e => e == FrameDimension.Time.Guid))
                    {
                        // Only convert for still image; animate ones are converted when played
                        GraphicUtils.BGRtoRGB(image);
                    }
                    _value = (T)Convert.ChangeType(image, typeof(T));
                    _loaded = true;
                }
                else if (typeof(T) == typeof(Font) && _id.GetType() == typeof(FontDef))
                {
                    var fontDef = (FontDef)_id;
                    log.Info("New font added to asset manager: {0}", fontDef);
                    var font = new Font(_assets, fontDef, fontDef.FillBrightness, fontDef.OutlineBrightness);
                    _value = (T)Convert.ChangeType(font, typeof(T));
                    _loaded = true;
                }
                else if (typeof(T) == typeof(ImageSequence) && _id.GetType() == typeof(ImageSequenceDef))
                {
                    ImageSequenceDef def = (ImageSequenceDef)_id;
                    List<Bitmap> images = new List<Bitmap>();
                    foreach (string filename in def._images)
                    {
                        var bmp = _assets.Load<Bitmap>(filename).Load();
                        images.Add(bmp);
                    }
                    ImageSequence actor = new ImageSequence(images, def.Fps, def.Loop);
                    _value = (T)Convert.ChangeType(actor, typeof(T));
                    _loaded = true;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unsupported asset type {0}", typeof(T)));
                }
            }
            return _value;
        }

        public void Unload()
        {
            if (_loaded && _value != null)
            {
                if (typeof(T) == typeof(Bitmap) && _value is Bitmap bmp)
                {
                    bmp.Dispose();
                }
                _value = default;
            }
            _loaded = false;
        }
    }

    public class AssetManager : IDisposable
    {
        private readonly Dictionary<object, object> _cache = new Dictionary<object, object>();
        private VPXFile _vpxFile = null;

        public string BasePath { get; set; } = "./";
        public string TableFile { get; set; } = null;

        public AssetManager()
        {
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// Resolve then open a stream for the given path. It is the responsibility of the caller to close the stream.
        ///
        /// File names are resolved using the following rules:
        /// <list type="bullet">
        /// <item>
        /// <description>If the file name starts with 'FlexDMD.Resources.' then the file is searched inside FlexDMD's embedded resources,</description>
        /// </item>
        /// <item>
        /// <description>If the file name starts with 'VPX.' then the file is searched inside the VPX table file,</description>
        /// </item>
        /// <item>
        /// <description>Otherwise, the file is searched in the project folder (see FlexDMD.SetProjectFolder).</description>
        /// </item>
        /// </list>
        ///
        /// Note that for the time being, for videos, only files placed in the project folder are supported.
        ///
        /// </summary>
        public Stream OpenStream(string path, string siblingPath = null)
        {
            if (path.Contains("&")) path = path.Split('&')[0];
            if (siblingPath != null)
            {
                if (siblingPath.StartsWith("FlexDMD.Resources."))
                    path = "FlexDMD.Resources." + path;
                else if (siblingPath.StartsWith("VPX."))
                    path = "VPX." + path;
                else
                    path = Path.Combine(siblingPath, "..", path);
            }
            if (path.StartsWith("FlexDMD.Resources."))
            {
                return Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            }
            else if (path.StartsWith("VPX."))
            {
                if (_vpxFile == null && TableFile != null && File.Exists(Path.Combine(BasePath, TableFile)))
                {
                    _vpxFile = new VPXFile(Path.Combine(BasePath, TableFile));
                }
                if (_vpxFile != null)
                {
                    return _vpxFile.OpenStream(GetVPXId(path));
                }
                return null;
            }
            else
            {
                var fullPath = Path.Combine(BasePath, path);
                return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            }
        }

        public bool FileExists(string path, string siblingPath = null)
        {
            if (path.Contains("&")) path = path.Split('&')[0];
            if (siblingPath != null)
            {
                if (siblingPath.StartsWith("FlexDMD.Resources."))
                    path = "FlexDMD.Resources." + path;
                else if (siblingPath.StartsWith("VPX."))
                    path = "VPX." + path;
                else
                    path = Path.Combine(siblingPath, "..", path);
            }
            if (path.StartsWith("FlexDMD.Resources."))
            {
                return Assembly.GetExecutingAssembly().GetManifestResourceInfo(path) != null;
            }
            else if (path.StartsWith("VPX."))
            {
                // TODO do not keept the file open (this blocks editing)
                if (_vpxFile == null && TableFile != null && File.Exists(Path.Combine(BasePath, TableFile)))
                {
                    _vpxFile = new VPXFile(Path.Combine(BasePath, TableFile));
                }
                if (_vpxFile != null)
                {
                    return _vpxFile.Contains(GetVPXId(path));
                }
                return false;
            }
            else
            {
                var fullPath = Path.Combine(BasePath, path);
                return File.Exists(fullPath);
            }
        }

        public FileType GetFileType(string path, string siblingPath = null)
        {
            if (path.Contains("&")) path = path.Split('&')[0];
            if (siblingPath != null)
            {
                if (siblingPath.StartsWith("FlexDMD.Resources."))
                    path = "FlexDMD.Resources." + path;
                else if (siblingPath.StartsWith("VPX."))
                    path = "VPX." + path;
                else
                    path = Path.Combine(siblingPath, "..", path);
            }
            if (path.StartsWith("FlexDMD.Resources."))
            {
                return GetTypeFromExt(path);
            }
            else if (path.StartsWith("VPX."))
            {
                if (_vpxFile == null && TableFile != null && File.Exists(Path.Combine(BasePath, TableFile)))
                {
                    _vpxFile = new VPXFile(Path.Combine(BasePath, TableFile));
                }
                if (_vpxFile != null)
                {
                    var file = _vpxFile.GetImportFile(GetVPXId(path));
                    if (file != null) return GetTypeFromExt(file);
                }
                return FileType.Unknow;
            }
            else
            {
                var fullPath = Path.Combine(BasePath, path);
                return GetTypeFromExt(fullPath);
            }
        }

        private FileType GetTypeFromExt(string file)
        {
            string extension = Path.GetExtension(file).ToLowerInvariant();
            if (extension.Equals(".png") || extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".bmp"))
            {
                return FileType.Image;
            }
            else if (extension.Equals(".gif"))
            {
                return FileType.Gif;
            }
            else if (extension.Equals(".wmv") || extension.Equals(".avi") || extension.Equals(".mp4"))
            {
                return FileType.Video;
            }
            return FileType.Unknow;
        }

        private string GetVPXId(string path)
        {
            return path.Split('&')[0].Substring(4);
        }

        public List<object> GetFilters(string path)
        {
            var filters = new List<object>();
            foreach (string definition in path.Split('&'))
            {
                if (definition.StartsWith("dmd=") && int.TryParse(definition.Substring(4), out int dotSize))
                {
                    var filter = new DotFilter();
                    filter._dotSize = dotSize;
                    filters.Add(filter);
                }
                else if (definition.StartsWith("dmd2=") && int.TryParse(definition.Substring(4), out int dotSize2))
                {
                    var filter = new DotFilter();
                    filter._dotSize = dotSize2;
                    filter._offset = 1;
                    filters.Add(filter);
                }
                else if (definition.StartsWith("add"))
                {
                    var filter = new AdditiveFilter();
                    filters.Add(filter);
                }
                else if (definition.StartsWith("sub="))
                {
                    // TODO parse sub rect
                }
            }
            return filters;
        }

        public Asset<T> Load<T>(object id)
        {
            if (_cache.ContainsKey(id))
            {
                Asset<T> asset = (Asset<T>)_cache[id];
                asset._refCount++;
                return asset;
            }
            else
            {
                var asset = new Asset<T>(this, id);
                _cache[id] = asset;
                return asset;
            }
        }

        public bool IsLoaded<T>(object id)
        {
            if (_cache.ContainsKey(id))
            {
                Asset<T> asset = (Asset<T>)_cache[id];
                return asset._loaded;
            }
            else
            {
                return false;
            }
        }

        public T Get<T>(object id)
        {
            if (_cache.ContainsKey(id))
            {
                Asset<T> asset = (Asset<T>)_cache[id];
                if (!asset._loaded)
                {
                    return asset.Value;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Get called on '{0}' which was not loaded yet.", id));
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("Get called on '{0}' which was not previously asked to be loaded.", id));
            }
        }

        public void Unload<T>(object id)
        {
            if (_cache.ContainsKey(id))
            {
                Asset<T> asset = (Asset<T>)_cache[id];
                asset._refCount--;
                if (asset._refCount == 0)
                {
                    _cache.Remove(id);
                    asset.Unload();
                }
                else if (asset._refCount < 0)
                {
                    throw new InvalidOperationException(string.Format("Unload called on '{0}' which was already unloaded.", id));
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unload called on '{0}' which was not previously loaded.", id));
            }
        }

    }
}
