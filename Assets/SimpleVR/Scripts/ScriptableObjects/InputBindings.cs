using UnityEngine;
using SimpleVR.Utils;
using Valve.VR;

namespace SimpleVR
{
	[CreateAssetMenu(fileName = "InputBindings", menuName = "SimpleVR/Config/InputBindings")]
	[System.Serializable]
	public class InputBindings : ScriptableSingleton<InputBindings>
	{
		[Header("Back")]
		public DataVR dataVR;

		[Header("General")]
		public SteamVR_Settings settings;

		[Header("Action Sets")]
		public SteamVR_ActionSet[] currentActionSets;
		public SteamVR_ActionSet ingameActionSet;
		public SteamVR_ActionSet menuActionSet;

		//[Header("Bindings - Deprecated")]
		//public SteamVR_Action_Boolean grab;
	}
}
