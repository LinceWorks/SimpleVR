using UnityEngine;

namespace SimpleVR
{
	public class PhysicsProximityAdjust : MonoBehaviour
	{
		#region References
		private CharacterVR characterVR;
		#endregion

		private void OnEnable()
		{
			GetComponent<SphereCollider>().radius = DataVR.Instance.physicsProximityRadius;
			characterVR = GetComponentInParent<CharacterVR>();
		}

		private void OnTriggerEnter(Collider other)
		{
			Interactable i = other.GetComponentInParent<Interactable>();
			if (i) characterVR.AddNearInteractables(i);

			Rigidbody rb = other.GetComponentInParent<Rigidbody>();
			Grabbable g = null;
			if(i) g = i.GetComponentInParent<Grabbable>();

			if (rb && (!g || !g.Attached)) rb.collisionDetectionMode = /*CollisionDetectionMode.Continuous*/CollisionDetectionMode.ContinuousSpeculative;   //invalidates Rigidbody.Sleep(); in Grabbable Awake?
		}

		private void OnTriggerExit(Collider other)
		{
			Interactable i = other.GetComponentInParent<Interactable>();
			if (i) characterVR.RemoveNearInteractables(i);

			Rigidbody rb = other.GetComponentInParent<Rigidbody>();
			if (rb) rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
		}
	}
}
