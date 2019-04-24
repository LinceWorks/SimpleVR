using UnityEngine;
using UnityEngine.XR;
using SimpleVR.Utils;
using System.Collections.Generic;
using DG.Tweening;

namespace SimpleVR
{
	/// <summary>
	/// Deals with movement of character.
	/// Be sure that ChracterVR executes the first (Script Execution Order: -32000) or replicate the content of Init() manually on your player prefab.
	/// </summary>
	[RequireComponent(typeof(CharacterController))]
	public class CharacterVR : MonoBehaviour
	{
		#region References
		//Transform that contains trackedObjects (Head and Hands)
		[SerializeField] Transform trackedObjects = null;
		//Sibling order inside is parent
		private int trackedObjectsSiblingIndex = 0;
		//list that tracks all near interactable elements
		private List<Interactable> nearInteractables = new List<Interactable>();
		#endregion

		#region Fields
		[SerializeField] private int id = 0;
		private Vector3 lastCameraPosition = Vector3.zero;
		#endregion

		#region Properties
		public int ID { get { return id; } private set { id = value; } }
		public Transform HeadTransform { get; private set; }
		public CharacterController CharacterController { get; private set; }
		public HandVR[] HandsVR { get; private set; }
		public Vector3 TopPosition { get { return FeetPosition + CharacterController.height * Vector3.up; } }
		public Vector3 EyesPosition { get { return HeadTransform.position; } }
		public Vector3 ChestPosition { get { return CenterPosition + 0.5f * (EyesPosition - CenterPosition); } }
		public Vector3 CenterPosition { get { return FeetPosition + 0.5f * CharacterController.height * Vector3.up; } }
		public Vector3 FeetPosition { get { return transform.position; } }
		public float Height { get { return CharacterController.height; } }
		public float Radius { get { return CharacterController.radius; } }
		public float SkinWidth { get { return CharacterController.skinWidth; } }
		public bool Crouched { get { return Height < DataVR.Instance.crouchMaxHeight ? true : false; } }
		public float SuitableSpeed { get { return Crouched ? DataVR.Instance.crouchingSpeed : DataVR.Instance.standingSpeed; } }
		public Vector3 Velocity { get { return CharacterController.velocity; } }
		public bool TurnsBlocked { get; set; } = false;
		public bool MovementBlocked { get; set; } = false;
		#endregion

		private void Awake()
		{
			trackedObjectsSiblingIndex = trackedObjects.GetSiblingIndex();

			CharacterController = GetComponent<CharacterController>();
			//characterController.enableOverlapRecovery = true;

			HeadTransform = GetComponentInChildren<Camera>().transform;
			//HeadTransform.GetComponent<UnityEngine.SpatialTracking.TrackedPoseDriver>().originPose = Pose.identity;	//Not needed because unchecked UseRelativeTransform

			HandsVR = GetComponentsInChildren<HandVR>();

			Init();
		}

		/// <summary>
		/// Reset position and rotation of tracked objects and set use relative transform to avoid problems with Oculus Rift (on HTC Vive was not needed)
		/// Be sure that ChracterVR executes the first (Script Execution Order: -32000) or replicate the content of Init() manually on your player prefab.
		/// </summary>
		private void Init()
		{
			UnityEngine.SpatialTracking.TrackedPoseDriver[] trackpedPoseDrivers = trackedObjects.GetComponentsInChildren<UnityEngine.SpatialTracking.TrackedPoseDriver>();

			for (int i = 0; i < trackpedPoseDrivers.Length; i++)
			{
				trackpedPoseDrivers[i].transform.localPosition = Vector3.zero;
				trackpedPoseDrivers[i].transform.localRotation = Quaternion.identity;
				trackpedPoseDrivers[i].UseRelativeTransform = true;
			}
		}

		private void OnEnable()
		{
			Input.OnTurnAroundDown += TurnAround;
			Input.OnTurnLeftDown += TurnLeft;
			Input.OnTurnRightDown += TurnRight;
		}

		private void OnDisable()
		{
			Input.OnTurnAroundDown -= TurnAround;
			Input.OnTurnLeftDown -= TurnLeft;
			Input.OnTurnRightDown -= TurnRight;
		}

		private void Start()
		{

		}

		private void Update()
		{
			//------------------------------------
			FollowCamera();

			//------------------------------------
			Vector2 movementInput = Input.Move();
			Vector3 velocity = transform.right * movementInput.x * SuitableSpeed +
									Vector3.up * DataVR.Instance.characterGravity +
									transform.forward * movementInput.y * SuitableSpeed;

			//Debug.Log("velocity: " + velocity);
			if(!MovementBlocked) CharacterController.Move(velocity * Time.deltaTime);

			//------------------------------------
			//Now it is event driven because this failed
			//if (Input.TurnAroundDown())
			//{
			//	TurnAround();
			//}
		}

		private void TurnAround()
		{
			if (!TurnsBlocked) transform.forward = -transform.forward;
		}

		private void TurnLeft()
		{
			if(!TurnsBlocked) Rotate(-90f);
		}

		private void TurnRight()
		{
			if (!TurnsBlocked) Rotate(90f);
		}

		private void LateUpdate()
		{
			lastCameraPosition = HeadTransform.position;
		}

		private void FixedUpdate()
		{
			SetCollisionIgnores();
		}

		/// <summary>
		/// This message is received after current frame Update.
		/// Basically moves trackedObjects back in X and Z axis yo avoid head colliding with walls.
		/// Needs Camera tracking Update before this script.
		/// </summary>
		/// <param name="hit"></param>
		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			//Debug.Log("hit.moveDirection: " + hit.moveDirection);
			//Debug.Log("hit.moveLength: " + hit.moveLength);

