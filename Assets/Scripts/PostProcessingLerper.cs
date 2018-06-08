using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessingLerper : MonoBehaviour {
    public PostProcessingBehaviour ppBehaviour;
    public PostProcessingProfile profileA;
    public PostProcessingProfile profileB;

    private AmbientOcclusionModel.Settings ambOccSettings;
    private MotionBlurModel.Settings motionBlurSettings;
    private BloomModel.Settings bloomSettings;
    private ColorGradingModel.Settings colorSettings;
    private GrainModel.Settings grainSettings;
    private VignetteModel.Settings vignetteSettings;

    private bool active = false;
    public float lerp = 0.1f;
	
	// Update is called once per frame
	void Update () {
        if (!active)
            return;


        ambOccSettings.intensity = Mathf.Lerp(ambOccSettings.intensity, profileB.ambientOcclusion.settings.intensity, lerp);
        ambOccSettings.radius = Mathf.Lerp(ambOccSettings.radius, profileB.ambientOcclusion.settings.radius, lerp);
        ambOccSettings.radius = Mathf.Lerp(ambOccSettings.radius, profileB.ambientOcclusion.settings.radius, lerp);
    }

    public void StartLerp()
    {
        ambOccSettings = profileA.ambientOcclusion.settings;
        motionBlurSettings = profileA.motionBlur.settings;
        bloomSettings = profileA.bloom.settings;
        colorSettings = profileA.colorGrading.settings;
        grainSettings = profileA.grain.settings;
        vignetteSettings = profileA.vignette.settings;

        active = true;
    }

    public void Stop()
    {
        active = false;
    }
}
