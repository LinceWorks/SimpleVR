using UnityEngine;
using DG.Tweening;
using SimpleVR.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SpatialTracking;

namespace SimpleVR
{
	[RequireComponent(typeof(Rigidbody))]
	public class Grabbable : Interactable
	{
		private int frameCount = 0;

		public enum BelongState : byte
		{
			Free,       //nobody has it
			Stored,     //in a Slot
			Attached,   //in a HandVR
			OtherOwns   //other (e.g.: an enemy) has it
		}

		#region Events & Delegates
		[HideInInspector] public event OnInteractionDelegate OnAttached;
		[HideInInspector] public event OnInteractionDelegate OnDetached;
		#endregion

		#region References
		private Dictionary<string, AttachmentPoint> attachmentPoints = new Dictionary<string, AttachmentPoint>();
		#endregion

		#region Fields
		[SerializeField] private AttachmentTypeName attachmentTypeName = null;
		private BelongState state = BelongState.Free;   //tracks who has this
		private int currentAttachmentPointIndex = 0;
		#endregion

		#region Properties
		public Rigidbody Rigidbody { get; private set; }

		public bool Free { get { return state == BelongState.Free; } }
		public bool OtherOwns { get { return state == BelongState.OtherOwns; } set { if (value) state = BelongState.OtherOwns; else state = BelongState.Free; } }

		public HandVR OwnerHandVR { get; private set; } //current or last ownerHandVR, look for OwnerHandVR = null commented line for better explanation
		public bool Attached { get { return state == BelongState.Attached; } }

		public Slot OwnerSlot { get; private set; }
		public bool Stored { get { return state == BelongState.Stored; } }

		public AttachmentTypeName SlotType { get { return attachmentTypeName; } }

		public bool Blocked { get { return Attached ? (OwnerHandVR.Blocked || Overlapping) : Stored; } }
		public Vector3 Velocity { get { return Attached ? OwnerHandVR.Velocity : Rigidbody.velocity; } }
		public Vector3 AngularVelocity { get { return Attached ? OwnerHandVR.AngularVelocity : Rigidbody.angularVelocity; } }

		public bool CollisionIsIntentional { get { if (Blocked) return false; else return Velocity.sqrMagnitude > DataVR.Instance.hand.minimumHandVRCollisionSqrVelocity || AngularVelocity.sqrMagnitude > DataVR.Instance.hand.minimumHandVRCollisionSqrAngularVelocity; } }
		#endregion

		public void SetOwnerSlot(Slot slot)
		{
			OwnerSlot = slot;
			state = BelongState.Stored;
		}

		#region Setters

		#endregion

		protected override void Awake()
		{
			base.Awake();
			Rigidbody = GetComponent<Rigidbody>();
			Rigidbody.Sleep();

			//AttachmentPoints
			//-----------------------------------------------------------------------------
			AttachmentPoint[] attachmentPoints = GetComponentsInChildren<AttachmentPoint>();

			for (int i = 0; i < attachmentPoints.Length; i++)
			{
				this.attachmentPoints[attachmentPoints[i].AttachmentPointName.name] = attachmentPoints[i];
			}
		}

		public override void Interact(HandVR handVR)
		{
			base.Interact(handVR);

			if (handVR.GrabDown() && (!Attached || handVR != OwnerHandVR))
			{
				Attach(handVR);
			}
			else if (!handVR.Grab() && Attached && handVR == OwnerHandVR && !Overlapping)   //!handVR.Interact() instead of handVR.InteractUp() because it works if hand did the interactUp inside a wall and then it went out so it wasn't Blocked anymore
			{
				Detach();
			}
		}

