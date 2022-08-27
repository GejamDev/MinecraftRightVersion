using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// Object enters trigger zone (e.g timebased)
/// OnTriggerEnter we addComponent this script on him
/// 
/// </summary>
public class TimeUpdatableProfile : UpdatableProfile
{
	public class TimeLerp
	{
		public float totalTime;
		public float rate = 0f;

		public void Init (float totalTime)
		{
			this.totalTime = totalTime;
			rate = 0f;
		}

		public void Update ()
		{
			rate += Time.deltaTime / totalTime;
		}
	}

	TimeLerp timeLerp = new TimeLerp ();

	public void LerpOverTimeTo (PostProcessingBehaviour behaviour, PostProcessingProfile desiredState, float totalTime)
	{
		InitProfiles (behaviour, desiredState);

		timeLerp.Init (totalTime);
	}

	protected override void Update ()
	{
		timeLerp.Update ();
		if (timeLerp.rate < 1.0f) {
			LerpProfiles (timeLerp.rate);
		} else {
			behaviour.profile = futureProfile;
			StopLerping ();
		}
	}
}


