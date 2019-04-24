using UnityEngine;

namespace SimpleVR.Utils
{
	public static class PhysicsExt
	{
		public static bool VisibleFrom(this Transform me, Vector3 fromPoint, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return me.VisibleSpecificPointFrom(fromPoint, me.position, layerMask, queryTriggerInteraction);
		}

		public static bool VisibleFrom(this Transform me, Vector3 fromPoint, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, GameObject[] filtered)
		{
			return me.VisibleSpecificPointFrom(fromPoint, me.position, layerMask, queryTriggerInteraction, filtered);
		}

		public static bool VisibleSpecificPointFrom(this Transform me, Vector3 fromPoint, Vector3 endPoint, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			RaycastHit hitInfo;

			Physics.Linecast(fromPoint, endPoint, out hitInfo, layerMask, queryTriggerInteraction);
			//if (hitInfo.collider != null) Debug.Log("hitInfo.collider: " + hitInfo.collider, hitInfo.collider.gameObject);
			if (hitInfo.collider == null || me.gameObject == hitInfo.collider.gameObject) return true;
			return false;
		}

		public static bool VisibleSpecificPointFrom(this Transform me, Vector3 endPoint, Vector3 fromPoint, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction, GameObject[] filtered)
		{
			//Debug.Log("_______________________");
			RaycastHit[] hitInfo = Physics.RaycastAll(fromPoint, (endPoint - fromPoint).normalized , (endPoint - fromPoint).magnitude, layerMask, queryTriggerInteraction);

			bool valid = (hitInfo == null || hitInfo.Length == 0) ? true : false;
			for (int i = 0; i < hitInfo.Length; i++)
			{
				//Debug.Log("false");
				valid = false;

				if (me.gameObject == hitInfo[i].collider.gameObject)
				{
					valid = true;
					continue;
				}

				for (int j = 0; j < filtered.Length; j++)
				{
					//Debug.Log("filt: " + filtered[j]);
					if (filtered[j] == hitInfo[i].collider.gameObject)
					{
						valid = true;
						break;
					}
				}

				if (!valid) return valid;
			}

			return valid;
		}

		///// <summary>
		///// Basically, a too slowly collision is not considered intentional
		///// </summary>
		///// <param name="collision"></param>
		///// <returns></returns>
		//public static bool CollisionIsIntentional(Collision collision)
		//{
		//	return (collision.impulse / Time.fixedDeltaTime).sqrMagnitude > DataVR.Instance.grabbable.minimumCollisionSqrForce;
		//}

		///// <summary>
		///// Basically, a too slowly collision is not considered intentional
		///// </summary>
		///// <param name="collision"></param>
		///// <returns></returns>
		//public static bool CollisionWithHandIsIntentional(Collision collision)
		//{
		//	return (collision.impulse / Time.fixedDeltaTime).sqrMagnitude > DataVR.Instance.grabbable.minimumHandVRCollisionSqrForce;
		//}

		/// <summary>
		/// Ignore collision between colliders
		/// </summary>
		/// <param name="collider"></param>
		/// <param name="colliders"></param>
		/// <param name="ignore"></param>
		public static void IgnoreCollisions(Collider collider, Collider[] colliders, bool ignore = true)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				Physics.IgnoreCollision(collider, colliders[i], ignore);
			}
		}

		/// <summary>
		/// Ignore collision between colliders
		/// </summary>
		/// <param name="collider"></param>
		/// <param name="colliders"></param>
		/// <param name="ignore"></param>
		public static void IgnoreCollisions(Collider[] colliders1, Collider[] colliders2, bool ignore = true)
		{
			for (int i = 0; i < colliders1.Length; i++)
			{
				for (int j = 0; j < colliders2.Length; j++)
				{
					Physics.IgnoreCollision(colliders1[i], colliders2[j], ignore);
				}
			}
		}
	}
}
