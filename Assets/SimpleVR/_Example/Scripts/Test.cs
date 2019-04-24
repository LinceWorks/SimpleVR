using UnityEngine;
using SimpleVR;

public class Test : MonoBehaviour
{
	public Transform teleportTransformReference;


	[ContextMenu("DebugInfo")]
	public void DebugInfo()
	{
		Debug.Log(CharacterVR1Info.ID);
		Debug.Log(CharacterVR1Info.FeetPosition);
	}

	private void Update()
	{
		if(UnityEngine.Input.GetKey(KeyCode.Space))
		{
			CharacterVR c = FindObjectOfType<CharacterVR>();
			c.MoveTo(teleportTransformReference.position);
			c.SetRotation(teleportTransformReference.rotation);
		}
	}
}
