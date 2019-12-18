using FlexDMD;
using NLog;
using System;
using System.Runtime.InteropServices;

namespace FlexDMDTest
{
    [ComImport, Guid("766e10d3-dfe3-4e1b-ac99-c4d2be16e91f")]
    public class DMDObject
    {
    }

    class FlexDMDTest
    {
        static void Main(string[] args)
        {
            Logger log = LogManager.GetCurrentClassLogger();
            IDMDObject dmd = (IDMDObject)(new DMDObject());
            // dmd.DmdWidth = 256;
            // dmd.DmdHeight = 64;
            dmd.Init();
            // dmd.DisplayScene00("Diablo.UltraDMD/act1.wmv", " ", 15, "FREE PLAY", 15, 14, 5000, 14);
            // dmd.DisplayScene00("Diablo.UltraDMD/act2.wmv", "SCENE", 0, "2", 0, 0, 1000, 0);
            // dmd.DisplayScene00("Diablo.UltraDMD/black.jpg", "SCENE", 0, "2", 0, 0, 1000, 0);
            // dmd.DisplayScene00("Metal Slug.UltraDMD/Explosion.gif", "SCENE", 0, "2", 0, 0, 1000, 0);
            // dmd.DisplayScene00("KISS.UltraDMD/scene16CROP.gif", "SCENE", 0, "2", 0, 0, 1000, 0);
            // dmd.DisplayScoreboard(4, 0, 1000, 2000, 3000, 4000, "LEFT", "RIGHT");
            // dmd.DisplayScene00("Metal Slug.UltraDMD/IntroMS.wmv", "SCENE", 0, "2", 0, 0, 5000, 0);
            dmd.DisplayScene00("Metal Slug.UltraDMD/Mision1Start.wmv", "SCENE", 0, "2", 0, 0, 5000, 0);
            // dmd.DisplayScene00Ex("Diablo.UltraDMD/act1.wmv", "SCENE", 5, 15, "Test n 2", 5, 15, 14, 5000, 14);
            System.Threading.Thread.Sleep(5000);
            dmd.Uninit();
        }
    }

}
