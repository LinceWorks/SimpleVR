using UnityEngine;
using UnityEngine.Events;

namespace SimpleVR
{
	[RequireComponent(typeof(Grabbable))]
	public class AttachEvents : MonoBehaviour
	{
		[SerializeField] private UnityEvent onAttached = null;
		[SerializeField] private UnityEvent onDetached = null;

		private void OnEnable()
		{
			Grabbable grabbable = GetComponent<Grabbable>();
			grabbable.OnAttached += OnAttached;
			grabbable.OnDetached += OnDetached;
		}

		private void OnDisable()
		{
			Grabbable grabbable = GetComponent<Grabbable>();
			grabbable.OnAttached += OnAttached;
			grabbable.OnDetached += OnDetached;
		}

		private void OnAttached(HandVR handVR)
		{
			onAttached.Invoke();
		}

		private void OnDetached(HandVR handVR)
		{
			onDetached.Invoke();
		}
	}
}
