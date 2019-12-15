using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexDMD.Actors
{
    class GraphicUtils
    {
        public static void BGRtoRGB(IntPtr scanLine0, int pitch)
        {
            unsafe
            {
                byte* ptr = ((byte*)scanLine0.ToPointer());
                for (int y = 0; y < 32; y++)
                {
                    for (int x = 0; x < 128; x++)
                    {
                        byte r = *ptr;
                        *ptr = *(ptr + 2);
                        *(ptr + 2) = r;
                        ptr = ptr + 3;
                    }
                    ptr = ptr + pitch - 3 * 128;
                }
            }
        }

    }
}
