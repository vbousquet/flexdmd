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
using System.Drawing;

namespace FlexDMD
{
    public class Image : Actor, IImageActor
    {
        public Bitmap Bitmap {get; set; } = null;
		public Scaling Scaling {get; set; } = Scaling.Stretch;
		public Alignment Alignment {get; set; } = Alignment.Center;

		public override float PrefWidth { get => Bitmap.Width; }
		
		public override float PrefHeight { get => Bitmap.Height; }

        public Image(Bitmap image)
        {
            Bitmap = image;
			Pack();
        }

        public override void Draw(Graphics graphics)
        {
            if (Visible && Bitmap != null) 
			{
				Layout.Scale(Scaling, PrefWidth, PrefHeight, Width, Height, out float w, out float h);
                Layout.Align(Alignment, w, h, Width, Height, out float x, out float y);
				graphics.DrawImage(Bitmap, (int)(X + x), (int)(Y + y), (int)w, (int)h);
			}
        }
    }
}

