using UnityEngine;
using UnityEngine.SpatialTracking;


namespace SimpleVR
{
	public class CharacterVRInitialRelocation : MonoBehaviour
	{
		private Transform headTransform;

		private void OnEnable ()
		{
			//Disable CharacterVR to avoid FollowCamera() until relocation is done
			GetComponent<CharacterVR>().enabled = false;
			headTransform = GetComponentInChildren<Camera>().transform;
		}
		
		private void Update ()
		{
			TryRelocation();
		}

		/// <summary>
		/// Detects when tracked camera moves and relocate to the start position and rotation, then it disables itself.
		/// This way, player view starts where CharacterVR is placed and looking at the given direction.
		/// </summary>
		private void TryRelocation()
		{
			//If headTransform is far away (more than characterInititalRelocationDistance) in XZ plane
			if (Mathf.Abs((Vector3.Scale(Vector3.right + Vector3.forward, transform.position - headTransform.position)).sqrMagnitude) > DataVR.Instance.characterInititalRelocationDistance * DataVR.Instance.characterInititalRelocationDistance)
			{
				//Set rotation with the difference in Y axis to head (camera) rotation
				transform.eulerAngles = -Vector3.Scale(Vector3.up, headTransform.localEulerAngles);

				//Gets head (camera) position in local space from the point of view of the CharacterVR transform
				//Inverse it
				//Scales with right and forward vector to avoid Y axis
				//transform the point to world space
				//Set CharacterVR position as the given point
				transform.position = transform.TransformPoint(Vector3.Scale(Vector3.right + Vector3.forward, -transform.InverseTransformPoint(headTransform.position)));

				//Enable CharacterVR and disable this
				GetComponent<CharacterVR>().enabled = true;
				enabled = false;
			}
		}
	}
}
