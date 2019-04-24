using UnityEngine;
using UnityEngine.Events;

namespace SimpleVR
{
	[RequireComponent(typeof(Interactable))]
	public class InteractableHoverEvents : MonoBehaviour
	{
		[SerializeField] private UnityEvent onHovered = null;
		[SerializeField] private UnityEvent onUnhovered = null;

		private void OnEnable()
		{
			Interactable interactable = GetComponent<Interactable>();
			interactable.OnHovered += OnHovered;
			interactable.OnUnhovered += OnUnhovered;
		}

		private void OnDisable()
		{
			Interactable interactable = GetComponent<Interactable>();
			interactable.OnHovered += OnHovered;
			interactable.OnUnhovered += OnUnhovered;
		}

		private void OnHovered(HandVR handVR)
		{
			onHovered.Invoke();
		}

		private void OnUnhovered(HandVR handVR)
		{
			onUnhovered.Invoke();
		}
	}
}
