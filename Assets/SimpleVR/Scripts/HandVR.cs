using UnityEngine;
using UnityEngine.SpatialTracking;
using SimpleVR.Utils;
using Unity.Labs.SuperScience;
using System.Collections;

namespace SimpleVR
{
	[RequireComponent(typeof(TrackedPoseDriver))]
	public class HandVR : MonoBehaviour
	{
		public enum Handtype
		{
			Left,
			Right
		}

#if UNITY_EDITOR
		#region Debug
		Vector3 hoveredClosestPoint;
		#endregion
#endif
		#region References
		private Interactable hoveredInteractable = null;
		private Grabbable attachedGrabbable = null;
		private HandVR otherHandVR = null;
		#endregion

		#region Fields
		private bool? blocked = null;
		[SerializeField] private AttachmentTypeName attachmentPointName = null; //AttachmentPointName used if desired
		[SerializeField] private Vector3 forwardDetectionDirection = Vector3.forward;
		#endregion

		#region Properties
		public CharacterVR CharacterVR { get; private set; }
		public PhysicsTracker PhysicsTracker { get; private set; }
		public Collider Collider { get; private set; }
		public TrackedPoseDriver TrackedPoseDriver { get; private set; }
		public PoseProvider PoseProvider { get; private set; }
		public Vector3 InteractableDetectionDirection { get { return (transform.position - CharacterVR.EyesPosition).normalized; } }
		public Vector3 ForwardDetectionDirection { get { return forwardDetectionDirection; } set { forwardDetectionDirection = value; } }
		public bool Visible
		{
			get
			{
				if (AttachedGrabbable && HoveredInteractable)
					return transform.VisibleFrom(CharacterVR.HeadTransform.position, DataVR.Instance.hand.blockingElements, QueryTriggerInteraction.Ignore, new GameObject[] { AttachedGrabbable.gameObject, HoveredInteractable.gameObject });
				else if(AttachedGrabbable)
					return transform.VisibleFrom(CharacterVR.HeadTransform.position, DataVR.Instance.hand.blockingElements, QueryTriggerInteraction.Ignore, new GameObject[] { AttachedGrabbable.gameObject });
				else if (HoveredInteractable)
					return transform.VisibleFrom(CharacterVR.HeadTransform.position, DataVR.Instance.hand.blockingElements, QueryTriggerInteraction.Ignore, new GameObject[] { HoveredInteractable.gameObject });
				else
					return transform.VisibleFrom(CharacterVR.HeadTransform.position, DataVR.Instance.hand.blockingElements, QueryTriggerInteraction.Ignore);
			}
		}
		public bool Blocked { get { if (blocked == null) blocked = !Visible; return blocked.Value; } }  //On late update we make it null so this is calculated one time per frame
		public Interactable HoveredInteractable
		{
			get { return hoveredInteractable; }
			private set
			{
				if (value == hoveredInteractable) return;
				hoveredInteractable?.Unhover(this);
				hoveredInteractable = value;
				value?.Hover(this);
				OnHandHoverChangeFeedback?.Invoke(value);
			}
		}
		public Grabbable AttachedGrabbable
		{
			get { return attachedGrabbable; }
			set
			{
				if (value != null)
				{
					HoveredInteractable = null;
				}
				else
				{
					//attachedGrabbable is going to be set as null, so it has been detached, so it has to free all collision ignores with nearInteractables of CharacterVR
					CharacterVR.FreeCollisionIgnores(attachedGrabbable);

					//Disable Collider to avoid just detached Grabbable to collide with HandVR on detach
					Collider.enabled = false;

					//Reenable HandVR collider in half a second
					StartCoroutine(
						DelayedCallback(0.5f, () => {
							Collider.enabled = true;
						})
					);
				}

				attachedGrabbable = value;
				OnHandAttachChangeFeedback?.Invoke(value);
			}
		}
		public HandVR OtherHandVR
		{
			get
			{
				if (!otherHandVR)
				{
					for (int i = 0; i < CharacterVR.HandsVR.Length; i++)
					{
						if (CharacterVR.HandsVR[i] != this)
						{
							otherHandVR = CharacterVR.HandsVR[i];
							break;
						}
					}
				}

				return otherHandVR;
			}
		}

