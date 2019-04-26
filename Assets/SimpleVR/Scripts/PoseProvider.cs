using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.Experimental.XR.Interaction;

namespace SimpleVR
{
	public class PoseProvider : BasePoseProvider
	{
		private Vector3 attachPositionOffset = Vector3.zero;
		private Quaternion attachRotationOffset = Quaternion.identity;

		public Vector3 AttachPositionOffset { get { return attachPositionOffset; } set { attachPositionOffset = value; } }
		public Quaternion AttachRotationOffset { get { return attachRotationOffset; } set { attachRotationOffset = value; } }
		public Vector3 AttachEulerAnglesOffset { get { return AttachRotationOffset.eulerAngles; } set { AttachRotationOffset = Quaternion.Euler(value); } }

		public Vector3 AttachPosition { get { return transform.TransformPoint(Quaternion.Inverse(AttachRotationOffset) * -AttachPositionOffset); } }
		public Quaternion AttachRotation { get { return transform.rotation * Quaternion.Inverse(AttachRotationOffset); } }

		/// <summary>
		/// Useful to attach Grabbables
		/// Something can track it with UnityEngine.SpatialTracking.TrackedPoseDriver to follow position and rotation.
		/// More accurate than UnityEngine.Animations Constraints classes and maybe more than kinematic parenting.
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public override PoseDataFlags GetPoseFromProvider(out Pose output)
		{
			output = new Pose(AttachPosition, AttachRotation);
			//output = new Pose(transform.TransformPoint(attachPositionOffset), transform.rotation * attachRotationOffset);
			return PoseDataFlags.Position | PoseDataFlags.Rotation;
		}
	}
}
