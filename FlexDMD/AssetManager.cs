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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Linq;
using Cyotek.Drawing.BitmapFont;

namespace FlexDMD
{

    public enum AssetSrcType
    {
        File, FlexResource, VPXResource
    }

    public enum AssetType
    {
        Unknow, Image, Gif, Video, BMFont
    }

    public class AssetSrc
    {
        public string Id { get; set; } = "";
        public string IdWithoutOptions { get => Id.Split('&')[0]; }
        public string Path { get; set; } = "";
        public AssetType AssetType { get; set; } = AssetType.Unknow;
        public AssetSrcType SrcType { get; set; } = AssetSrcType.File;

        // Still images can have filters applied to the source bitmap
        public List<IBitmapFilter> BitmapFilters { get; } = new List<IBitmapFilter>();

        // Bitmap Fonts can have a tint and a border applied
        public Color FontTint { get; set; } = Color.White;
        public Color FontBorderTint { get; set; } = Color.White;
        public int FontBorderSize { get; set; } = 0;
    }

    public class CachedBitmap
    {
        public Stream Stream { get; set; } = null;
        public Bitmap Bitmap { get; set; } = null;
        public CachedBitmap(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }
        public CachedBitmap(Stream stream)
        {
            Stream = stream;
            Bitmap = new Bitmap(stream);
        }
    }

    /// <summary>
    /// Manages assets used by the actors (asset caching and file resolving).
    /// 
    /// Data lifecycle is the following:
    /// - When they are on stage (InStage property), Actors request the assets they need to the asset manager.
    /// - Assets returned by the manager are valid until ClearAll is called.
    /// - FlexDMD keeps the cache live for the duration of the Run state. When Run is False, all actors are 
    ///   set as out of stage (no more stage to be on), and ClearAll is then called.
    /// </summary>
    public class AssetManager : IDisposable
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private VPXFile _vpxFile = null;
        private readonly Dictionary<string, CachedBitmap> _cachedBitmaps = new Dictionary<string, CachedBitmap>();
        private readonly Dictionary<string, Font> _cachedFonts = new Dictionary<string, Font>();

        public string BasePath { get; set; } = "./";
        public string TableFile { get; set; } = null;

        public AssetManager()
        {
        }

        public void Dispose()
        {
            ClearAll();
        }

        public void ClearAll()
        {
            foreach (KeyValuePair<string, CachedBitmap> entry in _cachedBitmaps)
            {
                entry.Value.Bitmap.Dispose();
                // If loaded from a stream, it must be kept open for the lifetime of the bitmap
                entry.Value.Stream?.Dispose();
            }
            foreach (KeyValuePair<string, Font> entry in _cachedFonts)
            {
                entry.Value.Dispose();
            }
            _cachedBitmaps.Clear();
            _cachedFonts.Clear();
            _vpxFile?.Dispose();
            _vpxFile = null;
        }

