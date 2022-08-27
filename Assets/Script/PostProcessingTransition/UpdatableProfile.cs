using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// 
/// I've commented out all the things I'm not handling yet (textures, bools)
/// </summary>
public abstract class UpdatableProfile : MonoBehaviour
{
	/// <summary>
	/// The original profile used on the behaviour
	/// </summary>
	protected PostProcessingProfile oldProfile;

	/// <summary>
	/// The behaviour carrying the profiles we are changing
	/// </summary>
	protected PostProcessingBehaviour behaviour;

	/// <summary>
	/// The post processing profile used to move from the old to the future one
	/// </summary>
	protected PostProcessingProfile transitionProfile;

	/// <summary>
	/// The target profile. We will lerp until we get there, then set it on the behaviour
	/// </summary>
	protected PostProcessingProfile futureProfile;

	public void StopLerping ()
	{
		GameObject.Destroy (this);
	}

	protected abstract void Update ();

	protected void InitProfiles (PostProcessingBehaviour behaviour, PostProcessingProfile desiredState)
	{
		// Assign old and future profiles
		this.behaviour = behaviour;
		this.oldProfile = behaviour.profile;
		this.futureProfile = desiredState;

		// Create the transition profile as a copy of the old one
		this.transitionProfile = Object.Instantiate (this.oldProfile);
		this.transitionProfile.name = "Transition Profile";

		// Assign it to the camera
		this.behaviour.profile = transitionProfile;
	}


	// When we transition to a disabled profile, it's better if the float values are set to 0 (no effect)
	// So that the transition will be smooth
	protected void LerpProfiles (float rate)
	{
		if (oldProfile.ambientOcclusion.enabled || futureProfile.ambientOcclusion.enabled)
			LerpAO (rate);

		if (oldProfile.screenSpaceReflection.enabled || futureProfile.screenSpaceReflection.enabled)
			LerpSSR (rate);

		if (oldProfile.depthOfField.enabled || futureProfile.depthOfField.enabled)
			LerpDOF (rate);

		if (oldProfile.motionBlur.enabled || futureProfile.motionBlur.enabled)
			LerpBlur (rate);

		if (oldProfile.eyeAdaptation.enabled || futureProfile.eyeAdaptation.enabled)
			LerpEyeAdapt (rate);

		if (oldProfile.colorGrading.enabled || futureProfile.colorGrading.enabled)
			LerpColorGrading (rate);

		if (oldProfile.userLut.enabled || futureProfile.userLut.enabled)
			LerpLUT (rate);

		if (oldProfile.chromaticAberration.enabled || futureProfile.chromaticAberration.enabled)
			LerpChromAberration (rate);

		if (oldProfile.grain.enabled || futureProfile.grain.enabled)
			LerpGrain (rate);

		if (oldProfile.vignette.enabled || futureProfile.vignette.enabled)
			LerpVignette (rate);

		if (oldProfile.dithering.enabled || futureProfile.dithering.enabled)
			LerpDither (rate);
	}

	protected void LerpAO (float rate)
	{
		// AmbientOcclusion
		float AO_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.ambientOcclusion.enabled = EnableOrDisableEffect (oldProfile.ambientOcclusion.enabled, futureProfile.ambientOcclusion.enabled, rate, AO_ACTIVATION_LIMIT);

		AmbientOcclusionModel.Settings aoSettings = oldProfile.ambientOcclusion.settings;

		aoSettings.intensity = Mathf.Lerp (aoSettings.intensity, futureProfile.ambientOcclusion.settings.intensity, rate);
		aoSettings.radius = Mathf.Lerp (aoSettings.radius, futureProfile.ambientOcclusion.settings.radius, rate);

		transitionProfile.ambientOcclusion.settings = aoSettings;
	}

