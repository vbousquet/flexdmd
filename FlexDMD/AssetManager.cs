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
    public class AssetManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<String, object> _cache = new Dictionary<String, object>();
        private string _basePath = "./";

        public class Asset<T>
        {
            private readonly AssetManager _assets;
            private readonly string _id;
            private readonly object _params;
            private T _value;
            public bool _loaded;
            public int _refCount;

            public Asset(AssetManager assets, string id, object loadParams)
            {
                _id = id;
                _assets = assets;
                _params = loadParams;
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
                    if (typeof(T) == typeof(Bitmap))
                    {
                        log.Info("New bitmap added to asset manager: {0}", _id);
                        var fullPath = System.IO.Path.Combine(_assets._basePath, _id);
                        Bitmap image = new Bitmap(fullPath);
                        Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                        BitmapData data = image.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        GraphicUtils.BGRtoRGB(data.Scan0, data.Stride);
                        image.UnlockBits(data);
                        _value = (T)Convert.ChangeType(image, typeof(T));
                        _loaded = true;
                    }
                    else if (typeof(T) == typeof(Actors.Font))
                    {
                        /*foreach (string s in assembly.GetManifestResourceNames())
                        {
                            log.Info("Ressource {0}", s);
                        }*/
                        var fields = _id.Split(':');
                        var fillBrightness = int.Parse(fields[1]) / 15f;
                        var outlineBrightness = int.Parse(fields[2]) / 15f;
                        var assembly = Assembly.GetExecutingAssembly();
                        var bmFont = new BitmapFont();
                        using (Stream stream = assembly.GetManifestResourceStream(fields[0]))
                        {
                            bmFont.LoadText(stream);
                        }
                        log.Info("New font added to asset manager: {0} [fill brightness: {1}, outline brightness: {2}]", fields[0], fillBrightness, outlineBrightness);
                        var font = new Actors.Font(bmFont, fillBrightness, outlineBrightness);
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

        public AssetManager()
        {
        }

        public void SetBasePath(string basePath)
        {
            _basePath = basePath;
        }

        public Asset<T> Load<T>(string id, object loadParams = null)
        {
            if (_cache.ContainsKey(id))
            {
                Asset<T> asset = (Asset<T>)_cache[id];
                asset._refCount++;
                return asset;
            }
            else
            {
                var asset = new Asset<T>(this, id, loadParams);
                _cache[id] = asset;
                return asset;
            }
        }

        public bool IsLoaded<T>(string id)
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

        public T Get<T>(string id)
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

        public void Unload<T>(string id)
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
