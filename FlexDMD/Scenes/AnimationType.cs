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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexDMD.Scenes
{
	public enum AnimationType {
		FadeIn = 0,
		FadeOut = 1,
		ZoomIn = 2,
		ZoomOut = 3,
		ScrollOffLeft = 4,
		ScrollOffRight = 5,
		ScrollOnLeft = 6,
		ScrollOnRight = 7,
		ScrollOffUp = 8,
		ScrollOffDown = 9,
		ScrollOnUp = 10,
		ScrollOnDown = 11,
		FillFadeIn = 12,
		FillFadeOut = 13,
		None = 14
	}
}
