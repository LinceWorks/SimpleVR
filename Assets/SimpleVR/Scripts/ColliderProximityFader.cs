using UnityEngine;

namespace SimpleVR
{
	/// <summary>
	/// Fade out gradually when near to colliders.
	/// It's not only a proximity sensor, it checks if user is looking at the collider or to another way and consider this for a proper fade grade.
	/// </summary>
	public class ColliderProximityFader : MonoBehaviour
	{
		private Vector2 fadingValues = Vector2.zero;
		private LayerMask layerMask;

		private void OnEnable()
		{
			fadingValues = DataVR.Instance.fadingProximityValues;
			layerMask = DataVR.Instance.fadingLayerMask;
		}

		//check shortersqrdistance every frame to set correct alpha on the CameraFade
		private void Update()
		{
			Collider[] nearColliders = Physics.OverlapSphere(transform.position, fadingValues.y, layerMask, QueryTriggerInteraction.Ignore);

			float shorterSqrDistance = float.MaxValue;

			for (int i = 0; i < nearColliders.Length; i++)
			{
				Vector3 camToClosestPoint = Physics.ClosestPoint(transform.position, nearColliders[i], nearColliders[i].transform.position, nearColliders[i].transform.rotation) - transform.position;
				// 1 /Clamped Dot multiplies abs sqrmagnitude to consider user view direction
				float sqrDistance = 1 / Mathf.Clamp(Vector3.Dot(camToClosestPoint.normalized, transform.forward),0,1) * Mathf.Abs(camToClosestPoint.sqrMagnitude);
				if (sqrDistance < shorterSqrDistance) shorterSqrDistance = sqrDistance;
			}

			CameraFading.CameraFade.Alpha = 1 - RemapNumberClamped(shorterSqrDistance, fadingValues[0] * fadingValues[0], fadingValues[1] * fadingValues[1], 0, 1);
		}

		//Functions from Valve.VR.InteractionSystem.Util
		//-------------------------------------------------
		public float RemapNumber(float num, float low1, float high1, float low2, float high2)
		{
			return low2 + (num - low1) * (high2 - low2) / (high1 - low1);
		}
		
		public float RemapNumberClamped(float num, float low1, float high1, float low2, float high2)
		{
			return Mathf.Clamp(RemapNumber(num, low1, high1, low2, high2), Mathf.Min(low2, high2), Mathf.Max(low2, high2));
		}
	}
}
