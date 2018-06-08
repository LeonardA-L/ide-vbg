using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessingLerper : MonoBehaviour {
    public PostProcessingBehaviour ppBehaviour;
    public PostProcessingProfile profileA;
    public PostProcessingProfile profileB;

    //private AmbientOcclusionModel.Settings ambOccSettings;
    //private MotionBlurModel.Settings motionBlurSettings;
    //private BloomModel.Settings bloomSettings;
    private ColorGradingModel.Settings colorSettings;
    //private GrainModel.Settings grainSettings;
    //private VignetteModel.Settings vignetteSettings;

    public bool active = false;
    public float lerp = 0.1f;
	
	// Update is called once per frame
	void Update () {
        if (!active)
            return;


        colorSettings.basic.postExposure = Mathf.Lerp(colorSettings.basic.postExposure, profileB.colorGrading.settings.basic.postExposure, lerp);
        colorSettings.basic.temperature = Mathf.Lerp(colorSettings.basic.temperature, profileB.colorGrading.settings.basic.temperature, lerp);
        colorSettings.basic.saturation = Mathf.Lerp(colorSettings.basic.saturation, profileB.colorGrading.settings.basic.saturation, lerp);
        colorSettings.basic.contrast = Mathf.Lerp(colorSettings.basic.contrast, profileB.colorGrading.settings.basic.contrast, lerp);
        colorSettings.colorWheels.linear.lift = Color.Lerp(colorSettings.colorWheels.linear.lift, ppBehaviour.profile.colorGrading.settings.colorWheels.linear.lift, lerp);
        colorSettings.colorWheels.linear.gamma = Color.Lerp(colorSettings.colorWheels.linear.gamma, ppBehaviour.profile.colorGrading.settings.colorWheels.linear.gamma, lerp);
        colorSettings.colorWheels.linear.gain = Color.Lerp(colorSettings.colorWheels.linear.gain, ppBehaviour.profile.colorGrading.settings.colorWheels.linear.gain, lerp);
        //colorSettings.channelMixer.red

        var settings = ppBehaviour.profile.colorGrading.settings;
        settings.basic = colorSettings.basic;
        settings.colorWheels = colorSettings.colorWheels;
        ppBehaviour.profile.colorGrading.settings = settings;
    }

    public void StartLerp()
    {
        //ambOccSettings = profileA.ambientOcclusion.settings;
        //motionBlurSettings = profileA.motionBlur.settings;
        //bloomSettings = profileA.bloom.settings;
        colorSettings = profileA.colorGrading.settings;
        //grainSettings = profileA.grain.settings;
        //vignetteSettings = profileA.vignette.settings;

        active = true;
    }

    public void Stop()
    {
        active = false;
    }
}