		//HandType
		//----------------------------------------------------------------
		public Handtype HandType { get { return TrackedPoseDriver.poseSource == TrackedPoseDriver.TrackedPose.LeftPose ? Handtype.Left : Handtype.Right; } }
		public bool IsLeft { get { return TrackedPoseDriver.poseSource == TrackedPoseDriver.TrackedPose.LeftPose; } }
		public bool IsRight { get { return TrackedPoseDriver.poseSource == TrackedPoseDriver.TrackedPose.RightPose; } }

		//PhysicsTracker
		//----------------------------------------------------------------
		public Vector3 Velocity { get { return PhysicsTracker.Velocity; } }
		public Vector3 AngularVelocity { get { return PhysicsTracker.AngularVelocity; } }
		public ScriptableObject AttachmentPointName { get { return attachmentPointName; } }
		public bool CollisionIsIntentional { get { if (Blocked) return false; else return Velocity.sqrMagnitude > DataVR.Instance.hand.minimumHandVRCollisionSqrVelocity || AngularVelocity.sqrMagnitude > DataVR.Instance.hand.minimumHandVRCollisionSqrAngularVelocity; } }
		#endregion

		#region Events & Delegates
		public delegate void OnSomethingChange(bool hover);

		[HideInInspector] public event OnSomethingChange OnHandHoverChangeFeedback;
		[HideInInspector] public event OnSomethingChange OnHandAttachChangeFeedback;
		#endregion

		private void Awake()
		{
			CharacterVR = GetComponentInParent<CharacterVR>();
			PhysicsTracker = new PhysicsTracker();
			Collider = GetComponentInChildren<Collider>();
			TrackedPoseDriver = GetComponent<TrackedPoseDriver>();
			PoseProvider = GetComponent<PoseProvider>();
		}

		private void Start()
		{
			PhysicsTracker.Reset(transform.position, transform.rotation, Vector3.zero, Vector3.zero);
		}

		private void Update()
		{
			PhysicsTracker.Update(transform.position, transform.rotation, Time.smoothDeltaTime);

			//If hand is blocked cannot do anything
			if (Blocked)
			{
				//Debug.Log("blocked");
				HoveredInteractable = null;
				return;
			}

			//Interactions with attached grabbable objects, it blocks hover interactions
			if (AttachedGrabbable)
			{
				//Debug.Log("AttachedGrabbable: " + AttachedGrabbable);
				AttachedGrabbable.Interact(this);
			}
			//IMPORTANT can't be else if because need to hover Slots to be able to let Grabbables on Slots
			/*else*/ if (HoveredInteractable = HoverInteractableDetection())
			{
				//Debug.Log("HoveredInteractable: " + HoveredInteractable);
				HoveredInteractable.Interact(this);
			}
		}

		protected virtual void LateUpdate()
		{
			blocked = null;
			if (AttachedGrabbable) AttachedGrabbable.NullOverlapping();
		}

