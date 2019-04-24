using UnityEngine;
using SimpleVR.Utils;

namespace SimpleVR
{
	public class GrabbableGameplayCollision : MonoBehaviour
	{
		public LayerMask mask;

		private void OnCollisionEnter(Collision collision)
		{
			if (mask.Contains(collision.gameObject.layer))
			{
				GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				g.transform.position = collision.contacts[0].point;
				g.transform.localScale = Vector3.one * 0.1f;
				g.GetComponent<SphereCollider>().enabled = false;
			}
		}
	}
}
