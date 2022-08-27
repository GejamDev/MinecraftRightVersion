using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public abstract class PostProcessingTransition : MonoBehaviour
{
	// Inspector fields
	[Tooltip ("The profile to switch to")]
	public PostProcessingProfile futureProfile;

	public class State
	{
		public List<UpdatableProfile> behavioursInTransit = new List<UpdatableProfile> ();
	}

	protected State state;

	protected virtual void Awake ()
	{
		state = new State ();
	}

	protected virtual void OnTriggerExit (Collider other)
	{
		if (state.behavioursInTransit.Count == 0) {
			return;
		}

		UpdatableProfile updProf = other.GetComponentInChildren <UpdatableProfile> ();
		if (updProf != null && state.behavioursInTransit.Contains (updProf)) {
			updProf.StopLerping ();
		}
	}
}

