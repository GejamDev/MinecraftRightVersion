using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using System;

class RangeUpdatableProfile : UpdatableProfile
{

	[Serializable]
	public class RangeLerp
	{
		public Vector3 destination;
		public float minRange;
		public float maxRange;
		public float rate = 0f;
	}

	RangeLerp rangeLerp = new RangeLerp ();

	public void LerpOverDistTo (PostProcessingBehaviour behaviour, PostProcessingProfile desiredState, Vector3 destination, float minRange, float maxRange)
	{
		InitProfiles (behaviour, desiredState);
		rangeLerp.destination = destination;
		rangeLerp.minRange = minRange;
		rangeLerp.maxRange = maxRange;
	}


	protected override void Update ()
	{
		float dist = Vector3.Distance (transform.position, rangeLerp.destination);
		if (dist < rangeLerp.minRange || dist > rangeLerp.maxRange) {
			StopLerping ();
		} else {
			float rate = 1 - dist / rangeLerp.maxRange;
			LerpProfiles (rate);
		}


	}
}