		/// <summary>
		/// Detection of Interactables in a radius or inside a capsule.
		/// </summary>
		/// <returns></returns>
		private Interactable HoverInteractableDetection()
		{
			int length = DataVR.Instance.hand.hoverDetectionMaxColliders;   //greater == more precision
			Collider[] results = new Collider[length];
			float[] sqrDistances = new float[length];
			Vector3[] closestPoints = new Vector3[length];
			float hoverDetectionRadius = DataVR.Instance.hand.hoverDetectionRadius;

			if (DataVR.Instance.hand.hoverDetectionCapsule)
				Physics.OverlapCapsuleNonAlloc(transform.position, transform.position + InteractableDetectionDirection.normalized * DataVR.Instance.hand.hoverDetectionCapsuleLength, hoverDetectionRadius, results, DataVR.Instance.hand.interactableElements, QueryTriggerInteraction.Ignore);
			else
				Physics.OverlapSphereNonAlloc(transform.position, hoverDetectionRadius, results, DataVR.Instance.hand.interactableElements, QueryTriggerInteraction.Ignore);

//#if UNITY_EDITOR
//			if(TrackedPoseDriver.poseSource == TrackedPoseDriver.TrackedPose.RightPose)
//			for (int i = 0; i < length; i++)
//			{
//				if(results[i] != null) Debug.Log("hoverResult: " + results[i], results[i].gameObject);
//			}
//#endif

			//Order by distance
			for (int i = 0; i < length; i++)
			{
				if (results[i] == null)
				{
					sqrDistances[i] = float.MaxValue;
					continue;
				}

				//Get the closest point in the collider of the detected interactable to the hand
				Vector3 closestPoint = Physics.ClosestPoint(transform.position, results[i], results[i].transform.position, results[i].transform.rotation);

				//SquareDistance between this hand and closest point of the hover detected collider
				sqrDistances[i] = (closestPoint - transform.position).sqrMagnitude;
				closestPoints[i] = closestPoint;
			}

			//Sort results and closestPoints by distance
			float[] sqrDistancesCopy = new float[length];
			System.Array.Copy(sqrDistances, sqrDistancesCopy, length);

			System.Array.Sort(sqrDistances, results);
			System.Array.Sort(sqrDistancesCopy, closestPoints);

			//Return first valid result
			for (int i = 0; i < results.Length; i++)
			{
				if (sqrDistances[i] == float.MaxValue) return null;

				//3 types with different validation conditions to be detected
				//Interactable				-> !AttachedGrabbable, sqrDistance < hoverDetectionRadius^2, visible || ethereal
				//Interactable Slot			-> sqrDistance < hoverDetectionRadius^2, (slot.AttachedGrabbable && !AttachedGrabbable) || !slot.AttachedGrabbable && AttachedGrabbable && slot.SlotType == AttachedGrabbable.SlotType
				//Interactable Grabbable	-> !AttachedGrabbable, visible || ethereal, !Stored

				//Grabbable before Slot because results[i].GetComponentInParent<Slot>(); can be truth with Grabbables (when they are stored)
				Grabbable grabbable = results[i].GetComponentInParent<Grabbable>();

				if (grabbable)
				{
					if (!AttachedGrabbable &&
						!grabbable.Undetectable &&
						(grabbable.ConstrainedDetection ? sqrDistances[i] <= hoverDetectionRadius * hoverDetectionRadius : true) &&
						!grabbable.Stored &&
						(grabbable.Ethereal || results[i].transform.VisibleSpecificPointFrom(transform.position, closestPoints[i], DataVR.Instance.hand.blockingElements, QueryTriggerInteraction.Ignore)))
					{
#if UNITY_EDITOR
						hoveredClosestPoint = closestPoints[i];
#endif
						return grabbable;
					}
					continue;
				}

				Slot slot = results[i].GetComponentInParent<Slot>();

				if(slot)
				{
					if ((slot.ConstrainedDetection ? sqrDistances[i] <= hoverDetectionRadius * hoverDetectionRadius : true) && 
						((slot.AttachedGrabbable && !AttachedGrabbable) || !slot.AttachedGrabbable && AttachedGrabbable && slot.SlotType == AttachedGrabbable.SlotType))
					{
#if UNITY_EDITOR
						hoveredClosestPoint = closestPoints[i];
#endif
						return slot;
					}

					continue;
				}

				Interactable interactable = results[i].GetComponentInParent<Interactable>();

				//It is useful to have things in interactable layer but without interactable component
				//Because they don't collide with CharacterVR layer
				if (interactable)
				{
					if (!AttachedGrabbable && 
						!interactable.Undetectable && 
						(interactable.ConstrainedDetection ? sqrDistances[i] <= hoverDetectionRadius * hoverDetectionRadius : true) && 
						(interactable.Ethereal || results[i].transform.VisibleSpecificPointFrom(transform.position, closestPoints[i], DataVR.Instance.hand.blockingElements, QueryTriggerInteraction.Ignore)))
					{
#if UNITY_EDITOR
						hoveredClosestPoint = closestPoints[i];
#endif
						return interactable;
					}

					continue;
				}
				continue;
			}
			return null;
		}

