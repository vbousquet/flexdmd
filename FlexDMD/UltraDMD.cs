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
using FlexDMD;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

/// <summary>
/// An object that fakes the UltraDMD object, delegating all the calls to a FlexDMD instance.
/// </summary>
namespace UltraDMD
{
    [Guid("E1612654-304A-4E07-A236-EB64D6D4F511"), ComVisible(true), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IDMDObjectEvents))]
    public class DMDObject : IDMDObject
    {
        private FlexDMD.DMDObject _dmd = new FlexDMD.DMDObject();

        // FlexDMD methods & properties
        public string GameName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort DmdWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ushort DmdHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int RenderMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color DmdColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object DmdColoredPixels => throw new NotImplementedException();
        public object DmdPixels => throw new NotImplementedException();
        public void SetTableFile(string tableFile) => throw new NotImplementedException();
        public void SetScoreFonts(string textFont, string normalFont, string highlightFont, int selectedBrightness, int unselectedBrightness) => throw new NotImplementedException();
        public void SetTwoLineFonts(string topFont, string bottomFont) => throw new NotImplementedException();
        public void SetSingleLineFonts(string[] fonts) => throw new NotImplementedException();
        public void DisplayJPSScene(string id, string background, string[] top, string[] bottom, int animateIn, int pauseTime, int animateOut) => throw new NotImplementedException();

        // UltraDMD methods & properties
        public void Init() => _dmd.Init();
        public void Uninit() => _dmd.Uninit();
        public int GetMajorVersion() => _dmd.GetMajorVersion();
        public int GetMinorVersion() => _dmd.GetMinorVersion();
        public int GetBuildNumber() => _dmd.GetBuildNumber();
        public bool SetVisibleVirtualDMD(bool bVisible) => _dmd.SetVisibleVirtualDMD(bVisible);
        public bool SetFlipY(bool flipY) => _dmd.SetFlipY(flipY);
        public bool IsRendering() => _dmd.IsRendering();
        public void CancelRendering() => _dmd.CancelRendering();
        public void CancelRenderingWithId(string sceneId) => _dmd.CancelRenderingWithId(sceneId);
        public void Clear() => _dmd.Clear();
        public void SetProjectFolder(string basePath) => _dmd.SetProjectFolder(basePath);
        public void SetVideoStretchMode(int mode) => _dmd.SetVideoStretchMode(mode);
        public int CreateAnimationFromImages(int fps, bool loop, string imagelist) => _dmd.CreateAnimationFromImages(fps, loop, imagelist);
        public int RegisterVideo(int videoStretchMode, bool loop, string videoFilename) => _dmd.RegisterVideo(videoStretchMode, loop, videoFilename);
        public void DisplayVersionInfo() => _dmd.DisplayVersionInfo();
        public void DisplayScene00(string background, string toptext, int topBrightness, string bottomtext, int bottomBrightness, int animateIn, int pauseTime, int animateOut) => _dmd.DisplayScene00(background, toptext, topBrightness, bottomtext, bottomBrightness, animateIn, pauseTime, animateOut);
        public void DisplayScene00Ex(string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut) => _dmd.DisplayScene00Ex(background, toptext, topBrightness, topOutlineBrightness, bottomtext, bottomBrightness, bottomOutlineBrightness, animateIn, pauseTime, animateOut);
        public void DisplayScene00ExWithId(string sceneId, bool cancelPrevious, string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut) => _dmd.DisplayScene00ExWithId(sceneId, cancelPrevious, background, toptext, topBrightness, topOutlineBrightness, bottomtext, bottomBrightness, bottomOutlineBrightness, animateIn, pauseTime, animateOut);
        public void ModifyScene00(string id, string toptext, string bottomtext) => _dmd.ModifyScene00(id, toptext, bottomtext);
        public void ModifyScene00Ex(string id, string toptext, string bottomtext, int pauseTime) => _dmd.ModifyScene00Ex(id, toptext, bottomtext, pauseTime);
        public void DisplayScene01(string sceneId, string background, string text, int textBrightness, int textOutlineBrightness, int animateIn, int pauseTime, int animateOut) => _dmd.DisplayScene01(sceneId, background, text, textBrightness, textOutlineBrightness, animateIn, pauseTime, animateOut);
        public void SetScoreboardBackgroundImage(string filename, int selectedBrightness, int unselectedBrightness) => SetScoreboardBackgroundImage(filename, selectedBrightness, unselectedBrightness);
        public void DisplayScoreboard(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight) => _dmd.DisplayScoreboard(cPlayers, highlightedPlayer, score1, score2, score3, score4, lowerLeft, lowerRight);
        public void DisplayScoreboard00(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight) => _dmd.DisplayScoreboard00(cPlayers, highlightedPlayer, score1, score2, score3, score4, lowerLeft, lowerRight);
        public void DisplayText(string text, int textBrightness, int textOutlineBrightness) => _dmd.DisplayText(text, textBrightness, textOutlineBrightness);
        public void ScrollingCredits(string background, string text, int textBrightness, int animateIn, int pauseTime, int animateOut) => _dmd.ScrollingCredits(background, text, textBrightness, animateIn, pauseTime, animateOut);
    }
}
