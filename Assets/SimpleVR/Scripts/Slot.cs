using UnityEngine;

namespace SimpleVR
{
	public class Slot : Interactable
	{
		#region Fields
		[SerializeField] private AttachmentTypeName slotType = null;	//Slots only admit grabbable with same SlotType
		[SerializeField] private Grabbable initialGrabbable = null;	//Initial grabbable to start with. If SlotType is not correct it won't initialize anything 
		#endregion

		#region Properties
		public AttachmentTypeName SlotType { get { return slotType; } }
		public Grabbable AttachedGrabbable { get; private set; }
		#endregion

		protected override void Awake()
		{
			base.Awake();

			TrySpawnInitialGrabbable();
		}

		public bool TrySpawnInitialGrabbable()
		{
			if (initialGrabbable == null || initialGrabbable.SlotType != SlotType) return false;

			Store(Instantiate(initialGrabbable.gameObject).GetComponent<Grabbable>());

			return true;
		}

		public void Store(Grabbable grabbable, CharacterVR characterVR = null)
		{
			grabbable.Rigidbody.isKinematic = true;
			grabbable.Transform.parent = transform;
			Vector3 attachPositionOffset = Vector3.zero;
			Quaternion attachRotationOffset = Quaternion.identity;
			grabbable.GetLocalAttachmentPositionAndRotation(transform, out attachPositionOffset, out attachRotationOffset, SlotType.name);
			grabbable.Transform.localPosition = -attachPositionOffset;
			grabbable.Transform.localRotation = Quaternion.Inverse(attachRotationOffset);

			if(!DataVR.Instance.grabbable.colliderEnabledWhenStored)
			{
				for (int i = 0; i < grabbable.Colliders.Length; i++)
				{
					grabbable.Colliders[i].enabled = false;
				}
				if(characterVR) characterVR.RemoveNearInteractables(grabbable);   //if fromInitialSpawn to avoid null reference remove on RemoveNearInteractables
			}

			AttachedGrabbable = grabbable;
			AttachedGrabbable.SetOwnerSlot(this);
		}

		public bool Unstore(Grabbable grabbable)
		{
			if (AttachedGrabbable != grabbable) return false;

			Unstore();
			return true;
		}

		public void Unstore()
		{
			for (int i = 0; i < AttachedGrabbable.Colliders.Length; i++)
			{
				AttachedGrabbable.Colliders[i].enabled = true;
			}

			AttachedGrabbable = null;
		}

		public override void Interact(HandVR handVR)
		{
			base.Interact(handVR);

			if (handVR.GrabDown() && AttachedGrabbable)
			{
				AttachedGrabbable.Attach(handVR);
			}
		}

		private void OnValidate()
		{
			if (Ethereal) Ethereal = false;
			if (!ConstrainedDetection) ConstrainedDetection = true;
			if (Undetectable) Undetectable = false;
		}
	}
}
