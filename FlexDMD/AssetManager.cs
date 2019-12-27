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
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace FlexDMD
{
    public class AnimatedImageDef
	{
		public List<string> _images;
		public int Fps { get; set; } = 25;
		public bool Loop { get; set; } = true;
		
		public AnimatedImageDef(string images, int fps, bool loop)
		{
			_images = new List<string>();
            foreach (string image in images.Split(','))
                _images.Add(image.Trim());
			Fps = fps;
			Loop = loop;
		}

        public override bool Equals(object obj)
        {
            return obj is AnimatedImageDef def &&
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
					// FIXME stream should closed on unload (but kept open for the lifetime of the Bitmap)
                    Bitmap image = new Bitmap(_assets.OpenStream((string)_id));
                    if (!Array.Exists(image.FrameDimensionsList, e => e == FrameDimension.Time.Guid))
                    {
                        // Only convert for still image; animate ones are converted when played
                        Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                        BitmapData data = image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        GraphicUtils.BGRtoRGB(data.Scan0, data.Stride, image.Width, image.Height);
                        image.UnlockBits(data);
                    }
                    _value = (T)Convert.ChangeType(image, typeof(T));
                    _loaded = true;
                }
                else if (typeof(T) == typeof(Actors.Font) && _id.GetType() == typeof(FontDef))
                {
                    var fontDef = (FontDef)_id;
                    log.Info("New font added to asset manager: {0}", fontDef);
                    var font = new Actors.Font(_assets, fontDef, fontDef.FillBrightness, fontDef.OutlineBrightness);
                    _value = (T)Convert.ChangeType(font, typeof(T));
                    _loaded = true;
                }
                else if (typeof(T) == typeof(AnimatedImage) && _id.GetType() == typeof(AnimatedImageDef))
                {
					AnimatedImageDef def = (AnimatedImageDef) _id;
					List<Bitmap> images = new List<Bitmap>();
					foreach (string filename in def._images)
					{
						var bmp = _assets.Load<Bitmap>(filename).Load();
						images.Add(bmp);
					}
					AnimatedImage actor = new AnimatedImage(images, def.Fps, def.Loop);
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
				_value = default(T);
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
			if (siblingPath != null)
			{
				if (siblingPath.StartsWith("FlexDMD.Resources.")) 
					path = "FlexDMD.Resources." + path;
				else if (siblingPath.StartsWith("VPX.")) 
					path = "VPX." + path;
				else
					path = System.IO.Path.Combine(siblingPath, "..", path);
			}
			if (path.StartsWith("FlexDMD.Resources."))
			{
                return Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			}
			else if (path.StartsWith("VPX."))
			{
				if (_vpxFile == null && TableFile != null && File.Exists(System.IO.Path.Combine(BasePath, TableFile)))
				{
					_vpxFile = new VPXFile(System.IO.Path.Combine(BasePath, TableFile));
				}
				if (_vpxFile != null)
				{
					return _vpxFile.OpenStream(path.Substring(4));
				}
				return null;
			}
			else
			{
                var fullPath = System.IO.Path.Combine(BasePath, path);
				return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
			}
		}
		
		public bool FileExists(string path, string siblingPath = null)
		{
			if (siblingPath != null)
			{
				if (siblingPath.StartsWith("FlexDMD.Resources.")) 
					path = "FlexDMD.Resources." + path;
				else if (siblingPath.StartsWith("VPX.")) 
					path = "VPX." + path;
				else
					path = System.IO.Path.Combine(siblingPath, "..", path);
			}
			if (path.StartsWith("FlexDMD.Resources."))
			{
                return Assembly.GetExecutingAssembly().GetManifestResourceInfo(path) != null;
			}
			else if (path.StartsWith("VPX."))
			{
				if (_vpxFile == null && TableFile != null && File.Exists(System.IO.Path.Combine(BasePath, TableFile)))
				{
					_vpxFile = new VPXFile(System.IO.Path.Combine(BasePath, TableFile));
				}
				if (_vpxFile != null)
				{
					return _vpxFile.Contains(path.Substring(4));
				}
				return false;
			}
			else
			{
                var fullPath = System.IO.Path.Combine(BasePath, path);
				return File.Exists(fullPath);
			}
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
