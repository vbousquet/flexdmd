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
using NLog;
using System.Drawing;

namespace FlexDMD
{
    class ScoreBoard : Group
    {
        private readonly Label[] _scores = new Label[4];
        private Actor _background = null;
        private int _highlightedPlayer = 0;
        public Label _lowerLeft, _lowerRight;

        public Font ScoreFont { get; private set; }
        public Font HighlightFont { get; private set; }
        public Font TextFont { get; private set; }

        public ScoreBoard(Font scoreFont, Font highlightFont, Font textFont)
        {
            ScoreFont = scoreFont;
            HighlightFont = highlightFont;
            TextFont = textFont;
            _lowerLeft = new Label(textFont, "");
            _lowerRight = new Label(textFont, "");
            AddActor(_lowerLeft);
            AddActor(_lowerRight);
            for (int i = 0; i < 4; i++)
            {
                _scores[i] = new Label(scoreFont, "0");
                AddActor(_scores[i]);
            }
        }

        public void SetBackground(Actor background)
        {
            if (_background != null)
            {
                RemoveActor(_background);
                if (_background is IDisposable e) e.Dispose();
            }
            _background = background;
            if (_background != null)
            {
                AddActorAt(_background, 0);
            }
        }

        public void SetNPlayers(int nPlayers)
        {
            for (int i = 0; i < 4; i++)
            {
                _scores[i].Visible = i < nPlayers;
            }
        }

        public void SetFonts(Font scoreFont, Font highlightFont, Font textFont)
        {
            ScoreFont = scoreFont;
            HighlightFont = highlightFont;
            TextFont = textFont;
            SetHighlightedPlayer(_highlightedPlayer);
            _lowerLeft.Font = textFont;
            _lowerRight.Font = textFont;
        }

        public void SetHighlightedPlayer(int player)
        {
            _highlightedPlayer = player;
            for (int i = 0; i < 4; i++)
            {
                if (i == player - 1)
                {
                    _scores[i].Font = HighlightFont;
                }
                else
                {
                    _scores[i].Font = ScoreFont;
                }
            }
        }

        public void SetScore(int score1, int score2, int score3, int score4)
        {
            _scores[0].Text = score1.ToString("#,##0");
            _scores[1].Text = score2.ToString("#,##0");
            _scores[2].Text = score3.ToString("#,##0");
            _scores[3].Text = score4.ToString("#,##0");
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            float yText = Height - TextFont.BitmapFont.BaseHeight - 1;
            // float yLine2 = 1 + HighlightFont.BitmapFont.BaseHeight + (Height - 2 - TextFont.BitmapFont.BaseHeight - 2 * HighlightFont.BitmapFont.BaseHeight) / 2;
            float yLine2 = (Height - TextFont.BitmapFont.BaseHeight) / 2f;
            float dec = (HighlightFont.BitmapFont.BaseHeight - ScoreFont.BitmapFont.BaseHeight) / 2f;
            // float yLine2 = (1 + yText) * 0.5f;
            _scores[0].SetPosition(1, 1 + (_highlightedPlayer == 1 ? 0 : dec));
            _scores[1].SetPosition(Width - _scores[1].Width - 1, 1 + (_highlightedPlayer == 2 ? 0 : dec));
            _scores[2].SetPosition(1, yLine2 + (_highlightedPlayer == 3 ? 0 : dec));
            _scores[3].SetPosition(Width - _scores[3].Width - 1, yLine2 + (_highlightedPlayer == 4 ? 0 : dec));
            _lowerLeft.SetPosition(1, yText);
            _lowerRight.SetPosition(Width - _lowerRight.Width - 1, yText);
        }

        public override void Draw(Graphics graphics)
        {
            graphics.Clear(Color.Black);
            _background?.SetSize(Width, Height);
            base.Draw(graphics);
        }
    }
}