		public void Attach(HandVR handVR, bool iterateAttachmentPoint = false)
		{
			//Debug.Log("Attach: " + this, gameObject);

			// TO DO: Test if not necessary I did it because maybe detach events are important. But attach should let things as they should be.
			if (Attached)
			{
				//True to avoid possible attach to Slot. Imagine you get from one hand to the other but the from hand is hovering a slot. then the detach will attach grabbable to the slot and then attach to the new hand making a strange effect
				Detach(true);
			}
			else if (Stored)
			{
				TryUnstore(OwnerSlot);
			}

			if (iterateAttachmentPoint) currentAttachmentPointIndex++;
			if (currentAttachmentPointIndex == attachmentPoints.Count) currentAttachmentPointIndex = 0;

			transform.DOKill();
			transform.parent = handVR.transform;
			OwnerHandVR = handVR;   //Important to do this and next line before real physic attach (position and rotation) to be able to detach from it if other hand steal it on the fly to the hand
			handVR.AttachedGrabbable = this;
			DisableColliders(); //Disable collider to avoid not desired collisions on attach

			Vector3 attachPositionOffset = Vector3.zero;
			Quaternion attachRotationOffset = Quaternion.identity;
			GetLocalAttachmentPositionAndRotation(handVR.transform, out attachPositionOffset, out attachRotationOffset, handVR.AttachmentPointName.name, iterateAttachmentPoint);

			float attachTime = DataVR.Instance.grabbable.attachTime;

			transform.DOLocalMove(-attachPositionOffset, attachTime).SetEase(Ease.OutSine);
			transform.DOLocalRotate(-attachRotationOffset.eulerAngles, attachTime).SetEase(Ease.OutSine).OnComplete
				(
				() =>
				{
					PhysicsExt.IgnoreCollisions(OwnerHandVR.CharacterVR.CharacterController, Colliders);    //Ignore Collisions between this and CharacterVR
					EnableColliders();  //Enable collider (disabled above, before tween attach) WARNING. it is detected by ontriggerenter of physicsproximityAdjust

					//TrackedPoseDriver approach
					{
						////Set final position
						////-------------------------------------------------
						//if (attachmentPoints.Count == 0 || (handVR.AttachmentPointName.name == "" && iterateAttachmentPoint == false))
						//{
						//	transform.localPosition = Vector3.zero;
						//	transform.localRotation = Quaternion.identity;
						//}
						//else
						//{
						//	transform.localPosition = -attachPositionOffset;
						//	transform.localRotation = Quaternion.Inverse(attachRotationOffset);
						//}
						////-------------------------------------------------
					}

					//SLOT: as it could be in a slot make it kinematic again
					Rigidbody.interpolation = RigidbodyInterpolation.None;
					Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
					//TrackedPoseDriver approach
					{
						//Original approach
						//Rigidbody.isKinematic = true;

						AddTrackedPoseDriver(handVR, attachPositionOffset, attachRotationOffset);
					}
					Rigidbody.useGravity = false;

					state = BelongState.Attached;

					//Events
					OnAttached?.Invoke(handVR);

#if UNITY_EDITOR
					Debug.Log("Attach Grabbable: " + this + " to HandVR: " + handVR, gameObject);
#endif
				}
			);
		}

		public void Detach(bool fromAttachMethod = false)
		{
			//Debug.Log("Detach: " + this, gameObject);

			//TrackedPoseDriver approach
			{
				DestroyTrackedPoseDriver();
			}

			transform.DOKill();

			//SLOT
			//-------------------------------------------------
			if (!fromAttachMethod)
			{
				TryStore(OwnerHandVR);
			}
			//-------------------------------------------------

			OwnerHandVR.AttachedGrabbable = null;

			//not needed, the BelongState gives if it is attached, not need to check if ownerHandVR is null like before, so ownerHandVR mainstains the last ownerHandVR when state is changed until it is attached again
			//can be useful to return grabbable to last hand
			//OwnerHandVR = null;

			//if it has been attached to a slot
			if (Stored)
			{
				//Events
				OnDetached?.Invoke(OwnerHandVR);

#if UNITY_EDITOR
				Debug.Log("Dettached and Stored Grabbable: " + this + " from HandVR: " + OwnerHandVR, gameObject);
#endif

				return;
			}

			//TrackedPoseDriver approach
			{
				transform.position = /*OwnerHandVR.transform.position*/ OwnerHandVR.PoseProvider.AttachPosition;
				transform.rotation = /*OwnerHandVR.transform.rotation*/ OwnerHandVR.PoseProvider.AttachRotation;
			}

			transform.parent = null;

			AdjustPhysicsWithPhysicsTracker(OwnerHandVR);

			state = BelongState.Free;

			PhysicsExt.IgnoreCollisions(OwnerHandVR.CharacterVR.CharacterController, Colliders, false);    //Undo Ignore Collisions between this and CharacterVR

			//Events
			OnDetached?.Invoke(OwnerHandVR);

#if UNITY_EDITOR
			Debug.Log("Dettached Grabbable: " + this + " from HandVR: " + OwnerHandVR, gameObject);
#endif
		}

