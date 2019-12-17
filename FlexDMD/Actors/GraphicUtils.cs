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
using System;

namespace FlexDMD.Actors
{
    class GraphicUtils
    {
        public static void BGRtoRGB(IntPtr scanLine0, int pitch, int width, int height)
        {
            unsafe
            {
                int offset = pitch - 3 * width;
                byte* ptr = ((byte*)scanLine0.ToPointer());
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte r = *ptr;
                        *ptr = *(ptr + 2);
                        *(ptr + 2) = r;
                        ptr += 3;
                    }
                    ptr += offset;
                }
            }
        }

    }
}
