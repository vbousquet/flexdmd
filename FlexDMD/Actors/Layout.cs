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
using System.Runtime.InteropServices;

// This code is adapted from LibGdx's implementation, from Nathan Sweet
// https://github.com/libgdx/libgdx/blob/master/gdx/src/com/badlogic/gdx/utils/Scaling.java
namespace FlexDMD
{
	public enum Scaling
	{
		// Scales the source to fit the target while keeping the same aspect ratio. This may cause the source to be smaller than the
		// target in one direction.
		Fit,

		// Scales the source to fill the target while keeping the same aspect ratio. This may cause the source to be larger than the
		// target in one direction.
		Fill,

		// Scales the source to fill the target in the x direction while keeping the same aspect ratio. This may cause the source to be
		// smaller or larger than the target in the y direction.
		FillX,

		// Scales the source to fill the target in the y direction while keeping the same aspect ratio. This may cause the source to be
		// smaller or larger than the target in the x direction.
		FillY,

		// Scales the source to fill the target. This may cause the source to not keep the same aspect ratio.
		Stretch,

		// Scales the source to fill the target in the x direction, without changing the y direction. This may cause the source to not
		// keep the same aspect ratio.
		StretchX,

		// Scales the source to fill the target in the y direction, without changing the x direction. This may cause the source to not
		// keep the same aspect ratio.
		StretchY,

		// The source is not scaled.
		None
	}
	
	public enum Alignment
	{
		TopLeft, Top, TopRight,
		Left, Center, Right,
		BottomLeft, Bottom, BottomRight
	}
	
    public class Layout
    {
		public static void Scale(Scaling mode, float sourceWidth, float sourceHeight, float targetWidth, float targetHeight, out float width, out float height)
		{
			switch (mode) {
				case Scaling.Fit: 
				{
					float targetRatio = targetHeight / targetWidth;
					float sourceRatio = sourceHeight / sourceWidth;
					float scale = targetRatio > sourceRatio ? targetWidth / sourceWidth : targetHeight / sourceHeight;
					width = sourceWidth * scale;
					height = sourceHeight * scale;
					break;
				}
				case Scaling.Fill: 
				{
					float targetRatio = targetHeight / targetWidth;
					float sourceRatio = sourceHeight / sourceWidth;
					float scale = targetRatio < sourceRatio ? targetWidth / sourceWidth : targetHeight / sourceHeight;
					width = sourceWidth * scale;
					height = sourceHeight * scale;
					break;
				}
				case Scaling.FillX: 
				{
					float scale = targetWidth / sourceWidth;
					width = sourceWidth * scale;
					height = sourceHeight * scale;
					break;
				}
				case Scaling.FillY: 
				{
					float scale = targetHeight / sourceHeight;
					width = sourceWidth * scale;
					height = sourceHeight * scale;
					break;
				}
				case Scaling.Stretch:
					width = targetWidth;
					height = targetHeight;
					break;
				case Scaling.StretchX:
					width = targetWidth;
					height = sourceHeight;
					break;
				case Scaling.StretchY:
					width = sourceWidth;
					height = targetHeight;
					break;
				case Scaling.None:
					width = sourceWidth;
					height = sourceHeight;
					break;
				default:
					width = 0;
					height = 0;
					break;
				}
		}
		
		public static void Align(Alignment mode, float width, float height, float containerWidth, float containerHeight, out float x, out float y)
		{
			switch (mode) {
				case Alignment.TopLeft:
				case Alignment.Left:
				case Alignment.BottomLeft:
					x = 0f;
					break;
				case Alignment.Top:
				case Alignment.Center:
				case Alignment.Bottom:
					x = (containerWidth - width) * 0.5f;
					break;
				case Alignment.TopRight:
				case Alignment.Right:
				case Alignment.BottomRight:
					x = containerWidth - width;
					break;
				default:
					x = 0;
					break;
			}
			switch (mode) {
				case Alignment.TopLeft:
				case Alignment.Top:
				case Alignment.TopRight:
					y = 0f;
					break;
				case Alignment.Left:
				case Alignment.Center:
				case Alignment.Right:
					y = (containerHeight - height) * 0.5f;
					break;
				case Alignment.BottomLeft:
				case Alignment.Bottom:
				case Alignment.BottomRight:
					y = containerHeight - height;
					break;
				default:
					y = 0;
					break;
			}
		}
    }
}
