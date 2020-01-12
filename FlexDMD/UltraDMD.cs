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
using FlexDMD.Actors;
using FlexDMD.Scenes;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace UltraDMD
{
    /// <summary>
    /// Implementation of the UltraDMD API using the FlexDMD rendering engine.
    /// </summary>
    public class UltraDMD : IUltraDMD
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly FlexDMD.FlexDMD _flexDMD;
        private readonly SceneQueue _queue = new SceneQueue();
        private readonly Dictionary<int, object> _preloads = new Dictionary<int, object>();
        private ScoreBoard _scoreBoard;
        private FontDef _scoreFontText, _scoreFontNormal, _scoreFontHighlight;
        private FontDef _twoLinesFontTop, _twoLinesFontBottom;
        private FontDef[] _singleLineFont;
        private bool _visible = true;
        private int _stretchMode = 0;
        private int _nextId = 1;

        public UltraDMD(FlexDMD.FlexDMD flexDMD)
        {
            _flexDMD = flexDMD;
            // UltraDMD uses f4by5 / f5by7 / f6by12
            _scoreFontText = new FontDef("FlexDMD.Resources.udmd-f4by5.fnt", 0.66f);
            _scoreFontNormal = new FontDef("FlexDMD.Resources.udmd-f5by7.fnt", 0.66f);
            _scoreFontHighlight = new FontDef("FlexDMD.Resources.udmd-f6by12.fnt");
            // UltraDMD uses f14by26 or f12by24 or f7by13 to fit in
            _singleLineFont = new FontDef[] {
                new FontDef("FlexDMD.Resources.udmd-f14by26.fnt"),
                new FontDef("FlexDMD.Resources.udmd-f12by24.fnt"),
                new FontDef("FlexDMD.Resources.udmd-f7by13.fnt")
            };
            // UltraDMD uses f5by7 / f6by12 for top / bottom line
            _twoLinesFontTop = new FontDef("FlexDMD.Resources.udmd-f5by7.fnt");
            _twoLinesFontBottom = new FontDef("FlexDMD.Resources.udmd-f6by12.fnt");
            // Core rendering tree
            _scoreBoard = new ScoreBoard(
                _flexDMD.NewFont(_scoreFontNormal.Path, _scoreFontNormal.FillBrightness, _scoreFontNormal.OutlineBrightness),
                _flexDMD.NewFont(_scoreFontHighlight.Path, _scoreFontHighlight.FillBrightness, _scoreFontHighlight.OutlineBrightness),
                _flexDMD.NewFont(_scoreFontText.Path, _scoreFontText.FillBrightness, _scoreFontText.OutlineBrightness)
                );
            _scoreBoard.Visible = false;
            _flexDMD.Stage.AddActor(_scoreBoard);
            _flexDMD.Stage.AddActor(_queue);
        }

        private Actor ResolveImage(string filename, bool useFrame)
        {
            try
            {
                if (Int32.TryParse(filename, out int preloadId) && _preloads.ContainsKey(preloadId))
                {
                    var preload = _preloads[preloadId];
                    if (preload is Video v)
                    {
                        // TODO implement (store preload info, and adapt asset manager, no newinstance logic which is flawed)
                        // return v.newInstance();
                    }
                    else if (preload is AnimatedImage ai)
                    {
                        // TODO implement
                        // return ai.newInstance();
                    }
                }
                else
                {
                    var actor = _flexDMD.ResolveImage(filename);
                    if (actor != null) return actor;
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Exception while resolving image: '{0}'", filename);
            }
            return useFrame ? new Frame() : new Actor();
        }

        public void Init() => _flexDMD.Init();
        public void Uninit() => _flexDMD.Uninit();

        public int GetMajorVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileMajorPart;
        }

        public int GetMinorVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileMinorPart;
        }

        public int GetBuildNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileBuildPart * 10000 + fvi.FilePrivatePart;
        }

        public bool SetVisibleVirtualDMD(bool bVisible)
        {
            log.Info("SetVisibleVirtualDMD({0})", bVisible);
            bool wasVisible = _visible;
            _visible = bVisible;
            if (!wasVisible && _visible)
                _flexDMD.Init();
            else if (wasVisible && !_visible)
                _flexDMD.Uninit();
            return wasVisible;
        }

        public bool SetFlipY(bool flipY)
        {
            log.Error("SetFlipY is not yet supported in FlexDMD");
            return false;
        }

        public bool IsRendering()
        {
            return _queue.IsRendering();
        }

        public void CancelRendering()
        {
            _flexDMD.Post(() => _queue.CancelRendering());
        }

        public void CancelRenderingWithId(string sceneId)
        {
            _flexDMD.Post(() => _queue.CancelRendering(sceneId));
        }

        public void Clear()
        {
            _flexDMD.Post(() =>
            {
                _flexDMD.Graphics.Clear(Color.Black);
                _scoreBoard.Visible = false;
            });
        }

        public void SetProjectFolder(string basePath) => _flexDMD.ProjectFolder = basePath;

		// TODO implement stretch: 0, crop to top: 1, crop to center: 2, crop to bottom: 3
        public void SetVideoStretchMode(int mode) => _stretchMode = mode;

        public int CreateAnimationFromImages(int fps, bool loop, string imagelist)
        {
            var id = _nextId;
            _nextId++;
            // TODO var def = new AnimatedImageDef(imagelist, fps, loop);
            // _preloads[id] = _assets.Load<AnimatedImage>(def).Load();
            return id;
        }

        public int RegisterVideo(int videoStretchMode, bool loop, string videoFilename)
        {
            var id = _nextId;
            _nextId++;
            // TODO var video = new Video(System.IO.Path.Combine(_assets.BasePath, videoFilename), loop, videoStretchMode);
            // _preloads[id] = video;
            return id;
        }

        private Label GetFittedLabel(string text, float fillBrightness, float outlineBrightness)
        {
            foreach (FontDef f in _singleLineFont)
            {
                var font = _flexDMD.NewFont(f.Path, fillBrightness, outlineBrightness);
                var label = new Label(font, text);
                label.SetPosition((_flexDMD.Width - label.Width) / 2, (_flexDMD.Height - label.Height) / 2);
                if ((label.X >= 0 && label.Y >= 0) || f == _singleLineFont[_singleLineFont.Length - 1]) return label;
            }
            return null;
        }

        public void DisplayVersionInfo()
        {
            // No version info in FlexDMD (this is an implementation choice to avoid delaying game startup and displaying again and again the same scene)
            _scoreBoard.Visible = false;
        }

        public void DisplayScene00(string background, string toptext, int topBrightness, string bottomtext, int bottomBrightness, int animateIn, int pauseTime, int animateOut)
        {
            DisplayScene00ExWithId("", false, background, toptext, topBrightness, -15, bottomtext, bottomBrightness, -15, animateIn, pauseTime, animateOut);
        }

        public void DisplayScene00Ex(string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut)
        {
            DisplayScene00ExWithId("", false, background, toptext, topBrightness, topOutlineBrightness, bottomtext, bottomBrightness, bottomOutlineBrightness, animateIn, pauseTime, animateOut);
        }

        public void DisplayScene00ExWithId(string sceneId, bool cancelPrevious, string background, string toptext, int topBrightness, int topOutlineBrightness, string bottomtext, int bottomBrightness, int bottomOutlineBrightness, int animateIn, int pauseTime, int animateOut)
        {
            _flexDMD.Post(() =>
            {
                _scoreBoard.Visible = false;
                if (cancelPrevious && sceneId != null && sceneId.Length > 0)
                {
                    var s = _queue.GetActiveScene();
                    if (s.Id == sceneId) _queue.CancelRendering(sceneId);
                }
                if (toptext != null && toptext.Length > 0 && bottomtext != null && bottomtext.Length > 0)
                {
                    var fontTop = _flexDMD.NewFont(_twoLinesFontTop.Path, topBrightness / 15f, topOutlineBrightness / 15f);
                    var fontBottom = _flexDMD.NewFont(_twoLinesFontBottom.Path, bottomBrightness / 15f, bottomOutlineBrightness / 15f);
                    var scene = new TwoLineScene(ResolveImage(background, true), toptext, fontTop, bottomtext, fontBottom, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, sceneId);
                    _queue.Enqueue(scene);
                }
                else if (toptext != null && toptext.Length > 0)
                {
                    var font = GetFittedLabel(toptext, topBrightness / 15f, topOutlineBrightness / 15f).Font;
                    var scene = new SingleLineScene(ResolveImage(background, true), toptext, font, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, false, sceneId);
                    _queue.Enqueue(scene);
                }
                else if (bottomtext != null && bottomtext.Length > 0)
                {
                    var font = GetFittedLabel(bottomtext, bottomBrightness / 15f, bottomOutlineBrightness / 15f).Font;
                    var scene = new SingleLineScene(ResolveImage(background, true), bottomtext, font, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, false, sceneId);
                    _queue.Enqueue(scene);
                }
                else
                {
                    var scene = new BackgroundScene(ResolveImage(background, true), (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, sceneId);
                    _queue.Enqueue(scene);
                }
            });
        }

        public void ModifyScene00(string id, string toptext, string bottomtext)
        {
            _flexDMD.Post(() =>
            {
                var scene = _queue.GetActiveScene();
                if (scene != null && id != null && id.Length > 0 && scene.Id == id)
                {
                    if (scene is TwoLineScene s2) s2.SetText(toptext, bottomtext);
                    if (scene is SingleLineScene s1) s1.SetText(toptext);
                }
            });
        }

        public void ModifyScene00Ex(string id, string toptext, string bottomtext, int pauseTime)
        {
            _flexDMD.Post(() =>
            {
                var scene = _queue.GetActiveScene();
                if (scene != null && id != null && id.Length > 0 && scene.Id == id)
                {
                    if (scene is TwoLineScene s2) s2.SetText(toptext, bottomtext);
                    if (scene is SingleLineScene s1) s1.SetText(toptext);
                    scene.Pause = scene.Time + pauseTime / 1000f;
                }
            });
        }

        public void DisplayScene01(string sceneId, string background, string text, int textBrightness, int textOutlineBrightness, int animateIn, int pauseTime, int animateOut)
        {
            _flexDMD.Post(() =>
            {
                _scoreBoard.Visible = false;
                var font = _flexDMD.NewFont(_singleLineFont[0].Path, textBrightness / 15f, textOutlineBrightness / 15f);
                var scene = new SingleLineScene(ResolveImage(background, false), text, font, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut, true, sceneId);
                _queue.Enqueue(scene);
            });
        }

        public void SetScoreboardBackgroundImage(string filename, int selectedBrightness, int unselectedBrightness)
        {
            _flexDMD.Post(() =>
            {
                _scoreBoard.SetBackground(ResolveImage(filename, false));
                _scoreBoard.SetFonts(
                    _flexDMD.NewFont(_scoreFontNormal.Path, unselectedBrightness, -1),
                    _flexDMD.NewFont(_scoreFontHighlight.Path, selectedBrightness, -1),
                    _flexDMD.NewFont(_scoreFontText.Path, unselectedBrightness, -1));
            });
        }

        public void DisplayScoreboard(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight)
        {
            _flexDMD.Post(() =>
            {
                // Direct rendering: render only if the scene queue is empty, and no direct rendering has happened (managed by scoreboard visibility)
                _scoreBoard.Visible = true;
                _scoreBoard.SetNPlayers(cPlayers);
                _scoreBoard.SetHighlightedPlayer(highlightedPlayer);
                _scoreBoard.SetScore(score1, score2, score3, score4);
                _scoreBoard._lowerLeft.Text = lowerLeft;
                _scoreBoard._lowerRight.Text = lowerRight;
            });
        }

        // KissDMDv2.vbs use this undocumented function as far as I know. As far as I can tell, this will just change the font.
        // UltraDMD.DisplayScoreboard00 PlayersPlayingGame, 0, Score(1), Score(2), Score(3), Score(4), "credits " & Credits, ""
        public void DisplayScoreboard00(int cPlayers, int highlightedPlayer, int score1, int score2, int score3, int score4, string lowerLeft, string lowerRight)
        {
            // TODO use an UltraDMD matching font
            DisplayScoreboard(cPlayers, highlightedPlayer, score1, score2, score3, score4, lowerLeft, lowerRight);
        }

        public void DisplayText(string text, int textBrightness, int textOutlineBrightness)
        {
            _flexDMD.Post(() =>
            {
                log.Error("DisplayText [untested] '{0}', {1}, {2}", text, textBrightness, textOutlineBrightness);
                _scoreBoard.Visible = false;
                GetFittedLabel(text, textBrightness / 15f, textOutlineBrightness / 15).Draw(_flexDMD.Graphics);
            });
        }

        // TODO I did not find an UltraDMD table using this, and I did not succeed in making it work in UltraDMD. So I'm just guessing it's behavior, font, etc.
        public void ScrollingCredits(string background, string text, int textBrightness, int animateIn, int pauseTime, int animateOut)
        {
            _flexDMD.Post(() =>
            {
                // log.Info("ScrollingCredits '{0}', '{1}', {2}", background, text, textBrightness);
                _scoreBoard.Visible = false;
                string[] lines = text.Split(new char[] { '\n', '|' });
                var font12 = _flexDMD.NewFont(_twoLinesFontBottom.Path, textBrightness / 15f, -1);
                var scene = new ScrollingCreditsScene(ResolveImage(background, false), lines, font12, (AnimationType)animateIn, pauseTime / 1000f, (AnimationType)animateOut);
                _queue.Enqueue(scene);
            });
        }
    }
}
