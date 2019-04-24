using UnityEngine;

namespace SimpleVR.Utils
{
	public struct TimeSince
{
	float time;

	public static implicit operator float(TimeSince ts)
	{
		return Time.time - ts.time;
	}

	public static implicit operator TimeSince(float ts)
	{
		return new TimeSince { time = Time.time - ts };
	}
}
}
