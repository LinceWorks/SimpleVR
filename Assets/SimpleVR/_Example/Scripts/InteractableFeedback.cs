using UnityEngine;

namespace SimpleVR.Example
{
	[RequireComponent(typeof(Interactable))]
	public class InteractableFeedback : MonoBehaviour
	{
		[SerializeField] private Outline outline = null;
		[SerializeField] private Outline.Mode mode = Outline.Mode.OutlineAndSilhouette;
		[SerializeField] private float width = 5f;

		public Interactable Interactable { get; private set; }
		public Outline Outline { get { return outline; } private set { outline = value; } }

		private void Awake()
		{
			Interactable = GetComponent<Interactable>();
		}

		private void OnValidate()
		{
			if (!Outline) Outline = GetComponent<Outline>();

			if (!Outline)
			{
				Debug.LogError("Assign Outline on: " + gameObject, gameObject);
				return;
			}

			Outline.enabled = false;
			Outline.OutlineMode = mode;
			Outline.OutlineWidth = width;
		}

		private void OnEnable()
		{
			Interactable.OnHovered += Hover;
			Interactable.OnUnhovered += Unhover;
		}

		private void OnDisable()
		{
			Interactable.OnHovered -= Hover;
			Interactable.OnUnhovered -= Unhover;
		}

		private void Hover(HandVR handVR)
		{
			Outline.enabled = true;
		}

		private void Unhover(HandVR handVR)
		{
			Outline.enabled = false;
		}
	}
}
