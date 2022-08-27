using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using System;

public class RangePostProcessingTransition : PostProcessingTransition
{
	[Serializable]
	public class Config
	{
		[Tooltip ("The closer the player gets to this object, the stronger we apply the newProfile. It should be a child of this object and have (1,1,1) scale")]
		public Transform target;
		[Tooltip ("If you set this, all colliders in target and its children will be destroyed and I will make a sphere collider with this much range. Leave 0 to use your own colliders")]
		// This will still end up using colliders, we just add them at spawn
		public float maxRange;
		[Tooltip ("If the player is less than this distance away from the target, we just set the newProfile on him")]
		public float minRange = 1.0f;
	}

	public Config config;

	protected override void Awake ()
	{
		base.Awake ();
		if (config.target == null) {
			config.target = transform;
		}
		if (config.maxRange != 0) {
			// Destroy all colliders
			foreach (Collider undesired in config.target.GetComponentsInChildren <Collider>()) {
				Destroy (undesired);
			}
			// Create sphere collider
			SphereCollider sphere = config.target.gameObject.AddComponent <SphereCollider> ();
			// Set range
			sphere.radius = config.maxRange;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		PostProcessingBehaviour ppBeh = other.GetComponentInChildren <PostProcessingBehaviour> ();
		if (ppBeh != null && ppBeh.profile != null) {
			RangeUpdatableProfile rangeProf = ppBeh.gameObject.AddComponent <RangeUpdatableProfile> ();
			// I use the collision position as maxRange - may want to change this
			// This may exhibit undesired behaviour in strange colliders combos. RangeUpdateProfile stopsLerping once outside of this maxRange.
			// but PostProcTransition will also stopLerping if it exits the collider. CAREFUL!
			float maxRange = Vector3.Distance (other.transform.position, config.target.position) + 0.5f; // account for small variations
			rangeProf.LerpOverDistTo (ppBeh, futureProfile, config.target.position, config.minRange, maxRange);
			state.behavioursInTransit.Add (rangeProf);
		}
	}
}
