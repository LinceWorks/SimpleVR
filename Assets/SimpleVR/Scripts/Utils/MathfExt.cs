using UnityEngine;

namespace SimpleVR.Utils
{
	public static class MathfExt
{
	public static Vector3 Abs(this Vector3 vector)
	{
		return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
	}
}
}
