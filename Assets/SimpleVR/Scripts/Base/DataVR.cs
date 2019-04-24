using UnityEngine;
using SimpleVR.Utils;

namespace SimpleVR
{
	[CreateAssetMenu(fileName = "DataVR", menuName = "SimpleVR/Config/DataVR")]
	[System.Serializable]
	public class DataVR : ScriptableSingleton<DataVR>
	{
		[Header("CharacterVR")]
		public int characterGravity = -25;  //personal gravity for CharacterVR
		public float headHeightAdd = 0.2f; //height above eyes to top of the head in meters
		public float physicsProximityRadius = 2.5f;	//physics proximity radius to improve rigidbody detection mode of near objects
		public Vector2 fadingProximityValues = new Vector2(0.25f, 0.4f); //closest proximity to fade to black and far proximity to fade to clear
		public LayerMask fadingLayerMask = 0; //layerMask used for fading
		public float crouchMaxHeight = 1.4f; //max CharacterVR Height to consider is crouched
		public float standingSpeed = 2f; //CharacterVR movement speed when standing
		public float crouchingSpeed = 1f; //CharacterVR movement speed when crouching
		public float rotatingSpeed = 0.2f; //CharacterVR rotating speed on CharacterVR.Rotate(float yawAngle) method

		[Header("CharacterVRInitialRelocation")]
		public float characterInititalRelocationDistance = 0.05f;    //minimum distance to relocate player by CharacterVRInitialRelocation

		[Header("TeleporterVR")]
		public float teleportDistance = 10f;	//Max distance to teleport.

		[Header("Other")]
		public HandVRConstants hand;
		public GrabbableConstants grabbable;
		public InputBindings input;
	}
}