	protected void LerpSSR (float rate)
	{
		float SSR_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.screenSpaceReflection.enabled = EnableOrDisableEffect (oldProfile.screenSpaceReflection.enabled, futureProfile.screenSpaceReflection.enabled, rate, SSR_ACTIVATION_LIMIT);

		ScreenSpaceReflectionModel.Settings settings = oldProfile.screenSpaceReflection.settings;

		// Reflection

		ScreenSpaceReflectionModel.ReflectionSettings currentReflection = settings.reflection;
		ScreenSpaceReflectionModel.ReflectionSettings futureReflection = futureProfile.screenSpaceReflection.settings.reflection;
		currentReflection.maxDistance = Mathf.Lerp (currentReflection.maxDistance, futureReflection.maxDistance, rate);
		currentReflection.iterationCount = (int)Mathf.Lerp (currentReflection.iterationCount, futureReflection.iterationCount, rate);
		currentReflection.stepSize = (int)Mathf.Lerp (currentReflection.stepSize, futureReflection.stepSize, rate);
		currentReflection.widthModifier = Mathf.Lerp (currentReflection.widthModifier, futureReflection.widthModifier, rate);
		currentReflection.reflectionBlur = Mathf.Lerp (currentReflection.reflectionBlur, futureReflection.reflectionBlur, rate);
		settings.reflection = currentReflection;


		// Intensity

		ScreenSpaceReflectionModel.IntensitySettings currentIntensity = settings.intensity;
		ScreenSpaceReflectionModel.IntensitySettings futureIntensity = futureProfile.screenSpaceReflection.settings.intensity;
		currentIntensity.reflectionMultiplier = Mathf.Lerp (currentIntensity.reflectionMultiplier, futureIntensity.reflectionMultiplier, rate);
		currentIntensity.fadeDistance = Mathf.Lerp (currentIntensity.fadeDistance, futureIntensity.fadeDistance, rate);
		currentIntensity.fresnelFade = Mathf.Lerp (currentIntensity.fresnelFade, futureIntensity.fresnelFade, rate);
		currentIntensity.fresnelFadePower = Mathf.Lerp (currentIntensity.fresnelFadePower, futureIntensity.fresnelFadePower, rate);
		settings.intensity = currentIntensity;
			

		// Screen Edge Mask

		ScreenSpaceReflectionModel.ScreenEdgeMask screenEdgeMask = settings.screenEdgeMask;
		screenEdgeMask.intensity = Mathf.Lerp (screenEdgeMask.intensity, futureProfile.screenSpaceReflection.settings.screenEdgeMask.intensity, rate);
		settings.screenEdgeMask = screenEdgeMask;

		transitionProfile.screenSpaceReflection.settings = settings;
	}

	protected void LerpDOF (float rate)
	{
		float DOF_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.depthOfField.enabled = EnableOrDisableEffect (oldProfile.depthOfField.enabled, futureProfile.depthOfField.enabled, rate, DOF_ACTIVATION_LIMIT);

		// All settings

		DepthOfFieldModel.Settings currentSettings = oldProfile.depthOfField.settings;
		DepthOfFieldModel.Settings futureSettings = futureProfile.depthOfField.settings;

		currentSettings.focusDistance = Mathf.Lerp (currentSettings.focusDistance, futureSettings.focusDistance, rate);
		currentSettings.aperture = Mathf.Lerp (currentSettings.aperture, futureSettings.aperture, rate);
		currentSettings.focalLength = Mathf.Lerp (currentSettings.focalLength, futureSettings.focalLength, rate);

		transitionProfile.depthOfField.settings = currentSettings;
	}

	protected void LerpBlur (float rate)
	{
		float BLUR_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.motionBlur.enabled = EnableOrDisableEffect (oldProfile.motionBlur.enabled, futureProfile.motionBlur.enabled, rate, BLUR_ACTIVATION_LIMIT);

		MotionBlurModel.Settings currentSettings = oldProfile.motionBlur.settings;
		MotionBlurModel.Settings futureSettings = futureProfile.motionBlur.settings;

		currentSettings.shutterAngle = Mathf.Lerp (currentSettings.shutterAngle, futureSettings.shutterAngle, rate);
		currentSettings.sampleCount = (int)Mathf.Lerp (currentSettings.sampleCount, futureSettings.sampleCount, rate);
		currentSettings.frameBlending = Mathf.Lerp (currentSettings.frameBlending, futureSettings.frameBlending, rate);

		transitionProfile.motionBlur.settings = currentSettings;
	}

	protected void LerpEyeAdapt (float rate)
	{
		float EYE_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.eyeAdaptation.enabled = EnableOrDisableEffect (oldProfile.eyeAdaptation.enabled, futureProfile.eyeAdaptation.enabled, rate, EYE_ACTIVATION_LIMIT);

		EyeAdaptationModel.Settings currentSettings = oldProfile.eyeAdaptation.settings;
		EyeAdaptationModel.Settings futureSettings = futureProfile.eyeAdaptation.settings;

		// Luminosity Range

		currentSettings.logMin = (int)Mathf.Lerp (currentSettings.logMin, futureSettings.logMin, rate);
		currentSettings.logMax = (int)Mathf.Lerp (currentSettings.logMax, futureSettings.logMax, rate);

		// Auto Exposure

		currentSettings.lowPercent = Mathf.Lerp (currentSettings.lowPercent, futureSettings.lowPercent, rate);
		currentSettings.highPercent = Mathf.Lerp (currentSettings.highPercent, futureSettings.highPercent, rate);
		currentSettings.minLuminance = Mathf.Lerp (currentSettings.minLuminance, futureSettings.minLuminance, rate);
		currentSettings.maxLuminance = Mathf.Lerp (currentSettings.maxLuminance, futureSettings.maxLuminance, rate);
//		Lerp(currentSettings.dynamicKeyValue)

		// Adaptation
//		Lerp(currentSettings.adaptationType)
		currentSettings.speedUp = Mathf.Lerp (currentSettings.speedUp, futureSettings.speedUp, rate);
		currentSettings.speedDown = Mathf.Lerp (currentSettings.speedDown, futureSettings.speedDown, rate);

		transitionProfile.eyeAdaptation.settings = currentSettings;
	}

