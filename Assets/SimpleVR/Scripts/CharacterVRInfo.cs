using UnityEngine;

namespace SimpleVR
{
	public static class CharacterVRInfo
	{
		public static int ID(int index) { return RuntimeVR.CharacterVRs[index].ID; }
		public static Vector3 FeetPosition(int index) { return RuntimeVR.CharacterVRs[index].FeetPosition; }
		public static Transform HeadTransform(int index) { return RuntimeVR.CharacterVRs[index].HeadTransform; }
	}

	public static class CharacterVR1Info
	{
		private const int index = 0;

		public static int ID { get { return RuntimeVR.CharacterVRs[index].ID; } }
		public static Vector3 FeetPosition { get { return RuntimeVR.CharacterVRs[index].FeetPosition; } }
		public static Transform Head { get { return RuntimeVR.CharacterVRs[index].HeadTransform; } }
	}

	public static class CharacterVR2Info
	{
		private const int index = 1;

		public static int ID { get { return RuntimeVR.CharacterVRs[index].ID; } }
		public static Vector3 FeetPosition { get { return RuntimeVR.CharacterVRs[index].FeetPosition; } }
		public static Transform Head { get { return RuntimeVR.CharacterVRs[index].HeadTransform; } }
	}
}