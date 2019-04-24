using UnityEngine;
using System.Collections;

namespace SimpleVR.Example
{
	[RequireComponent(typeof(HandVR))]
	public class HandVRFeedback : MonoBehaviour
	{
		public HandVR HandVR { get; private set; }

		public bool Hovering { get; private set; }
		public bool Attaching { get; private set; }

		private void Awake()
		{
			HandVR = GetComponent<HandVR>();
		}

		private void OnEnable()
		{
			HandVR.OnHandHoverChangeFeedback += HandHoverChangeFeedback;
			HandVR.OnHandAttachChangeFeedback += HandAttachChangeFeedback;
		}

		private void OnDisable()
		{
			HandVR.OnHandHoverChangeFeedback -= HandHoverChangeFeedback;
			HandVR.OnHandAttachChangeFeedback -= HandAttachChangeFeedback;
		}

		private void HandHoverChangeFeedback(bool hover)
		{
			Hovering = hover;
			if (hover)
			{
				StartCoroutine(HoverHandFeedback());

				if (HandVR.HoveredInteractable is Grabbable) GetComponentInChildren<Renderer>().transform.localScale = Vector3.one * 0.1f + Vector3.forward * 0.05f;
				else GetComponentInChildren<Renderer>().transform.localScale = Vector3.one * 0.1f + Vector3.right * 0.05f;
			}
			else
			{
				GetComponentInChildren<Renderer>().transform.localScale = Vector3.one * 0.1f;
			}
		}

		private void HandAttachChangeFeedback(bool attach)
		{
			Attaching = attach;
			if(attach) HandVR.Haptic(0, DataVR.Instance.hand.grabHaptics.durationSeconds, DataVR.Instance.hand.grabHaptics.frequency, DataVR.Instance.hand.grabHaptics.amplitude);
		}

		private IEnumerator HoverHandFeedback()
		{
			StopCoroutine(HoverHandFeedback());
			while (Hovering)
			{
				HandVR.Haptic(0, DataVR.Instance.hand.interactableHoverHaptics.durationSeconds, DataVR.Instance.hand.interactableHoverHaptics.frequency, DataVR.Instance.hand.interactableHoverHaptics.amplitude);
				yield return null;
			}
		}
	}
}