		public void AddTrackedPoseDriver(HandVR handVR, Vector3 attachPositionOffset, Quaternion attachRotationOffset)
		{
			transform.parent = null;

			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;

			//TrackedPoseDriver tpd = gameObject.AddComponent<TrackedPoseDriver>();
			//Just in case... check if already has a trackedPoseProvider
			TrackedPoseDriver tpd = gameObject.GetComponent<TrackedPoseDriver>();
			if (!tpd) tpd = gameObject.AddComponent<TrackedPoseDriver>();
			tpd.UseRelativeTransform = true;

			handVR.PoseProvider.AttachPositionOffset = attachPositionOffset;
			handVR.PoseProvider.AttachRotationOffset = attachRotationOffset;
			tpd.poseProviderComponent = handVR.PoseProvider;
		}

		public void DestroyTrackedPoseDriver(bool immediate = false)
		{
			if(immediate) DestroyImmediate(GetComponent<TrackedPoseDriver>());
			else Destroy(GetComponent<TrackedPoseDriver>());
		}

		private void OnCollisionEnter(Collision collision)
		{
			//Avoid double collision on same frame
			if (Time.frameCount == frameCount) return;
			frameCount = Time.frameCount;

			//if (!Attached) return;
			//Rigidbody rb = collision.collider.GetComponentInParent<Rigidbody>();
			//if (!rb) return;
			//rb.AddForce(SpeedToForce.SpeedToCollisionForce(OwnerHandVR.PhysicsTracker.Velocity, rb.mass), ForceMode.Impulse);
		}

		private bool TryStore(HandVR handVR)
		{
			if (handVR.HoveredInteractable == null) return false;

			Slot slot = handVR.HoveredInteractable.GetComponent<Slot>();

			if (slot == null || slot.SlotType != attachmentTypeName) return false;

			slot.Store(this, OwnerHandVR.CharacterVR);
			OwnerSlot = slot;

			return true;
		}

		private bool TryUnstore(Slot ownerSlot)
		{
			if (ownerSlot == null) return false;

			ownerSlot.Unstore(this);
			return true;
		}

		/// <summary>
		/// Gives velocity to Rigidbody
		/// </summary>
		/// <param name="rb"></param>
		/// <param name="handVR"></param>
		private void AdjustPhysicsWithPhysicsTracker(HandVR handVR)
		{
			Rigidbody.isKinematic = false;	//Slot Store method set Grabbables as kinematic
			Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			Rigidbody.useGravity = true;

			Vector3 position = Vector3.zero;
			Vector3 velocity = Vector3.zero;
			Vector3 angularVelocity = Vector3.zero;


			Rigidbody.velocity = handVR.Velocity;
			Rigidbody.angularVelocity = handVR.AngularVelocity;

			// Make the object travel at the release velocity for the amount
			// of time it will take until the next fixed update, at which
			// point Unity physics will take over
			float timeUntilFixedUpdate = (Time.fixedDeltaTime + Time.fixedTime) - Time.time;
			transform.position += timeUntilFixedUpdate * velocity;
			float angle = Mathf.Rad2Deg * angularVelocity.magnitude;
			Vector3 axis = angularVelocity.normalized;
			transform.rotation *= Quaternion.AngleAxis(angle * timeUntilFixedUpdate, axis);

			StartCoroutine(EndAdjustPhysicsWithPhysicsTracker());
		}

		/// <summary>
		/// Return to usual interpolation mode.
		/// </summary>
		/// <returns></returns>
		private IEnumerator EndAdjustPhysicsWithPhysicsTracker()
		{
			yield return new WaitForEndOfFrame();
			Rigidbody.interpolation = RigidbodyInterpolation.None;
		}