        /// <summary>
        /// Resolve a source string into an asset descriptor.
        ///
        /// Source string are made of a filename eventually followed by parameters. A base source can be provided, 
        /// in which case the source is resolved against it. This can be usefull to resolve dependencies for example
        /// a bitmap font is defined by a text file and texture as sibling files. The textures will be resolved
        /// against the base path of the text file.
        ///
        /// The filename can be:
        /// - a filename to be resolved on the filesystem. eg "video.mp4"
        /// - a resource from FlexDMD itself: "FlexDMD.Resources." followed by the name of the FlexDMD resource.. eg "FlexDMD.Resources.bm_army-12.fnt"
        /// - a file included inside the VPX file: "VPX." followed by the name of the VPX resource. eg "VPX.DMD"
        ///
        /// The parameters are under the form of a list of "&param=value" or "&flag"
        ///	- For still images, this is a list of filters
        /// - For fonts, this is color & border paramters
        /// </summary>
        public AssetSrc ResolveSrc(string src, AssetSrc baseSrc = null)
        {
            if (src.Contains("|")) throw new Exception("'|' is not allowed inside file names as it is the separator for image sequences");
            var def = new AssetSrc();
            var parts = src.Split('&');
            // Apply the parenting if any
            if (baseSrc != null)
            {
                if (baseSrc.SrcType == AssetSrcType.FlexResource)
                    parts[0] = "FlexDMD.Resources." + parts[0];
                else if (baseSrc.SrcType == AssetSrcType.VPXResource)
                    parts[0] = "VPX." + parts[0];
                else if (baseSrc.SrcType == AssetSrcType.File)
                    parts[0] = Path.Combine(baseSrc.Path, "..", parts[0]);
            }
            def.Id = string.Join("&", parts);
            // Resolve the source path
            var ext = parts[0].Length > 4 ? parts[0].Substring(parts[0].Length - 4).ToLowerInvariant() : "";
            if (parts[0].StartsWith("FlexDMD.Resources."))
            {
                def.SrcType = AssetSrcType.FlexResource;
                def.Path = parts[0];
            }
            else if (parts[0].StartsWith("VPX."))
            {
                def.SrcType = AssetSrcType.VPXResource;
                def.Path = parts[0].Substring(4);
                ext = "";
                if (_vpxFile == null && TableFile != null && File.Exists(Path.Combine(BasePath, TableFile)))
                    _vpxFile = new VPXFile(Path.Combine(BasePath, TableFile));
                if (_vpxFile != null)
                {
                    var file = _vpxFile.GetImportFile(def.Path);
                    if (file != null && file.Length > 4) ext = file.Substring(file.Length - 4).ToLowerInvariant();
                }
            }
            else
            {
                def.SrcType = AssetSrcType.File;
                def.Path = parts[0];
            }
            // Identify the asset type
            if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg") || ext.Equals(".bmp"))
                def.AssetType = AssetType.Image;
            else if (ext.Equals(".wmv") || ext.Equals(".avi") || ext.Equals(".mp4"))
                def.AssetType = AssetType.Video;
            else if (ext.Equals(".gif"))
                def.AssetType = AssetType.Gif;
            else if (ext.Equals(".fnt"))
                def.AssetType = AssetType.BMFont;
            // Parse the asset options
            if (def.AssetType == AssetType.Image)
            { // Still image filters
                foreach (string definition in parts.Skip(1))
                {
                    try
                    {
                        if (definition.StartsWith("dmd=") && int.TryParse(definition.Substring(4), out int dotSize))
                        {
                            var filter = new DotFilter
                            {
                                DotSize = dotSize
                            };
                            def.BitmapFilters.Add(filter);
                        }
                        else if (definition.StartsWith("dmd2=") && int.TryParse(definition.Substring(5), out int dotSize2))
                        {
                            var filter = new DotFilter
                            {
                                DotSize = dotSize2,
                                Offset = 1
                            };
                            def.BitmapFilters.Add(filter);
                        }
                        else if (definition.StartsWith("add"))
                        {
                            var filter = new AdditiveFilter();
                            def.BitmapFilters.Add(filter);
                        }
                        else if (definition.StartsWith("region="))
                        {
                            var filter = new RegionFilter();
                            var rect = definition.Substring(7).Split(',');
                            filter.X = int.Parse(rect[0]);
                            filter.Y = int.Parse(rect[1]);
                            filter.Width = int.Parse(rect[2]);
                            filter.Height = int.Parse(rect[3]);
                            def.BitmapFilters.Add(filter);
                        }
                        else if (definition.StartsWith("pad="))
                        {
                            var filter = new PadFilter();
                            var rect = definition.Substring(4).Split(',');
                            filter.Left = int.Parse(rect[0]);
                            filter.Top = int.Parse(rect[1]);
                            filter.Right = int.Parse(rect[2]);
                            filter.Bottom = int.Parse(rect[3]);
                            def.BitmapFilters.Add(filter);
                        }
                        else
                            log.Error("Unsupported Bitmap parameter in {0}: {1}", src, definition);
                    }
                    catch (Exception e)
                    {
                        log.Error(e, "Failed to create bitmap filter for: ", definition);
                    }
                }
            }
            else if (def.AssetType == AssetType.BMFont)
            { // Font parameters
                foreach (string definition in parts.Skip(1))
                {
                    try
                    {
                        if (definition.StartsWith("tint=") && int.TryParse(definition.Substring(5), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int tint))
                            def.FontTint = Color.FromArgb(tint);
                        else if (definition.StartsWith("border_tint=") && int.TryParse(definition.Substring(12), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int borderTint))
                            def.FontBorderTint = Color.FromArgb(borderTint);
                        else if (definition.StartsWith("border_size=") && int.TryParse(definition.Substring(12), out int borderSize))
                            def.FontBorderSize = borderSize;
                        else
                            log.Error("Unsupported Font parameter in {0}: {1}", src, definition);
                    }
                    catch (Exception e)
                    {
                        log.Error(e, "Failed to read font parameter: ", definition);
                    }
                }
            }
            return def;
        }

