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
using Cyotek.Drawing.BitmapFont;
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
	public enum PathType 
	{
		Resource = 0,
		FilePath = 1
	}
	
	public class FontDef
    {
		public float FillBrightness { get; set; } = 1f;
		public float OutlineBrightness { get; set; } = -1f;
		public PathType PathType { get; set; } = PathType.FilePath;
		public string Path { get; set; } = "";
		
		public FontDef(PathType pathType, string path, float fillBrightness, float outlineBrightness) 
		{
			Path = path;
			FillBrightness = fillBrightness;
			OutlineBrightness = outlineBrightness;
			PathType = pathType;
		}

		public FontDef(PathType pathType, string path, float brightness) : this(pathType, path, brightness, -1f)
        {
        }
		
		public FontDef(PathType pathType, string path) : this(pathType, path, 1f, -1f)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is FontDef def &&
                   FillBrightness == def.FillBrightness &&
                   OutlineBrightness == def.OutlineBrightness &&
                   PathType == def.PathType &&
                   Path == def.Path;
        }

        public override int GetHashCode()
        {
            var hashCode = -1876634251;
            hashCode = hashCode * -1521134295 + FillBrightness.GetHashCode();
            hashCode = hashCode * -1521134295 + OutlineBrightness.GetHashCode();
            hashCode = hashCode * -1521134295 + PathType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
            return hashCode;
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
					var fullPath = System.IO.Path.Combine(_assets.BasePath, (string) _id);
					Bitmap image = new Bitmap(fullPath);
					Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
					BitmapData data = image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					GraphicUtils.BGRtoRGB(data.Scan0, data.Stride);
					image.UnlockBits(data);
					_value = (T)Convert.ChangeType(image, typeof(T));
					_loaded = true;
				}
				else if (typeof(T) == typeof(Actors.Font) && _id.GetType() == typeof(FontDef))
				{
					var fontDef = (FontDef)_id;
					log.Info("New font added to asset manager: {0}", fontDef);
					var font = new Actors.Font(fontDef, fontDef.FillBrightness, fontDef.OutlineBrightness);
					_value = (T)Convert.ChangeType(font, typeof(T));
					_loaded = true;
				}
				else
				{
					throw new InvalidOperationException(string.Format("Unsupported asset type {0}", typeof(T)));
				}
			}
			return _value;
		}
	}

    public class AssetManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<object, object> _cache = new Dictionary<object, object>();
        public string BasePath { get; set; } = "./";

        public AssetManager()
        {
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
                    if (asset is IDisposable d) d.Dispose();
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
