using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

/// <summary>
///  Texture Packer used to prepare frame sequences, derived from https://github.com/mfascia/TexturePacker
///  All credits goes to https://github.com/mfascia for creating this concise texture packing code
///  
/// Modification made:
/// - support png, jpg and jpeg files
/// - detect duplicates
/// - remove unused code (standalone app and output)
/// </summary>
namespace FlexDMDUI
{
    /// <summary>
    /// Represents a Texture in an atlas
    /// </summary>
    public class TextureInfo
    {
        /// <summary>
        /// Path of the source textures on disk that lead to this image (we detect duplicates)
        /// </summary>
        public List<string> Sources = new List<string>();

        /// <summary>
        /// Width in Pixels before whitespace removal
        /// </summary>
        public int OriginalWidth;

        /// <summary>
        /// Height in Pixels before whitespace removal
        /// </summary>
        public int OriginalHeight;

        /// <summary>
        /// Width in Pixels after whitespace removal
        /// </summary>
        public int PackedWidth;

        /// <summary>
        /// Height in Pixels after whitespace removal
        /// </summary>
        public int PackedHeight;

        /// <summary>
        /// Offset in Pixels of left whitespace removal
        /// </summary>
        public int PackedX;

        /// <summary>
        /// Offset in Pixels of top whitespace removal
        /// </summary>
        public int PackedY;

        /// <summary>
        /// Hash of actual bitmap (for duplicate detection)
        /// </summary>
        public byte[] Hash;
    }

    /// <summary>
    /// Indicates in which direction to split an unused area when it gets used
    /// </summary>
    public enum SplitType
    {
        /// <summary>
        /// Split Horizontally (textures are stacked up)
        /// </summary>
        Horizontal,

        /// <summary>
        /// Split verticaly (textures are side by side)
        /// </summary>
        Vertical,
    }

    /// <summary>
    /// Different types of heuristics in how to use the available space
    /// </summary>
    public enum BestFitHeuristic
    {
        /// <summary>
        /// 
        /// </summary>
        Area,

        /// <summary>
        /// 
        /// </summary>
        MaxOneAxis,
    }

    /// <summary>
    /// A node in the Atlas structure
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Bounds of this node in the atlas
        /// </summary>
        public Rectangle Bounds;

        /// <summary>
        /// Texture this node represents
        /// </summary>
        public TextureInfo Texture;