			Vector3 scaledHitMoveDirection = Vector3.Scale(Vector3.right + Vector3.forward, hit.moveDirection); //remove Y component
			Vector3 absoluteCameraPositionDifference = (HeadTransform.position - lastCameraPosition).Abs();

			trackedObjects.position -= Vector3.Scale(scaledHitMoveDirection, absoluteCameraPositionDifference);
			//problematic on different heights than y = 0, makes CharacterVR fall //trackedObjects.localPosition = new Vector3(trackedObjects.localPosition.x, -CharacterController.skinWidth, trackedObjects.localPosition.z); //hack to avoid reduce height when something collides from above and there is not enough space
			return;
		}

		private void FollowCamera()
		{
			trackedObjects.parent = null;

			//POSITION
			//------------------------------------
			if (/*XRSettings.enabled && */ XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale)
			{
				CharacterController.Move(new Vector3(HeadTransform.position.x, transform.position.y, HeadTransform.position.z) - transform.position);

				CharacterController.height = (HeadTransform.position - transform.position).y + DataVR.Instance.headHeightAdd;
				CharacterController.center = 0.5f * CharacterController.height * Vector3.up;
			}

			//ROTATION
			//------------------------------------
			transform.forward = Vector3.Dot(HeadTransform.up, Vector3.up) * Vector3.ProjectOnPlane(HeadTransform.forward, Vector3.up);

			trackedObjects.parent = transform;
			trackedObjects.SetSiblingIndex(trackedObjectsSiblingIndex);
		}

		public void MoveTo(Vector3 position)
		{
			CharacterController.Move(position - transform.position);
		}

		public void SetPosition(Vector3 position)
		{
			transform.position = position;
		}

		public void LookAt(Vector3 forward)
		{
			transform.forward = forward;
		}

		public void SetRotation(Quaternion rotation)
		{
			transform.rotation = rotation;
		}

		public void SetRotation(Vector3 eulerAngles)
		{
			transform.eulerAngles = eulerAngles;
		}

		public void SetYawRotation(float yawAngle)
		{
			transform.eulerAngles = yawAngle * Vector3.up;
		}

		public void AddYawRotation(float yawAngle)
		{
			transform.eulerAngles = (transform.eulerAngles.y + yawAngle) * Vector3.up;
		}

		public void Rotate(float yawAngle)
		{
			transform.DORotate((transform.eulerAngles.y + yawAngle) * Vector3.up, DataVR.Instance.rotatingSpeed);
		}

		/// <summary>
		/// Adds interactables to the list of near interactables
		/// </summary>
		/// <param name="interactable"></param>
		public void AddNearInteractables(Interactable interactable)
		{
			//track if it is already in because Grabbables disable their colliders on Attach to "avoid not desired collisions"
			if (!nearInteractables.Contains(interactable)) nearInteractables.Add(interactable);
		}

		/// <summary>
		/// Removing near interactables.
		/// Important to make available collisions between them and attached grabbables again.
		/// </summary>
		/// <param name="interactable"></param>
		public void RemoveNearInteractables(Interactable interactable)
		{
			nearInteractables.Remove(interactable);

			//Reset collisions between attached interactables to hands and leaving interactable
			for (int i = 0; i < HandsVR.Length; i++)
			{
				if (HandsVR[i].AttachedGrabbable)
				{
					PhysicsExt.IgnoreCollisions(interactable.Colliders, HandsVR[i].AttachedGrabbable.Colliders, false);
				}
			}
		}

		/// <summary>
		/// Modify each nearInteractable collision with hands attached grabbables.
		/// Only can be collision between attached grabbables and other interactables if HandVR of the attached (the hand) is NOT Blocked (HandVR is Blocked when it isn't visible) and near interactable is visible from the hand
		/// </summary>
		private void SetCollisionIgnores()
		{
			for (int i = 0; i < HandsVR.Length; i++)
			{
				if (HandsVR[i].AttachedGrabbable)
				{
					int length = nearInteractables.Count;
					for (int j = 0; j < length; j++)
					{
						//Following line seems unneeded. Ignore or not collisions with itself don't affect
						//if (nearInteractables[j].gameObject == handsVR[i].AttachedGrabbable.gameObject) return;

						//grabbable is blocked if grabbing hand is blocked or if grabbable is overlapping.
						//if AttachedGrabbable is blocked or NOTHING -> following is deprecated ------->>>>>>   near interactable is NOT visible from hand, can't be collisions
						//it's not worth to calculate VisibleFrom with every Collider of the nearInteractables
						if (HandsVR[i].AttachedGrabbable.Blocked)// || !nearInteractables[j].transform.VisibleFrom(HandsVR[i].transform.position, DataVR.Instance.hand.blockingElements, QueryTriggerInteraction.Ignore))
						{
							PhysicsExt.IgnoreCollisions(nearInteractables[j].Colliders, HandsVR[i].AttachedGrabbable.Colliders, true);
						}
						else
						{
							PhysicsExt.IgnoreCollisions(nearInteractables[j].Colliders, HandsVR[i].AttachedGrabbable.Colliders, false);
						}
					}
				}
			}
		}

		/// <summary>
		/// Make interactable be able to collide to nearInteractables.
		/// This is done to a interactable (grabbable) when it is detached.
		/// </summary>
		/// <param name="interactable"></param>
		public void FreeCollisionIgnores(Interactable interactable)
		{
			if (interactable == null) return;

			int length = nearInteractables.Count;
			for (int i = 0; i < length; i++)
			{
				PhysicsExt.IgnoreCollisions(nearInteractables[i].Colliders, interactable.Colliders, false);
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Debug.DrawRay(transform.position, transform.forward, Color.red);
		}
#endif
	}
}