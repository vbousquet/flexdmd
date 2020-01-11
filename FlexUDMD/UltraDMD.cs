using FlexDMD;
using System;
using System.Runtime.InteropServices;

namespace FlexUDMD
{
    /// <summary>
    /// An object that fakes the UltraDMD object, delegating all the calls to a FlexDMD instance.
    /// </summary>
    [Guid("E1612654-304A-4E07-A236-EB64D6D4F511"), ComVisible(true), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IDMDObjectEvents))]
    public class DMDObject : IUltraDMD
    {
        private IUltraDMD _udmd = new FlexDMD.FlexDMD().NewUltraDMD();
        public void Init() => _udmd.Init();
        public void Uninit() => _udmd.Uninit();
        public int GetMajorVersion() => _udmd.GetMajorVersion();
        public int GetMinorVersion() => _udmd.GetMinorVersion();
        public int GetBuildNumber() => _udmd.GetBuildNumber();
        public bool SetVisibleVirtualDMD(bool bVisible) => _udmd.SetVisibleVirtualDMD(bVisible);
        public bool SetFlipY(bool flipY) => _udmd.SetFlipY(flipY);
        public bool IsRendering() => _udmd.IsRendering();
        public void CancelRendering() => _udmd.CancelRendering();
        public void CancelRenderingWithId(string sceneId) => _udmd.CancelRenderingWithId(sceneId);
        public void Clear() => _udmd.Clear();
        public void SetProjectFolder(string basePath) => _udmd.SetProjectFolder(basePath);
        public void SetVideoStretchMode(int mode) => _udmd.SetVideoStretchMode(mode);
        public int CreateAnimationFromImages(int fps, bool loop, string imagelist) => _udmd.CreateAnimationFromImages(fps, loop, imagelist);
        public int RegisterVideo(int videoStretchMode, bool loop, string videoFilename) => _udmd.RegisterVideo(videoStretchMode, loop, videoFilename);
        public void DisplayVersionInfo() => _udmd.DisplayVersionInfo();
        public void DisplayScene00(string background, string toptext, int topBrightness, string bottomtext, int bottomBrightness, int animateIn, int pauseTime, int animateOut) => _udmd.DisplayScene00(background, toptext, topBrightness, bottomtext, bottomBrightness, animateIn, pauseTime, animateOut);
        public void DisplayScene00Ex(string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut) => _udmd.DisplayScene00Ex(background, toptext, topBrightness, topOutlineBrightness, bottomtext, bottomBrightness, bottomOutlineBrightness, animateIn, pauseTime, animateOut);
        public void DisplayScene00ExWithId(string sceneId, bool cancelPrevious, string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut) => _udmd.DisplayScene00ExWithId(sceneId, cancelPrevious, background, toptext, topBrightness, topOutlineBrightness, bottomtext, bottomBrightness, bottomOutlineBrightness, animateIn, pauseTime, animateOut);
        public void ModifyScene00(string id, string toptext, string bottomtext) => _udmd.ModifyScene00(id, toptext, bottomtext);
        public void ModifyScene00Ex(string id, string toptext, string bottomtext, int pauseTime) => _udmd.ModifyScene00Ex(id, toptext, bottomtext, pauseTime);
        public void DisplayScene01(string sceneId, string background, string text, int textBrightness, int textOutlineBrightness, int animateIn, int pauseTime, int animateOut) => _udmd.DisplayScene01(sceneId, background, text, textBrightness, textOutlineBrightness, animateIn, pauseTime, animateOut);
        public void SetScoreboardBackgroundImage(string filename, int selectedBrightness, int unselectedBrightness) => _udmd.SetScoreboardBackgroundImage(filename, selectedBrightness, unselectedBrightness);
        public void DisplayScoreboard(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight) => _udmd.DisplayScoreboard(cPlayers, highlightedPlayer, score1, score2, score3, score4, lowerLeft, lowerRight);
        public void DisplayScoreboard00(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight) => _udmd.DisplayScoreboard00(cPlayers, highlightedPlayer, score1, score2, score3, score4, lowerLeft, lowerRight);
        public void DisplayText(string text, int textBrightness, int textOutlineBrightness) => _udmd.DisplayText(text, textBrightness, textOutlineBrightness);
        public void ScrollingCredits(string background, string text, int textBrightness, int animateIn, int pauseTime, int animateOut) => _udmd.ScrollingCredits(background, text, textBrightness, animateIn, pauseTime, animateOut);
    }
}
