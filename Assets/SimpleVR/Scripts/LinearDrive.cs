//Modification of LinearDrive of Valve.VR.InteractionSystem for SimpleVR

//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Drives a linear mapping based on position between 2 positions
//
//=============================================================================

using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections;

namespace SimpleVR
{
	//-------------------------------------------------------------------------
	public class LinearDrive : Interactable
	{
		public Transform startPosition;
		public Transform endPosition;
		public LinearMapping linearMapping;
		public bool repositionGameObject = true;
		public bool maintainMomemntum = true;
		public bool autoReturn = false;
		public float momemtumDampenRate = 5.0f;

		private float initialMappingOffset;
		private int numMappingChangeSamples = 5;
		private float[] mappingChangeSamples;
		private float prevMapping = 0.0f;
		private float mappingChangeRate;
		private int sampleCount = 0;

		private bool interacting = false;

		public bool Interacting
		{
			get
			{
				return interacting;
			}

			set
			{
				interacting = value;

				if (value) StopAllCoroutines();
				else if (autoReturn) StartCoroutine(Inertia(() =>
				{
					StartCoroutine(Return());
				}));
				else StartCoroutine(Inertia());
			}
		}



		//-------------------------------------------------
		protected override void Awake()
		{
			base.Awake();
			mappingChangeSamples = new float[numMappingChangeSamples];

			LinearDisplacement ld = linearMapping.GetComponent<LinearDisplacement>();
			if(ld) ld.displacement = endPosition.localPosition - startPosition.localPosition;
		}


		//-------------------------------------------------
		void Start()
		{
			if (linearMapping == null)
			{
				linearMapping = GetComponent<LinearMapping>();
			}

			if (linearMapping == null)
			{
				linearMapping = gameObject.AddComponent<LinearMapping>();
			}

			initialMappingOffset = linearMapping.value;

			if (repositionGameObject)
			{
				UpdateLinearMapping(transform);
			}
		}

		//don't like it
		//public override void Hover(HandVR handVR, bool affectChildren = true)
		//{
		//	base.Hover(handVR, affectChildren);

		//	if (handVR.Grab())
		//	{
		//		Debug.Log("down");
		//		initialMappingOffset = linearMapping.value - CalculateLinearMapping(handVR.transform);
		//		sampleCount = 0;
		//		mappingChangeRate = 0.0f;

		//		Interacting = true;
		//	}
		//}

		public override void Unhover(HandVR handVR, bool affectChildren = true)
		{
			base.Unhover(handVR, affectChildren);

			if (!Interacting || hoverHands.Count != 0) return;

			CalculateMappingChangeRate();

			Interacting = false;
		}

		public override void Interact(HandVR handVR)
		{
			base.Interact(handVR);

			if (handVR.GrabDown())
			{
				initialMappingOffset = linearMapping.value - CalculateLinearMapping(handVR.transform);
				sampleCount = 0;
				mappingChangeRate = 0.0f;

				Interacting = true;
			}

			if (handVR.GrabUp())
			{
				CalculateMappingChangeRate();

				Interacting = false;
			}

			if (handVR.Grab())
			{
				if (Interacting) UpdateLinearMapping(handVR.transform);
			}
		}

		#region unusedcode from SteamVR plugin
		////-------------------------------------------------
		//private void HandHoverUpdate(Hand hand)
		//{
		//	if (hand.GetStandardInteractionButtonDown())
		//	{
		//		hand.HoverLock(GetComponent<Interactable>());

		//		initialMappingOffset = linearMapping.value - CalculateLinearMapping(hand.transform);
		//		sampleCount = 0;
		//		mappingChangeRate = 0.0f;

		//		interacting = true;
		//	}

		//	if (hand.GetStandardInteractionButtonUp())
		//	{
		//		hand.HoverUnlock(GetComponent<Interactable>());

		//		CalculateMappingChangeRate();