		public bool GetLocalAttachmentPositionAndRotation(Transform transformToAttachTo, out Vector3 attachPositionOffset, out Quaternion attachRotationOffset, string attachmentPointName = "", bool iterateAttachmentPoint = false)
		{
			if (attachmentPoints.Count == 0 || (attachmentPointName == "" && iterateAttachmentPoint == false))
			{
				attachPositionOffset = Vector3.zero;
				attachRotationOffset = Quaternion.identity;
				return false;
			}

			AttachmentPoint ap = iterateAttachmentPoint ? attachmentPoints.Values.ElementAt(currentAttachmentPointIndex) : attachmentPoints.ContainsKey(attachmentPointName) ? attachmentPoints[attachmentPointName] : null;

			//if (ap) return Vector3.Scale(-ap.AttachEulerAnglesOffset, -ap.AttachPositionOffset);	That not work because of Gimbal Lock and Quaternion order we need to do Quaternion.Inverse (-eulerAngles not working)
			if (ap)
			{
				attachPositionOffset = ap.AttachPositionOffset;
				attachRotationOffset = ap.AttachRotationOffset;
				return true;
			}

			attachPositionOffset = Vector3.zero;
			attachRotationOffset = Quaternion.identity;
			return false;
		}

		//private void OnDrawGizmosSelected()
		//{
		//	Color old = Gizmos.color;
		//	Gizmos.color = Color.magenta;

		//	for (int i = 0; i < Colliders.Length; i++)
		//	{
		//		BoxCollider boxCollider = Colliders[i] as BoxCollider;
		//		if (boxCollider)
		//		{
		//			Vector3 pos0 = transform.TransformPoint(boxCollider.center - Vector3.right * boxCollider.size.x * 0.5f - Vector3.up * boxCollider.size.y * 0.5f - Vector3.forward * boxCollider.size.z * 0.5f);
		//			Vector3 pos1 = transform.TransformPoint(boxCollider.center + Vector3.right * boxCollider.size.x * 0.5f - Vector3.up * boxCollider.size.y * 0.5f - Vector3.forward * boxCollider.size.z * 0.5f);
		//			Vector3 pos2 = transform.TransformPoint(boxCollider.center - Vector3.right * boxCollider.size.x * 0.5f + Vector3.up * boxCollider.size.y * 0.5f - Vector3.forward * boxCollider.size.z * 0.5f);
		//			Vector3 pos3 = transform.TransformPoint(boxCollider.center - Vector3.right * boxCollider.size.x * 0.5f - Vector3.up * boxCollider.size.y * 0.5f + Vector3.forward * boxCollider.size.z * 0.5f);

		//			Vector3 pos4 = transform.TransformPoint(boxCollider.center + Vector3.right * boxCollider.size.x * 0.5f + Vector3.up * boxCollider.size.y * 0.5f + Vector3.forward * boxCollider.size.z * 0.5f);
		//			Vector3 pos5 = transform.TransformPoint(boxCollider.center - Vector3.right * boxCollider.size.x * 0.5f + Vector3.up * boxCollider.size.y * 0.5f + Vector3.forward * boxCollider.size.z * 0.5f);
		//			Vector3 pos6 = transform.TransformPoint(boxCollider.center + Vector3.right * boxCollider.size.x * 0.5f - Vector3.up * boxCollider.size.y * 0.5f + Vector3.forward * boxCollider.size.z * 0.5f);
		//			Vector3 pos7 = transform.TransformPoint(boxCollider.center + Vector3.right * boxCollider.size.x * 0.5f + Vector3.up * boxCollider.size.y * 0.5f - Vector3.forward * boxCollider.size.z * 0.5f);

		//			Gizmos.DrawWireSphere(pos0, 0.01f);
		//			Gizmos.DrawWireSphere(pos1, 0.01f);
		//			Gizmos.DrawWireSphere(pos2, 0.01f);
		//			Gizmos.DrawWireSphere(pos3, 0.01f);
		//			Gizmos.DrawWireSphere(pos4, 0.01f);
		//			Gizmos.DrawWireSphere(pos5, 0.01f);
		//			Gizmos.DrawWireSphere(pos6, 0.01f);
		//			Gizmos.DrawWireSphere(pos7, 0.01f);
		//		}
		//	}
		//	Gizmos.color = old;
		//}
	}
}