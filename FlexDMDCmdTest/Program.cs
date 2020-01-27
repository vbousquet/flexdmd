using FlexDMD;

namespace FlexDMDTest
{
    class FlexDMDTest
    {
        static void Main(string[] args)
        {
            var dmd = new FlexDMD.FlexDMD();
            // dmd.DmdWidth = 512;
            // dmd.DmdHeight = 128;
            dmd.Show = true;
            var udmd = dmd.NewUltraDMD();
            // udmd.DisplayScene00("Diablo.UltraDMD/act1.wmv", " ", 15, "FREE PLAY", 15, 14, 5000, 14);
            // dmd.DisplayScene00("Diablo.UltraDMD/act2.wmv", "SCENE", 0, "2", 0, 0, 1000, 0);
            // dmd.DisplayScene00("Diablo.UltraDMD/black.jpg", "SCENE", 0, "2", 0, 0, 1000, 0);
            // dmd.DisplayScene00("Metal Slug.UltraDMD/Explosion.gif", "SCENE", 0, "2", 0, 0, 1000, 0);
            // dmd.DisplayScene00("KISS.UltraDMD/scene16CROP.gif", "SCENE", 0, "2", 0, 0, 1000, 0);
            // udmd.DisplayScoreboard(4, 0, 1000, 2000, 3000, 4000, "LEFT", "RIGHT");
            // dmd.DisplayScene00("Metal Slug.UltraDMD/IntroMS.wmv", "SCENE", 0, "2", 0, 0, 5000, 0);
            // dmd.DisplayScene00Ex("Diablo.UltraDMD/act1.wmv", "SCENE", 5, 15, "Test n 2", 5, 15, 14, 5000, 14);
            udmd.DisplayScene01("", "Diablo.UltraDMD/black.jpg&add", "Flex DMD Test", 5, 15, 0, 5000, 0);
            // udmd.DisplayScene00Ex("FlexDMD.Resources.dmds.williams.gif", "", 5, 15, "", 0, 0, 14, 10000, 14);
            // dmd.ScrollingCredits("Diablo.UltraDMD/black.jpg", "Line 1|Line 2|Line 3|Line 4|Line 5|Line 6", 15, 0, 5000, 0);
            // dmd.DisplayScene00("Diablo.UltraDMD/black.jpg", "SCENE", 15, "", 15, 6, 2000, 8);
            System.Threading.Thread.Sleep(5000);
            dmd.Show = false;
        }
    }

}
