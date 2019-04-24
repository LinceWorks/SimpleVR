using UnityEngine;

namespace SimpleVR
{
	[CreateAssetMenu(fileName = "ActionHaptics", menuName = "SimpleVR/Config/ActionHaptics")]
	public class ActionHaptics : ScriptableObject
	{
		public float durationSeconds;
		public float frequency;
		public float amplitude;
	}
}
