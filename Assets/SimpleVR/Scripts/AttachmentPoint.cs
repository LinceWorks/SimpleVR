using UnityEngine;

namespace SimpleVR
{
	public class AttachmentPoint : MonoBehaviour
	{
		#region Fields
		[SerializeField] private AttachmentTypeName attachmentPointName = null;
		[SerializeField] private Vector3 attachPositionOffset = Vector3.zero;
		[SerializeField] private Vector3 attachEulerAnglesOffset = Vector3.zero;
		#endregion

		#region Properties
		public AttachmentTypeName AttachmentPointName { get { return attachmentPointName; } }

		public Vector3 AttachPositionOffset { get { return attachPositionOffset; } set { attachPositionOffset = value; } }

		public Vector3 AttachEulerAnglesOffset { get { return attachEulerAnglesOffset; } set { attachEulerAnglesOffset = value; } }
		public Quaternion AttachRotationOffset { get { return Quaternion.Euler(attachEulerAnglesOffset); } set { attachEulerAnglesOffset = value.eulerAngles; } }

		public Vector3 AttachPosition { get { return transform.TransformPoint(AttachPositionOffset); } set { AttachPositionOffset = transform.InverseTransformPoint(value); } }

		public Vector3 AttachEulerAngles { get { return (transform.rotation * Quaternion.Euler(AttachEulerAnglesOffset)).eulerAngles; } set { attachEulerAnglesOffset = (transform.rotation * Quaternion.Euler(value)).eulerAngles; } }
		public Quaternion AttachRotation { get { return transform.rotation * AttachRotationOffset; } set { AttachRotationOffset = transform.rotation * value; } }
		#endregion

#if UNITY_EDITOR
		[ContextMenu("Fill Data")]
		public void FillData()
		{
			HandVR[] hands = FindObjectsOfType<HandVR>();

			for (int i = 0; i < hands.Length; i++)
			{
				if(hands[i].AttachmentPointName == AttachmentPointName)
				{
					GameObject g = new GameObject();
					g.transform.parent = hands[i].transform;
					g.transform.localPosition = Vector3.zero;
					g.transform.localRotation = Quaternion.identity;
					g.transform.parent = transform;
					AttachPositionOffset = g.transform.localPosition;
					AttachEulerAnglesOffset = g.transform.localEulerAngles;
					Destroy(g);
				}
			}
		}

		[ContextMenu("Fill Data 2")]
		public void FillData2()
		{
			GameObject g = new GameObject();
			g.transform.parent = transform.parent;
			g.transform.localPosition = Vector3.zero;
			g.transform.localRotation = Quaternion.identity;
			g.transform.parent = transform;
			AttachPositionOffset = g.transform.localPosition;
			AttachEulerAnglesOffset = g.transform.localEulerAngles;
			DestroyImmediate(g);
		}
#endif

		//GIZMOS
		//------------------------------------------------------------------------------------------
		private void OnDrawGizmosSelected()
		{
			Color c = Gizmos.color;
			Gizmos.color = Color.blue;

			Gizmos.DrawWireSphere(AttachPosition, 0.05f);

			Gizmos.color = c;
		}
	}
}