        public Stream Open(AssetSrc src)
        {
            switch (src.SrcType)
            {
                case AssetSrcType.File:
                    return new FileStream(src.Path, FileMode.Open, FileAccess.Read);
                case AssetSrcType.FlexResource:
                    return Assembly.GetExecutingAssembly().GetManifestResourceStream(src.Path);
                case AssetSrcType.VPXResource:
                    if (_vpxFile == null && TableFile != null && File.Exists(Path.Combine(BasePath, TableFile)))
                        _vpxFile = new VPXFile(Path.Combine(BasePath, TableFile));
                    if (_vpxFile != null)
                        return _vpxFile.OpenStream(src.Path);
                    return null;
            }
            return null;
        }

        /// <summary>
        /// Load a bitmap (still image or GIF), applying bitmap filters to still images.
        /// </summary>
        public Bitmap GetBitmap(AssetSrc src)
        {
            if (src.AssetType != AssetType.Image && src.AssetType != AssetType.Gif) throw new Exception("Asked to load a bitmap from a resource of type " + src.AssetType);
            if (_cachedBitmaps.ContainsKey(src.Id))
            { // The requested bitmap is cached
                return _cachedBitmaps[src.Id].Bitmap;
            }
            else if (_cachedBitmaps.ContainsKey(src.IdWithoutOptions))
            { // The bitmap from which the requested one is derived is cached
                var cache = new CachedBitmap(_cachedBitmaps[src.IdWithoutOptions].Bitmap);
                _cachedBitmaps[src.Id] = cache;
                log.Info("Bitmap added to cache:{0}", src.Id);
                if (!Array.Exists(cache.Bitmap.FrameDimensionsList, e => e == FrameDimension.Time.Guid))
                { // Apply filters to still bitmaps
                    foreach (IBitmapFilter filter in src.BitmapFilters)
                        cache.Bitmap = filter.Filter(cache.Bitmap);
                }
                return cache.Bitmap;
            }
            else
            { // This is a new bitmap that should be added to the cache
                var cache = new CachedBitmap(Open(src));
                _cachedBitmaps[src.IdWithoutOptions] = cache;
                log.Info("Bitmap added to cache:{0}", src.IdWithoutOptions);
                if (!Array.Exists(cache.Bitmap.FrameDimensionsList, e => e == FrameDimension.Time.Guid))
                { // Apply RGB conversion and filters to still bitmaps
                    GraphicUtils.BGRtoRGB(cache.Bitmap);
                    if (src.BitmapFilters.Count > 0)
                    {
                        cache = new CachedBitmap(cache.Bitmap);
                        _cachedBitmaps[src.Id] = cache;
                        foreach (IBitmapFilter filter in src.BitmapFilters)
                            cache.Bitmap = filter.Filter(cache.Bitmap);
                        log.Info("Bitmap added to cache:{0}", src.Id);
                    }
                }
                return cache.Bitmap;
            }
        }

        /// <summary>
        /// Load a font
        /// </summary>
        public Font GetFont(AssetSrc assetSrc)
        {
            if (assetSrc.AssetType != AssetType.BMFont) throw new Exception("Asked to load a font from a resource of type " + assetSrc.AssetType);
            if (_cachedFonts.ContainsKey(assetSrc.Id))
                return _cachedFonts[assetSrc.Id];
            var font = new Font(this, assetSrc);
            _cachedFonts[assetSrc.Id] = font;
            log.Info("Font added to cache:{0}", assetSrc.Id);
            return font;
        }
    }
}