        /// <summary>
        /// If this is an empty node, indicates how to split it when it will  be used
        /// </summary>
        public SplitType SplitType;
    }

    /// <summary>
    /// The texture atlas
    /// </summary>
    public class Atlas
    {
        /// <summary>
        /// Width in pixels
        /// </summary>
        public int Width;

        /// <summary>
        /// Height in Pixel
        /// </summary>
        public int Height;

        /// <summary>
        /// List of the nodes in the Atlas. This will represent all the textures that are packed into it and all the remaining free space
        /// </summary>
        public List<Node> Nodes;
    }

    /// <summary>
    /// Objects that performs the packing task. Takes a list of textures as input and generates a set of atlas textures/definition pairs
    /// </summary>
    public class Packer
    {
        /// <summary>
        /// List of all the textures that need to be packed
        /// </summary>
        public List<TextureInfo> SourceTextures;

        /// <summary>
        /// Stream that recieves all the info logged
        /// </summary>
        public StringWriter Log;

        /// <summary>
        /// Stream that recieves all the error info
        /// </summary>
        public StringWriter Error;

        /// <summary>
        /// Number of pixels that separate textures in the atlas
        /// </summary>
        public int Padding;

        /// <summary>
        /// Size of the atlas in pixels. Represents one axis, as atlases are square
        /// </summary>
        public int AtlasSize;

        /// <summary>
        /// Toggle for debug mode, resulting in debug atlasses to check the packing algorithm
        /// </summary>
        public bool DebugMode;

        /// <summary>
        /// Which heuristic to use when doing the fit
        /// </summary>
        public BestFitHeuristic FitHeuristic;

        /// <summary>
        /// List of all the output atlases
        /// </summary>
        public List<Atlas> Atlasses;

        public Packer()
        {
            SourceTextures = new List<TextureInfo>();
            Log = new StringWriter();
            Error = new StringWriter();
        }

        public void Process(string _SourceDir, int _AtlasSize, int _Padding, bool _DebugMode)
        {
            Padding = _Padding;
            AtlasSize = _AtlasSize;
            DebugMode = _DebugMode;

            //1: scan for all the textures we need to pack
            ScanForTextures(_SourceDir, "*.png");
            ScanForTextures(_SourceDir, "*.jpg");
            ScanForTextures(_SourceDir, "*.jpeg");

            List<TextureInfo> textures = new List<TextureInfo>();
            textures = SourceTextures.ToList();

            //2: generate as many atlasses as needed (with the latest one as small as possible)
            Atlasses = new List<Atlas>();
            while (textures.Count > 0)
            {
                Atlas atlas = new Atlas();
                atlas.Width = _AtlasSize;
                atlas.Height = _AtlasSize;

                List<TextureInfo> leftovers = LayoutAtlas(textures, atlas);

                if (leftovers.Count == 0)
                {
                    // we reached the last atlas. Check if this last atlas could have been twice smaller
                    while (leftovers.Count == 0)
                    {
                        atlas.Width /= 2;
                        atlas.Height /= 2;
                        leftovers = LayoutAtlas(textures, atlas);
                    }
                    // we need to go 1 step larger as we found the first size that is to small
                    atlas.Width *= 2;
                    atlas.Height *= 2;
                    leftovers = LayoutAtlas(textures, atlas);
                }

                Atlasses.Add(atlas);

                textures = leftovers;
            }
        }



        private void ScanForTextures(string _Path, string _Wildcard)
        {
            DirectoryInfo di = new DirectoryInfo(_Path);
            FileInfo[] files = di.GetFiles(_Wildcard, SearchOption.AllDirectories);

            foreach (FileInfo fi in files)
            {
                Image img = Image.FromFile(fi.FullName);
                if (img != null)
                {
                    if (img.Width <= AtlasSize && img.Height <= AtlasSize)
                    {
                        Bitmap bmp = new Bitmap(img);

                        // Remove whitespace
                        int alphaThreshold = 32;
                        int leftWS = 0, rightWS = 0, topWS = 0, bottomWS = 0;
                        for (int x = 0;  x < img.Width; x++)
                        {
                            leftWS = x;
                            for (int y = 0; y < img.Height; y++)
                            {
                                var col = bmp.GetPixel(x, y);
                                if (col.A > alphaThreshold)
                                {
                                    x = img.Width;
                                    y = img.Height;
                                }
                            }
                        }
                        for (int x = img.Width - 1; x >= 0; x--)
                        {
                            rightWS = (img.Width - 1) - x;
                            for (int y = 0; y < img.Height; y++)
                            {
                                var col = bmp.GetPixel(x, y);
                                if (col.A > alphaThreshold)
                                {
                                    x = -1;
                                    y = img.Height;
                                }
                            }
                        }
                        for (int y = 0; y < img.Height; y++)
                        {
                            topWS = y;
                            for (int x = 0; x < img.Width; x++)
                            {
                                var col = bmp.GetPixel(x, y);
                                if (col.A > alphaThreshold)
                                {
                                    x = img.Width;
                                    y = img.Height;
                                }
                            }
                        }
                        for (int y = img.Height - 1; y >= 0; y--)
                        {
                            bottomWS = (img.Height - 1) - y;
                            for (int x = 0; x < img.Width; x++)
                            {
                                var col = bmp.GetPixel(x, y);
                                if (col.A > alphaThreshold)
                                {
                                    x = img.Width;
                                    y = -1;
                                }
                            }
                        }
                        var packedWidth = img.Width - leftWS - rightWS;
                        var packedHeight = img.Height - topWS - bottomWS;
                        if (packedWidth <= 0 || packedHeight <= 0)
                        {
                            packedWidth = packedHeight = 1;
                            leftWS = topWS = 0;
                            rightWS = img.Width - 1;
                            bottomWS = img.Height - 1;
                        }
                        System.Console.WriteLine("Packed: " + leftWS + ", " + topWS + ", " + rightWS + ", " + bottomWS + " / " + img.Width + ", " + img.Height);

                        // Compute image hash for duplicate detection
                        BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                        int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                        byte[] rawImageData = new byte[bytes];
                        Marshal.Copy(bmpData.Scan0, rawImageData, 0, bytes);
                        bmp.UnlockBits(bmpData);
                        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                        byte[] hash = md5.ComputeHash(rawImageData);

                        TextureInfo ti = null;
                        foreach (TextureInfo texi in SourceTextures)
                        {
                            if (texi.OriginalWidth == img.Width && texi.OriginalHeight == img.Height && texi.Hash.SequenceEqual(hash))
                            {
                                System.Console.WriteLine(fi.FullName + " is a duplicate of " + texi.Sources[0]);
                                Log.WriteLine(fi.FullName + " is a duplicate of " + texi.Sources[0]);
                                ti = texi;
                                break;
                            }
                        }
                        if (ti == null)
                        {
                            ti = new TextureInfo
                            {
                                OriginalWidth = img.Width,
                                OriginalHeight = img.Height,
                                PackedX = leftWS,
                                PackedY = topWS,
                                PackedWidth = packedWidth,
                                PackedHeight = packedHeight,
                                Hash = hash
                            };
                            SourceTextures.Add(ti);
                            Log.WriteLine("Added " + fi.FullName);
                        }
                        ti.Sources.Add(fi.FullName);
                    }
                    else
                    {
                        Error.WriteLine(fi.FullName + " is too large to fix in the atlas. Skipping!");
                    }
                }
            }
        }

        private void HorizontalSplit(Node _ToSplit, int _Width, int _Height, List<Node> _List)
        {
            Node n1 = new Node();
            n1.Bounds.X = _ToSplit.Bounds.X + _Width + Padding;
            n1.Bounds.Y = _ToSplit.Bounds.Y;
            n1.Bounds.Width = _ToSplit.Bounds.Width - _Width - Padding;
            n1.Bounds.Height = _Height;
            n1.SplitType = SplitType.Vertical;

            Node n2 = new Node();
            n2.Bounds.X = _ToSplit.Bounds.X;
            n2.Bounds.Y = _ToSplit.Bounds.Y + _Height + Padding;
            n2.Bounds.Width = _ToSplit.Bounds.Width;
            n2.Bounds.Height = _ToSplit.Bounds.Height - _Height - Padding;
            n2.SplitType = SplitType.Horizontal;

            if (n1.Bounds.Width > 0 && n1.Bounds.Height > 0)
                _List.Add(n1);
            if (n2.Bounds.Width > 0 && n2.Bounds.Height > 0)
                _List.Add(n2);
        }

        private void VerticalSplit(Node _ToSplit, int _Width, int _Height, List<Node> _List)
        {
            Node n1 = new Node();
            n1.Bounds.X = _ToSplit.Bounds.X + _Width + Padding;
            n1.Bounds.Y = _ToSplit.Bounds.Y;
            n1.Bounds.Width = _ToSplit.Bounds.Width - _Width - Padding;
            n1.Bounds.Height = _ToSplit.Bounds.Height;
            n1.SplitType = SplitType.Vertical;

            Node n2 = new Node();
            n2.Bounds.X = _ToSplit.Bounds.X;
            n2.Bounds.Y = _ToSplit.Bounds.Y + _Height + Padding;
            n2.Bounds.Width = _Width;
            n2.Bounds.Height = _ToSplit.Bounds.Height - _Height - Padding;
            n2.SplitType = SplitType.Horizontal;

            if (n1.Bounds.Width > 0 && n1.Bounds.Height > 0)
                _List.Add(n1);
            if (n2.Bounds.Width > 0 && n2.Bounds.Height > 0)
                _List.Add(n2);
        }

        private TextureInfo FindBestFitForNode(Node _Node, List<TextureInfo> _Textures)
        {
            TextureInfo bestFit = null;

            float nodeArea = _Node.Bounds.Width * _Node.Bounds.Height;
            float maxCriteria = 0.0f;

            foreach (TextureInfo ti in _Textures)
            {
                switch (FitHeuristic)
                {
                    // Max of Width and Height ratios
                    case BestFitHeuristic.MaxOneAxis:
                        if (ti.PackedWidth <= _Node.Bounds.Width && ti.PackedHeight <= _Node.Bounds.Height)
                        {
                            float wRatio = (float)ti.PackedWidth / (float)_Node.Bounds.Width;
                            float hRatio = (float)ti.PackedHeight / (float)_Node.Bounds.Height;
                            float ratio = wRatio > hRatio ? wRatio : hRatio;
                            if (ratio > maxCriteria)
                            {
                                maxCriteria = ratio;
                                bestFit = ti;
                            }
                        }
                        break;

                    // Maximize Area coverage
                    case BestFitHeuristic.Area:

                        if (ti.PackedWidth <= _Node.Bounds.Width && ti.PackedHeight <= _Node.Bounds.Height)
                        {
                            float textureArea = ti.PackedWidth * ti.PackedHeight;
                            float coverage = textureArea / nodeArea;
                            if (coverage > maxCriteria)
                            {
                                maxCriteria = coverage;
                                bestFit = ti;
                            }
                        }
                        break;
                }
            }

            return bestFit;
        }

        private List<TextureInfo> LayoutAtlas(List<TextureInfo> _Textures, Atlas _Atlas)
        {
            List<Node> freeList = new List<Node>();
            List<TextureInfo> textures = new List<TextureInfo>();

            _Atlas.Nodes = new List<Node>();

            textures = _Textures.ToList();

            Node root = new Node();
            root.Bounds.Size = new Size(_Atlas.Width, _Atlas.Height);
            root.SplitType = SplitType.Horizontal;

            freeList.Add(root);

            while (freeList.Count > 0 && textures.Count > 0)
            {
                Node node = freeList[0];
                freeList.RemoveAt(0);

                TextureInfo bestFit = FindBestFitForNode(node, textures);
                if (bestFit != null)
                {
                    if (node.SplitType == SplitType.Horizontal)
                    {
                        HorizontalSplit(node, bestFit.PackedWidth, bestFit.PackedHeight, freeList);
                    }
                    else
                    {
                        VerticalSplit(node, bestFit.PackedWidth, bestFit.PackedHeight, freeList);
                    }

                    node.Texture = bestFit;
                    node.Bounds.Width = bestFit.PackedWidth;
                    node.Bounds.Height = bestFit.PackedHeight;

                    textures.Remove(bestFit);
                }

                _Atlas.Nodes.Add(node);
            }

            return textures;
        }

        public Image CreateAtlasImage(Atlas _Atlas)
        {
            Image img = new Bitmap(_Atlas.Width, _Atlas.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(img);

            if (DebugMode)
            {
                g.FillRectangle(Brushes.Green, new Rectangle(0, 0, _Atlas.Width, _Atlas.Height));
            }

            foreach (Node n in _Atlas.Nodes)
            {
                if (n.Texture != null)
                {
                    // var sourceImg = Image.FromFile(n.Texture.Sources[0]);
                    var sourceImg = new Bitmap(n.Texture.Sources[0]);
                    sourceImg = sourceImg.Clone(new Rectangle(n.Texture.PackedX, n.Texture.PackedY, n.Texture.PackedWidth, n.Texture.PackedHeight), sourceImg.PixelFormat);
                    g.DrawImage(sourceImg, n.Bounds);

                    if (DebugMode)
                    {
                        string label = Path.GetFileNameWithoutExtension(n.Texture.Sources[0]);
                        SizeF labelBox = g.MeasureString(label, SystemFonts.MenuFont, new SizeF(n.Bounds.Size));
                        RectangleF rectBounds = new Rectangle(n.Bounds.Location, new Size((int)labelBox.Width, (int)labelBox.Height));
                        g.FillRectangle(Brushes.Black, rectBounds);
                        g.DrawString(label, SystemFonts.MenuFont, Brushes.White, rectBounds);
                    }
                }
                else
                {
                    g.FillRectangle(Brushes.DarkMagenta, n.Bounds);

                    if (DebugMode)
                    {
                        string label = n.Bounds.Width.ToString() + "x" + n.Bounds.Height.ToString();
                        SizeF labelBox = g.MeasureString(label, SystemFonts.MenuFont, new SizeF(n.Bounds.Size));
                        RectangleF rectBounds = new Rectangle(n.Bounds.Location, new Size((int)labelBox.Width, (int)labelBox.Height));
                        g.FillRectangle(Brushes.Black, rectBounds);
                        g.DrawString(label, SystemFonts.MenuFont, Brushes.White, rectBounds);
                    }
                }
            }

            return img;
        }

    }

}