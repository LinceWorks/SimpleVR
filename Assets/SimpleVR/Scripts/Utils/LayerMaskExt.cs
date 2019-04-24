using UnityEngine;

namespace SimpleVR.Utils
{
	public static class LayerMaskExt
	{
		public static bool Contains(this LayerMask layerMask, int layer)
		{
			return (layerMask == (layerMask | (1 << layer)));
		}
	}
}