	protected void LerpBloom (float rate)
	{
		float BLOOM_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.bloom.enabled = EnableOrDisableEffect (oldProfile.bloom.enabled, futureProfile.bloom.enabled, rate, BLOOM_ACTIVATION_LIMIT);

		BloomModel.Settings settings = oldProfile.bloom.settings;


		// Bloom

		BloomModel.BloomSettings currentBloom = settings.bloom;
		BloomModel.BloomSettings futureBloom = futureProfile.bloom.settings.bloom;
		currentBloom.intensity = Mathf.Lerp (currentBloom.intensity, futureBloom.intensity, rate);
		currentBloom.threshold = Mathf.Lerp (currentBloom.threshold, futureBloom.threshold, rate);
		currentBloom.softKnee = Mathf.Lerp (currentBloom.softKnee, futureBloom.softKnee, rate);
		currentBloom.radius = Mathf.Lerp (currentBloom.radius, futureBloom.radius, rate);
//		currentSettings.antiFlicker = ;
		settings.bloom = currentBloom;


		// Lens Dirt

		BloomModel.LensDirtSettings currentLensDirt = settings.lensDirt;
//		currentLensDirt.texture = ; 
		currentLensDirt.intensity = Mathf.Lerp (currentLensDirt.intensity, futureProfile.bloom.settings.lensDirt.intensity, rate);
		settings.lensDirt = currentLensDirt;

		transitionProfile.bloom.settings = settings;
	}

	protected void LerpColorGrading (float rate)
	{
		float CG_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.colorGrading.enabled = EnableOrDisableEffect (oldProfile.colorGrading.enabled, futureProfile.colorGrading.enabled, rate, CG_ACTIVATION_LIMIT);

		ColorGradingModel.Settings settings = oldProfile.colorGrading.settings;

		ColorGradingModel.TonemappingSettings currentTone = settings.tonemapping;
		ColorGradingModel.TonemappingSettings futureTone = futureProfile.colorGrading.settings.tonemapping;
		ColorGradingModel.BasicSettings currentBasic = settings.basic;
		ColorGradingModel.BasicSettings futureBasic = futureProfile.colorGrading.settings.basic;
		ColorGradingModel.ColorWheelsSettings currentWheel = settings.colorWheels;
		ColorGradingModel.ColorWheelsSettings futureWheel = futureProfile.colorGrading.settings.colorWheels;

		// TONEMAPPING

		currentTone.neutralBlackIn = Mathf.Lerp (currentTone.neutralBlackIn, futureTone.neutralBlackIn, rate);
		currentTone.neutralWhiteIn = Mathf.Lerp (currentTone.neutralWhiteIn, futureTone.neutralWhiteIn, rate);
		currentTone.neutralBlackOut = Mathf.Lerp (currentTone.neutralBlackOut, futureTone.neutralBlackOut, rate);
		currentTone.neutralWhiteOut = Mathf.Lerp (currentTone.neutralWhiteOut, futureTone.neutralWhiteOut, rate);
		currentTone.neutralWhiteLevel = Mathf.Lerp (currentTone.neutralWhiteLevel, futureTone.neutralWhiteLevel, rate);
		currentTone.neutralWhiteClip = Mathf.Lerp (currentTone.neutralWhiteClip, futureTone.neutralWhiteClip, rate);

		// BASIC

		currentBasic.postExposure = Mathf.Lerp (currentBasic.postExposure, futureBasic.postExposure, rate);
		currentBasic.temperature = Mathf.Lerp (currentBasic.temperature, futureBasic.temperature, rate);
		currentBasic.tint = Mathf.Lerp (currentBasic.tint, futureBasic.tint, rate);
		currentBasic.hueShift = Mathf.Lerp (currentBasic.hueShift, futureBasic.hueShift, rate);
		currentBasic.saturation = Mathf.Lerp (currentBasic.saturation, futureBasic.saturation, rate);
		currentBasic.contrast = Mathf.Lerp (currentBasic.contrast, futureBasic.contrast, rate);

		// COLOR WHEELS

		currentWheel.log.slope = Color.Lerp (currentWheel.log.slope, futureWheel.log.slope, rate);
		currentWheel.log.power = Color.Lerp (currentWheel.log.power, futureWheel.log.power, rate);
		currentWheel.log.offset = Color.Lerp (currentWheel.log.offset, futureWheel.log.offset, rate);
		currentWheel.linear.lift = Color.Lerp (currentWheel.linear.lift, futureWheel.linear.lift, rate);
		currentWheel.linear.gamma = Color.Lerp (currentWheel.linear.gamma, futureWheel.linear.gamma, rate);
		currentWheel.linear.gain = Color.Lerp (currentWheel.linear.gain, futureWheel.linear.gain, rate);

		settings.tonemapping = currentTone;
		settings.basic = currentBasic;
		settings.colorWheels = currentWheel;

		transitionProfile.colorGrading.settings = settings;
	}

