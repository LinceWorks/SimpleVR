// Copyright 2018 Andrew Maneri 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Related to article: http://www.yarsoft.com/2018/05/13/when-vr-hands-hit-digital-objects-believable-physics-conversion/

using UnityEngine;

public static class SpeedToForce
{
	/// <summary>
	/// Simple scalar for m/s into Newtons.  This value will make the average adult hit with a force of around 4500 newtons
	/// </summary>
	const float k_SpeedToForce = 10f; //740.0f;

	/// <summary>
	/// How long a 'collision' in this case is imparting force onto a target object
	/// </summary>
	const float k_CollisionTimeStep = 0.125f;

	/// <summary>
	/// Converts an impact velocity into a force that can be directly applied to a rigidbody with believable results
	/// </summary>
	/// <param name="impactVelocity">How fast the object was moving before it collided with a target</param>
	/// <param name="targetMass">How much mass the target object contains (rigidbody.mass) </param>
	/// <returns>A force that can be fed directly into RigidBody.AddForceAtPosition</returns>
	public static Vector3 SpeedToCollisionForce(Vector3 impactVelocity, float targetMass)
	{
		// For lighter objects will reach a speed where the impacting object is no longer colliding, we estimate that here
		var timeApplied = targetMass / (k_SpeedToForce * k_CollisionTimeStep);

		// We then adjust the total force imparted by this reduced time step
		var forceMultiplier = k_SpeedToForce * Mathf.Min(k_CollisionTimeStep, timeApplied);

		// The impacting object will be delivering less force at the end of the impact, so we also estimate the impact of that effect here
		// In the case where where the target object accelerates away, we average the starting and ending forces (full and zero) to get .5
		if (timeApplied < k_CollisionTimeStep)
		{
			forceMultiplier *= 0.5f;
		}
		else
		{
			forceMultiplier *= 1.0f - (k_CollisionTimeStep / (timeApplied * 2.0f));
		}

		return impactVelocity * forceMultiplier;
	}
}