		private void OnCollisionEnter(Collision collision)
		{
			//if attachedGrabbable, deal collisions with it. Maybe it can change in the future (maybe having a sword in hand player can stun enemies with his fists (up-to-down punch))
			if (Blocked || AttachedGrabbable) return;
			
			//Check if it is intentional
			if(!CollisionIsIntentional) return;

			Rigidbody rb = collision.collider.GetComponentInParent<Rigidbody>();
			if (!rb) return;
			rb.AddForce(SpeedToForce.SpeedToCollisionForce(PhysicsTracker.Velocity, rb.mass), ForceMode.Impulse);
		}

		public IEnumerator DelayedCallback(float waitTime, System.Action CallBack)
		{
			yield return new WaitForSeconds(waitTime);
			CallBack();
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			DrawCapsule(transform.position - InteractableDetectionDirection.normalized * DataVR.Instance.hand.hoverDetectionRadius, transform.position + InteractableDetectionDirection.normalized * (DataVR.Instance.hand.hoverDetectionCapsuleLength + DataVR.Instance.hand.hoverDetectionRadius), Color.red, DataVR.Instance.hand.hoverDetectionRadius);
			Color c = Gizmos.color;
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, DataVR.Instance.hand.hoverDetectionRadius);
			Gizmos.color = c;

			if(HoveredInteractable) Debug.DrawLine(transform.position, hoveredClosestPoint, Color.green);
		}

		/// <summary>
		/// From DebugExtension asset on asset store
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="color"></param>
		/// <param name="radius"></param>
		public static void DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1)
		{
			Vector3 up = (end - start).normalized * radius;
			Vector3 forward = Vector3.Slerp(up, -up, 0.5f);
			Vector3 right = Vector3.Cross(up, forward).normalized * radius;

			Color oldColor = Gizmos.color;
			Gizmos.color = color;

			float height = (start - end).magnitude;
			float sideLength = Mathf.Max(0, (height * 0.5f) - radius);
			Vector3 middle = (end + start) * 0.5f;

			start = middle + ((start - middle).normalized * sideLength);
			end = middle + ((end - middle).normalized * sideLength);

			//Radial circles
			//DebugExtension.DrawCircle(start, up, color, radius);
			//DebugExtension.DrawCircle(end, -up, color, radius);

			//Side lines
			Debug.DrawLine(start + right, end + right, color);
			Debug.DrawLine(start - right, end - right, color);

			Debug.DrawLine(start + forward, end + forward, color);
			Debug.DrawLine(start - forward, end - forward, color);

			for (int i = 1; i < 26; i++)
			{

				//Start endcap
				Debug.DrawLine(Vector3.Slerp(right, -up, i / 25.0f) + start, Vector3.Slerp(right, -up, (i - 1) / 25.0f) + start, color);
				Debug.DrawLine(Vector3.Slerp(-right, -up, i / 25.0f) + start, Vector3.Slerp(-right, -up, (i - 1) / 25.0f) + start, color);
				Debug.DrawLine(Vector3.Slerp(forward, -up, i / 25.0f) + start, Vector3.Slerp(forward, -up, (i - 1) / 25.0f) + start, color);
				Debug.DrawLine(Vector3.Slerp(-forward, -up, i / 25.0f) + start, Vector3.Slerp(-forward, -up, (i - 1) / 25.0f) + start, color);

				//End endcap
				Debug.DrawLine(Vector3.Slerp(right, up, i / 25.0f) + end, Vector3.Slerp(right, up, (i - 1) / 25.0f) + end, color);
				Debug.DrawLine(Vector3.Slerp(-right, up, i / 25.0f) + end, Vector3.Slerp(-right, up, (i - 1) / 25.0f) + end, color);
				Debug.DrawLine(Vector3.Slerp(forward, up, i / 25.0f) + end, Vector3.Slerp(forward, up, (i - 1) / 25.0f) + end, color);
				Debug.DrawLine(Vector3.Slerp(-forward, up, i / 25.0f) + end, Vector3.Slerp(-forward, up, (i - 1) / 25.0f) + end, color);
			}

			Gizmos.color = oldColor;
		}
#endif
	}
}