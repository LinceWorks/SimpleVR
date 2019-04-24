#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace SimpleVR
{
	[CustomEditor(typeof(AttachmentPoint)), CanEditMultipleObjects]
	public class AttachmentPointEditor : Editor
	{
		private void OnSceneGUI()
		{
			AttachmentPoint attachmentPoint = (AttachmentPoint)target;
			Handles.Label(attachmentPoint.AttachPosition, "Attachment: " + attachmentPoint.AttachmentPointName?.name);

			if (Application.isPlaying)
			{
				Handles.PositionHandle(attachmentPoint.AttachPosition, attachmentPoint.AttachRotation);
				return;
			}

			EditorGUI.BeginChangeCheck();
			Vector3 newAttachPosition = Handles.PositionHandle(attachmentPoint.AttachPosition, attachmentPoint.AttachRotation);
			Quaternion newAttachRotation = Handles.RotationHandle(attachmentPoint.AttachRotation, attachmentPoint.AttachPosition);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(attachmentPoint, "Attachment offset change.");
				attachmentPoint.AttachPosition = newAttachPosition;
				attachmentPoint.AttachRotation = newAttachRotation;
			}
		}
	}
}
#endif
