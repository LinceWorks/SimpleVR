using UnityEngine;
using SimpleVR.Utils;

namespace SimpleVR
{
	[CreateAssetMenu(fileName = "GrabbableConstants", menuName = "SimpleVR/Config/GrabbableConstants")]
	[System.Serializable]
	public class GrabbableConstants : ScriptableSingleton<GrabbableConstants>
	{
		[Header("Back")]
		public DataVR dataVR;
		[Space()]
		[Header("General")]
		public float attachTime = 0.15f;	//time to tween position and rotation of attached grabbable
		//public float intentionalSqrVelocityEstimate = 3f;   //Minimum Sqr velocity (PhysicsTracker) that is considered intentional (can be used to check if player is attacking)
		//public int minimumCollisionSqrForce = 8000; //Minimum Sqr force ((collision.impulse / Time.fixedDeltaTime).sqrMagnitude) to do things with a collision
		//public float minimumHandVRCollisionSqrForce = 1.5f; //Minimum Sqr force ((collision.impulse / Time.fixedDeltaTime).sqrMagnitude) to do things with a collision with empty HandVR
		public bool colliderEnabledWhenStored = false;	//config if let stored (attached to Slot) Grabbables to collide with things (that can't cause move other grabbables around without purpose)
	}
}
