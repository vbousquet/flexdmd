using Cyotek.Drawing.BitmapFont;
using FlexDMD.Properties;
using NLog;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;

namespace FlexDMD.Actors
{
    class Font
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private System.Drawing.Bitmap[] _textures;
        public readonly BitmapFont _font;

        public static Font LoadFromRessource(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            /*foreach (string s in assembly.GetManifestResourceNames())
            {
                log.Info("Ressource {0}", s);
            }*/
            var font = new BitmapFont();
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                font.LoadText(stream);
            }
            return new Font(font);
        }

        public Font(BitmapFont font)
        {
            _font = font;
            _textures = new System.Drawing.Bitmap[font.Pages.Length];
            var assembly = Assembly.GetExecutingAssembly();
            for (int i = 0; i < font.Pages.Length; i++)
            {
                _textures[i] = new Bitmap(assembly.GetManifestResourceStream("FlexDMD.Resources." + font.Pages[i].FileName));
                // _textures[i] = new System.Drawing.Bitmap(font.Pages[i].FileName);
            }
        }

        public void DrawCharacter(Graphics g, Character character, float x, float y)
        {
            g.DrawImage(_textures[character.TexturePage], new RectangleF((int)x, (int)y, character.Bounds.Width, character.Bounds.Height), character.Bounds, GraphicsUnit.Pixel);
        }

        public void DrawText(Graphics graphics, float x, float y, string text)
        {
            char previousCharacter = ' ';
            foreach (char character in text)
            {
                switch (character)
                {
                    case '\n':
                        x = 0;
                        y += _font.LineHeight;
                        break;
                    default:
                        try
                        {
                            Character data = _font[character];
                            int kerning = _font.GetKerning(previousCharacter, character);
                            this.DrawCharacter(graphics, data, x + data.Offset.X + kerning, y + data.Offset.Y);
                            x += data.XAdvance + kerning;
                        }
                        catch (Exception e)
                        {
                            log.Error(e, "Failed to render char '{0}'", character);
                        }
                        break;
                }
                previousCharacter = character;
            }
        }
    }
}
