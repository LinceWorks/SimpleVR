using UnityEngine;
using System.Collections.Generic;

namespace SimpleVR
{
	//Done on OnValidate
	//[RequireComponent(typeof(Collider))]
	public class Interactable : MonoBehaviour
	{
		#region References
		protected List<HandVR> hoverHands = new List<HandVR>();
		#endregion

		#region Fields
		private bool? overlapping = null;
		[SerializeField] private bool ethereal = false; //if ethereal it can be interacted through a blocking element
		[SerializeField] private bool constrainedDetection = false; //if DataVR.Instance.hand.hoverDetectionCapsule is true, use this to constrain detection to DataVR.Instance.hand.hoverDetectionRadius for this Interactable
		[SerializeField] private bool undetectable = false; //can't be detected by HandVR
		#endregion

		#region Properties
		public Transform Transform { get { return transform; } }
		public Collider[] Colliders { get; private set; }
		public bool Ethereal { get { return ethereal; } protected set { ethereal = value; } }
		public bool ConstrainedDetection { get { return constrainedDetection; } protected set { constrainedDetection = value; } }
		public bool Hovered { get; private set; }
		public bool Overlapping { get { if (overlapping == null) overlapping = IsOverlapping(); return overlapping.Value; } }
		public bool Undetectable { get { return undetectable; } set { undetectable = value; } }
		#endregion

		#region Events & Delegates
		public delegate void OnInteractionDelegate(HandVR handVR);

		[HideInInspector] public event OnInteractionDelegate OnInteract;
		[HideInInspector] public event OnInteractionDelegate OnHovered;
		[HideInInspector] public event OnInteractionDelegate OnUnhovered;
		#endregion

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (!GetComponentInChildren<Collider>() &&
				UnityEditor.EditorUtility.DisplayDialog("No collider found.", "Place a box collider?", "Place", "Do Not Place"))
			{
				gameObject.AddComponent<BoxCollider>();
			}
		}
#endif

		protected virtual void Awake()
		{
			Colliders = GetComponentsInChildren<Collider>();
		}

		public virtual void Hover(HandVR handVR, bool affectChildren = true)
		{
			hoverHands.Add(handVR);
			Hovered = true;
			OnHovered?.Invoke(handVR);
		}

		public virtual void Unhover(HandVR handVR, bool affectChildren = true)
		{
			hoverHands.Remove(handVR);

			if (hoverHands.Count == 0)
			{
				Hovered = false;
				OnUnhovered?.Invoke(handVR);
			}
		}

		public bool IsOverlapping()
		{
			bool isOverlapping = false;
			int[] layers0 = new int[Colliders.Length];
			LayerMask overlapCheckMask;

			for (int i = 0; i < Colliders.Length; i++)
			{
				layers0[i] = Colliders[i].gameObject.layer;
				Colliders[i].gameObject.layer = 2; //Ignore Raycast
			}

			//Check if is overlapping with something (if overlapping can't detach)
			for (int i = 0; i < Colliders.Length; i++)
			{
				if (!Colliders[i].enabled) continue;

				//LayerMask to check overlap is blocking elements' one
				overlapCheckMask = DataVR.Instance.hand.blockingElements;

				//if Colliders[i] can collide CharacterVR.CharacterController (check original layermask of it with layers0[i]), add CharacterVR.CharacterController layer to the LayerMask
				if (!Physics.GetIgnoreLayerCollision(layers0[i], DataVR.Instance.hand.characterControllerLayer))
				{
					overlapCheckMask |= (1 << DataVR.Instance.hand.characterControllerLayer);
				}

				BoxCollider boxCollider = Colliders[i] as BoxCollider;

				if (boxCollider)
				{
					if (Physics.CheckBox(Colliders[i].bounds.center, Vector3.Scale(boxCollider.transform.lossyScale, boxCollider.size) * 0.4f, boxCollider.transform.rotation, overlapCheckMask, QueryTriggerInteraction.Ignore))
					{
						isOverlapping = true;
						break;
					}
				}
				else
				{
					CapsuleCollider capsuleCollider = Colliders[i] as CapsuleCollider;
					if (capsuleCollider)
					{

						Vector3 direction = Vector3.up;
						float height = capsuleCollider.height * capsuleCollider.transform.lossyScale.y;
						float radius = capsuleCollider.radius * Mathf.Max(capsuleCollider.transform.lossyScale.x, capsuleCollider.transform.lossyScale.z);

						if (capsuleCollider.direction == 0)
						{
							direction = Vector3.right;
							height = capsuleCollider.height * capsuleCollider.transform.lossyScale.x;
							radius = capsuleCollider.radius * Mathf.Max(capsuleCollider.transform.lossyScale.y, capsuleCollider.transform.lossyScale.z);
						}
						else if (capsuleCollider.direction == 2)
						{
							direction = Vector3.forward;
							height = capsuleCollider.height * capsuleCollider.transform.lossyScale.z;
							radius = capsuleCollider.radius * Mathf.Max(capsuleCollider.transform.lossyScale.x, capsuleCollider.transform.lossyScale.y);
						}

						if (Physics.CheckCapsule(Colliders[i].bounds.center - direction * 0.4f * (height - radius),
							Colliders[i].bounds.center + direction * 0.4f * (height - radius),
							radius * 0.8f, overlapCheckMask, QueryTriggerInteraction.Ignore))
						{
							isOverlapping = true;
							break;
						}
					}
					else
					{
						SphereCollider sphereCollider = Colliders[i] as SphereCollider;

						if (sphereCollider)
						{
							if (Physics.CheckSphere(Colliders[i].bounds.center, Mathf.Max(sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.y, sphereCollider.transform.lossyScale.z) * sphereCollider.radius * 0.8f, overlapCheckMask, QueryTriggerInteraction.Ignore))
							{
								isOverlapping = true;
								break;
							}
						}
						else
						{
							MeshCollider meshCollider = Colliders[i] as MeshCollider;

							if (meshCollider)
							{
								//0.3f instead of 0.4f of boxCollider CheckBox because meshCollider.bounds.size isn't accurate on rotation with collider shape
								//this makes be more permisive to detach Grabbables when meshColliders are inside other colliders

								if (Physics.CheckBox(Colliders[i].bounds.center, Vector3.Scale(meshCollider.transform.lossyScale, meshCollider.bounds.size) * 0.3f, meshCollider.transform.rotation, overlapCheckMask, QueryTriggerInteraction.Ignore))
								{
									isOverlapping = true;
									break;
								}
							}
						}
					}
				}
			}

			for (int i = 0; i < Colliders.Length; i++)
			{
				Colliders[i].gameObject.layer = layers0[i];
			}

			return isOverlapping;
		}

		public void NullOverlapping()
		{
			overlapping = null;
		}

		public void EnableColliders()
		{
			for (int i = 0; i < Colliders.Length; i++)
			{
				Colliders[i].enabled = true;
			}
		}

		public void DisableColliders()
		{
			for (int i = 0; i < Colliders.Length; i++)
			{
				Colliders[i].enabled = false;
			}
		}

		public void ToggleColliders()
		{
			for (int i = 0; i < Colliders.Length; i++)
			{
				Colliders[i].enabled = !Colliders[i].enabled;
			}
		}

		public virtual void Interact(HandVR handVR)
		{
			OnInteract?.Invoke(handVR);
		}
	}
}
