using UnityEngine;
using SimpleVR.Utils;

namespace SimpleVR
{
	[CreateAssetMenu(fileName = "HandVRConstants", menuName = "SimpleVR/Config/HandVRConstants")]
	[System.Serializable]
	public class HandVRConstants : ScriptableSingleton<HandVRConstants>
	{
		[Header("Back")]
		public DataVR dataVR;
		[Space()]
		[Header("General")]
		public LayerMask interactableElements = 0;  //Interactable elements that can be detected by HandVR.
		public LayerMask blockingElements = 0;  //Elements that visually blocked HandVR, so player can't see it.
		public int characterControllerLayer = 9;  //CharacterVR's CharacterController layer.
		public int hoverDetectionMaxColliders = 5;   //Max colliders that HandVR InteractableDetection tries to detect.
		public float hoverDetectionRadius = 0.15f;   //Interactable and Grabbable objects within this radius can be interacted and attached.
		public bool hoverDetectionCapsule = false;  //Use a Capsule instead a Sphere to hover detect Interactable objects, that way player can get distant objects.
		public float hoverDetectionCapsuleLength = 1.5f;    //Length of the Capsule used to hover detect Interactable objects (only works if handHoverDetectionCapsule is true)
		public float minimumHandVRCollisionSqrVelocity = 8.0f; //Minimum Squared Velocity to do things on Collisions with HandVR
		public float minimumHandVRCollisionSqrAngularVelocity = 50.0f; //Minimum Squared Angular Velocity to do things on Collisions with HandVR

		[Header("Haptics")]
		public ActionHaptics grabHaptics;
		public ActionHaptics interactableHoverHaptics;
	}
}