		//		interacting = false;
		//	}

		//	if (hand.GetStandardInteractionButton())
		//	{
		//		if (interacting) UpdateLinearMapping(hand.transform);
		//	}
		//	else
		//	{
		//		interacting = false;
		//	}
		//}
		#endregion

		//-------------------------------------------------
		private void CalculateMappingChangeRate()
		{
			//Compute the mapping change rate
			mappingChangeRate = 0.0f;
			int mappingSamplesCount = Mathf.Min(sampleCount, mappingChangeSamples.Length);
			if (mappingSamplesCount != 0)
			{
				for (int i = 0; i < mappingSamplesCount; ++i)
				{
					mappingChangeRate += mappingChangeSamples[i];
				}
				mappingChangeRate /= mappingSamplesCount;
			}
		}


		//-------------------------------------------------
		private void UpdateLinearMapping(Transform tr)
		{
			prevMapping = linearMapping.value;
			linearMapping.value = Mathf.Clamp01(initialMappingOffset + CalculateLinearMapping(tr));

			mappingChangeSamples[sampleCount % mappingChangeSamples.Length] = (1.0f / Time.deltaTime) * (linearMapping.value - prevMapping);
			sampleCount++;

			if (repositionGameObject)
			{
				transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
			}
		}


		//-------------------------------------------------
		private float CalculateLinearMapping(Transform tr)
		{
			Vector3 direction = endPosition.position - startPosition.position;
			float length = direction.magnitude;
			direction.Normalize();

			Vector3 displacement = tr.position - startPosition.position;

			return Vector3.Dot(displacement, direction) / length;
		}

		#region unusedcode from SteamVR plugin
		////-------------------------------------------------
		//void Update()
		//{
		//	if (maintainMomemntum && mappingChangeRate != 0.0f)
		//	{
		//		//Dampen the mapping change rate and apply it to the mapping
		//		mappingChangeRate = Mathf.Lerp(mappingChangeRate, 0.0f, momemtumDampenRate * Time.deltaTime);
		//		linearMapping.value = Mathf.Clamp01(linearMapping.value + (mappingChangeRate * Time.deltaTime));

		//		if (repositionGameObject)
		//		{
		//			transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
		//		}
		//	}
		//}
		#endregion

		private IEnumerator Inertia()
		{
			while (maintainMomemntum && (mappingChangeRate > 0.001f || mappingChangeRate < -0.001f))
			{
				//Dampen the mapping change rate and apply it to the mapping
				mappingChangeRate = Mathf.Lerp(mappingChangeRate, 0.0f, momemtumDampenRate * Time.deltaTime);
				linearMapping.value = Mathf.Clamp01(linearMapping.value + (mappingChangeRate * Time.deltaTime));

				if (repositionGameObject)
				{
					transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
				}
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator Inertia(System.Action CallBack)
		{
			while (maintainMomemntum && (mappingChangeRate > 0.001f || mappingChangeRate < -0.001f))
			{
				//Dampen the mapping change rate and apply it to the mapping
				mappingChangeRate = Mathf.Lerp(mappingChangeRate, 0.0f, momemtumDampenRate * Time.deltaTime);
				linearMapping.value = Mathf.Clamp01(linearMapping.value + (mappingChangeRate * Time.deltaTime));

				if (repositionGameObject)
				{
					transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
				}
				yield return new WaitForEndOfFrame();
			}

			CallBack();
		}

		private IEnumerator Return()
		{
			while (maintainMomemntum && mappingChangeRate > -0.999f)
			{
				//Dampen the mapping change rate and apply it to the mapping
				mappingChangeRate = Mathf.Lerp(mappingChangeRate, -1f, momemtumDampenRate * Time.deltaTime);
				linearMapping.value = Mathf.Clamp01(linearMapping.value + (mappingChangeRate * Time.deltaTime));

				if (repositionGameObject)
				{
					transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);
				}
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
