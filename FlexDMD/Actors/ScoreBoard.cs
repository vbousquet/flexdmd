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

namespace FlexDMD.Actors
{
    class ScoreBoard : Group
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private Actor _background = null;
        public int _selectedBrightness = 15;
        public int _unselectedBrightness = 15;
        private int _highlightedPlayer = 0;
        private Label[] _scores = new Label[4];
        private readonly Font _scoreFont, _highlightFont, _textFont;
        public Label _lowerLeft, _lowerRight;

        public ScoreBoard(Font scoreFont, Font highlightFont, Font textFont)
        {
            _scoreFont = scoreFont;
            _highlightFont = highlightFont;
            _textFont = textFont;
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
                _scores[i]._visible = i < nPlayers;
            }
        }

        public void SetHighlightedPlayer(int player)
        {
            _highlightedPlayer = player;
            for (int i = 0; i < 4; i++)
            {
                if (i == player - 1)
                {
                    _scores[i].Font = _highlightFont;
                }
                else
                {
                    _scores[i].Font = _scoreFont;
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
            float yText = _height - _textFont._font.BaseHeight - 1;
            float yLine2 = (1 + yText) * 0.5f;
            _scores[0].SetPosition(1, 1);
            _scores[1].SetPosition(_width - _scores[1]._width - 1, 1);
            _scores[2].SetPosition(1, yLine2);
            _scores[3].SetPosition(_width - _scores[3]._width - 1, yLine2);
            _lowerLeft.SetPosition(1, yText);
            _lowerRight.SetPosition(_width - _lowerRight._width - 1, yText);
        }
    }
}
