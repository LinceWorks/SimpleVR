using System.Linq;
using UnityEngine;

namespace SimpleVR
{
	public static class RuntimeVR
	{
		private static CharacterVR[] characterVRs = null;

		public static CharacterVR[] CharacterVRs
		{
			get
			{
				if (characterVRs == null)
				{
					characterVRs = RefreshCharacterVRs();
				}
				return characterVRs;
			}
		}

		public static CharacterVR[] RefreshCharacterVRs()
		{
			//Using Linq here is acceptable because it will be done few times, one time surely.
			//More info about Unity Optimization:
			//http://www.gamasutra.com/blogs/AmirFassihi/20130828/199134/0__60_fps_in_14_days_What_we_learned_trying_to_optimize_our_game_using_Unity3D.php
			//Specifically the line about Linq:
			//Not using LINQ commands since they tended to allocate intermediate buffers, food for the GC.
			return Object.FindObjectsOfType<CharacterVR>().OrderBy(c => c.ID).ToArray();
		}
	}
}