	protected void LerpLUT (float rate)
	{
		float LUT_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.userLut.enabled = EnableOrDisableEffect (oldProfile.userLut.enabled, futureProfile.userLut.enabled, rate, LUT_ACTIVATION_LIMIT);

		UserLutModel.Settings currentSettings = oldProfile.userLut.settings;
		UserLutModel.Settings futureSettings = futureProfile.userLut.settings;

//		settings.lut = ;
		currentSettings.contribution = Mathf.Lerp (currentSettings.contribution, futureSettings.contribution, rate);

		transitionProfile.userLut.settings = currentSettings;
	}

	protected void LerpChromAberration (float rate)
	{
		float CHROM_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.chromaticAberration.enabled = EnableOrDisableEffect (oldProfile.chromaticAberration.enabled, futureProfile.chromaticAberration.enabled, rate, CHROM_ACTIVATION_LIMIT);

		ChromaticAberrationModel.Settings currentSettings = oldProfile.chromaticAberration.settings;
		ChromaticAberrationModel.Settings futureSettings = futureProfile.chromaticAberration.settings;

//		currentSettings.spectralTexture = ;
		currentSettings.intensity = Mathf.Lerp (currentSettings.intensity, futureSettings.intensity, rate);

		transitionProfile.chromaticAberration.settings = currentSettings;
	}

	protected void LerpGrain (float rate)
	{
		float GRAIN_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.grain.enabled = EnableOrDisableEffect (oldProfile.grain.enabled, futureProfile.grain.enabled, rate, GRAIN_ACTIVATION_LIMIT);

		GrainModel.Settings currentSettings = oldProfile.grain.settings;
		GrainModel.Settings futureSettings = futureProfile.grain.settings;

		currentSettings.intensity = Mathf.Lerp (currentSettings.intensity, futureSettings.intensity, rate);
		currentSettings.luminanceContribution = Mathf.Lerp (currentSettings.luminanceContribution, futureSettings.luminanceContribution, rate);
		currentSettings.size = Mathf.Lerp (currentSettings.size, futureSettings.size, rate);
//		currentSettings.colored = ;

		transitionProfile.grain.settings = currentSettings;
	}

	protected void LerpVignette (float rate)
	{
		float VIGN_ACTIVATION_LIMIT = 0.05f;

		transitionProfile.vignette.enabled = EnableOrDisableEffect (oldProfile.vignette.enabled, futureProfile.vignette.enabled, rate, VIGN_ACTIVATION_LIMIT);

		VignetteModel.Settings currentSettings = oldProfile.vignette.settings;
		VignetteModel.Settings futureSettings = futureProfile.vignette.settings;

//		currentSettings.mode = ;
		currentSettings.color = Color.Lerp (currentSettings.color, futureSettings.color, rate);
		currentSettings.center = Vector2.Lerp (currentSettings.center, futureSettings.center, rate);
		currentSettings.intensity = Mathf.Lerp (currentSettings.intensity, futureSettings.intensity, rate);
		currentSettings.smoothness = Mathf.Lerp (currentSettings.smoothness, futureSettings.smoothness, rate);
		currentSettings.roundness = Mathf.Lerp (currentSettings.roundness, futureSettings.roundness, rate);
//		currentSettings.rounded = ;

		transitionProfile.vignette.settings = currentSettings;
	}

	protected void LerpDither (float rate)
	{
		// Nothing to configure!
	}

	protected bool EnableOrDisableEffect (bool oldEnabled, bool futureEnabled, float rate, float activationLimit)
	{
		if (oldEnabled == futureEnabled)
			return futureEnabled;
		else if (futureEnabled && rate > activationLimit) {
			return true;
		} else if (!futureEnabled && rate > 1 - activationLimit) {
			return false;
		}

		return futureEnabled;
	}
}



