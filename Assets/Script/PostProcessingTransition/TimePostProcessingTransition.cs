using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using System;

public class TimePostProcessingTransition : PostProcessingTransition
{
	[Serializable]
	public class Config
	{
		public float timeForTransition = 3.0f;
	}

	public Config config;

	void OnTriggerEnter (Collider other)
	{
		PostProcessingBehaviour ppBeh = other.GetComponentInChildren <PostProcessingBehaviour> ();
		if (ppBeh != null && ppBeh.profile != null) {
			TimeUpdatableProfile timeProf = ppBeh.gameObject.AddComponent <TimeUpdatableProfile> ();
			timeProf.LerpOverTimeTo (ppBeh, futureProfile, config.timeForTransition);
			state.behavioursInTransit.Add (timeProf);
		}
	}
}
