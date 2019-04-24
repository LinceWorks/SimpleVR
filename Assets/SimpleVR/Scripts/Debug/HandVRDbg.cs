#if UNITY_EDITOR
using UnityEngine;
using SimpleVR;

namespace SimpleVR.Dbg
{
	[RequireComponent(typeof(HandVR))]
	public class HandVRDbg : MonoBehaviour
	{
		[Header("Config")]
		public Color active;
		public Color inactive;

		[Header("References")]
		public Color blocked;
		public Color visible;
		public Color hover;
		public Color grab;

		//-----------------------------
		private HandVR handVR;

		private void Awake()
		{
			handVR = GetComponent<HandVR>();
		}

		private void Update()
		{
			blocked = handVR.Blocked ? active : inactive; 
			visible = handVR.Visible ? active : inactive; 
			hover = handVR.HoveredInteractable ? active : inactive;
			grab = handVR.AttachedGrabbable ? active : inactive;
		}
	}
}
#endif
