using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
public class ProcessingController : MonoBehaviour
{
    [SerializeField] AnimationCurve hitEffectCurve  = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] AnimationCurve flashEffectCurve;

    [SerializeField] Volume volume;
    Vignette vignette;
    ColorCurves colorCurves;
    LensDistortion lensDistortion;
    ChromaticAberration chromatic;
    MotionBlur motionBlur;
    DepthOfField dof;

    VolumeParameter<float> floatParameter;
    ColorParameter colorParameter;
    ClampedFloatParameter clamped;

    Coroutine hitCo;
    Coroutine flashCo;
    Coroutine chromaciCo;
    Coroutine motionCo;
    Coroutine blurCo;
    Coroutine lensCo;
    public void Start()
    {
        volume ??= FindObjectOfType<Volume>();
        VolumeProfile profile = volume.sharedProfile;

        if (!profile.TryGet<Vignette>(out vignette))
            vignette = profile.Add<Vignette>(false);
        if (!profile.TryGet<ColorCurves>(out colorCurves))
            colorCurves = profile.Add<ColorCurves>(false);
        if (!profile.TryGet<LensDistortion>(out lensDistortion))
            lensDistortion = profile.Add<LensDistortion>(false);
        if (!profile.TryGet<ChromaticAberration>(out chromatic))
            chromatic = profile.Add<ChromaticAberration>(false);
        if (!profile.TryGet<MotionBlur>(out motionBlur))
            motionBlur = profile.Add<MotionBlur>(false);
        if (!profile.TryGet<DepthOfField>(out dof))
            dof = profile.Add<DepthOfField>(false);

        floatParameter = new VolumeParameter<float>();
        colorParameter = new ColorParameter(Color.red);
        clamped = new ClampedFloatParameter(1,0,1,true);
        
        //FlashEffect();
    }


    public void HitEffect()
        => this.ReStartCoroutine(HitRoutine(), ref hitCo);
    public void FlashEffect()
        => this.ReStartCoroutine(FlashBangRoutine(), ref flashCo);



    void MotionBlur(AnimationCurve curve, float waitTime = 0)
        => this.ReStartCoroutine(MotionBlurRoutine(curve, waitTime), ref motionCo);//ReStartCoroutine(MotionBlurRoutine(curve, waitTime), ref motionCo);
    void Chromatic(AnimationCurve curve, float waitTime = 0)
        => this.ReStartCoroutine(ChromaticRoutine(curve, waitTime), ref chromaciCo);
    void Blur(AnimationCurve curve, float waitTime = 0)
        => this.ReStartCoroutine(BlurRoutine(curve, waitTime), ref blurCo);

    IEnumerator HitRoutine()
    {
        vignette.active = true;
        if(vignette.color.overrideState == false)
            vignette.color.overrideState = true;
        if(vignette.intensity.overrideState == false)
            vignette.intensity.overrideState = true;
        if(vignette.smoothness.overrideState == false)
            vignette.smoothness.overrideState = true;

        colorParameter.value = Color.red;
        clamped.value = 1;
        floatParameter.value = 0;
        float time = 0;
        float outTime = hitEffectCurve[hitEffectCurve.length - 1].time;
        vignette.color = colorParameter;
        vignette.smoothness = clamped;
        while (outTime >= time)
        {
            time += Time.deltaTime;
            floatParameter.value = hitEffectCurve.Evaluate(time);
            vignette.intensity.SetValue(floatParameter);
            yield return null;
        }
        vignette.active = false;
    }
    IEnumerator FlashBangRoutine()
    {
        Blur(flashEffectCurve, 0.4f);
        Chromatic(flashEffectCurve, 0.4f);
        MotionBlur(flashEffectCurve, 1);

        colorCurves.active = true;
        lensDistortion.active = true;
        
        VolumeParameter<float> volumeParameter = new VolumeParameter<float>();
        volumeParameter.value = 0;
        TextureCurveParameter textureCurveParameter= colorCurves.master;
        textureCurveParameter.overrideState = true;
        TextureCurve originCurve = textureCurveParameter.value;

        if (lensDistortion.intensity.overrideState == false)
            lensDistortion.intensity.overrideState = true;

        if (originCurve.length > 2)
        {
            for (int i = originCurve.length - 1; i > 1; i--)
            {
                originCurve.RemoveKey(i);
            }
        }
        Keyframe key = originCurve[1];
        key.value = 1;
        key.time = 1;
        originCurve.MoveKey(1, key);

        volumeParameter.value = 0;

        float time = 0;
        float outTime = flashEffectCurve[flashEffectCurve.length - 1].time;
        key = originCurve[0];
        motionBlur.quality.Override(MotionBlurQuality.Low);
        motionBlur.clamp.value = 0.2f;

        while (outTime >= time)
        {
            float timeValue = Mathf.Lerp(0, 1f, flashEffectCurve.Evaluate(time));
            key.value = timeValue;
            volumeParameter.value = timeValue;
            motionBlur.intensity.SetValue(volumeParameter);
            lensDistortion.intensity.SetValue(volumeParameter);
            originCurve.MoveKey(0, key);
            yield return null;
            time += Time.deltaTime;
        }
        colorCurves.active = false;
        lensDistortion.active = false;
    }

    IEnumerator Lens()
    {
        lensDistortion.active = true;
        VolumeParameter<float> volumeParameter = new VolumeParameter<float>();
        if (lensDistortion.intensity.overrideState == false)
            lensDistortion.intensity.overrideState = true;

        volumeParameter.value = 0;

        float time = 0;
        float outTime = flashEffectCurve[flashEffectCurve.length - 1].time;

        while (outTime >= time)
        {
            float timeValue = Mathf.Lerp(0, 1f, flashEffectCurve.Evaluate(time));
            volumeParameter.value = timeValue;
            lensDistortion.intensity.SetValue(volumeParameter);
            yield return null;
            time += Time.deltaTime;
        }
        lensDistortion.active = false;
    }

    IEnumerator ChromaticRoutine(AnimationCurve curve, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        chromatic.active = true;

        float time = 0;
        float outTime = flashEffectCurve[flashEffectCurve.length - 1].time;
        VolumeParameter<float> volumeParameter = new VolumeParameter<float>();
        volumeParameter.value = 0;
        while (outTime >= time)
        {
            float timeValue = Mathf.Lerp(0, 1f, flashEffectCurve.Evaluate(time));
            volumeParameter.value = timeValue;
            chromatic.intensity.SetValue(volumeParameter);
            yield return null;
            time += Time.deltaTime;
        }
        chromatic.active = false;
    }



    IEnumerator MotionBlurRoutine(AnimationCurve curve, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        motionBlur.active = true;
        float time = 0;
        float outTime = flashEffectCurve[flashEffectCurve.length - 1].time;
        VolumeParameter<float> volumeParameter = new VolumeParameter<float>();
        volumeParameter.value = 0;
        while (outTime >= time)
        {
            float timeValue = Mathf.Lerp(0, 1f, flashEffectCurve.Evaluate(time));
            volumeParameter.value = timeValue;
            motionBlur.intensity.SetValue(volumeParameter);
            yield return null;
            time += Time.deltaTime;
        }
        motionBlur.active = false;
    }

    IEnumerator BlurRoutine(AnimationCurve curve, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        dof.active = true;
        dof.mode.Override(DepthOfFieldMode.Gaussian);
        float time = 0;
        float outTime = flashEffectCurve[flashEffectCurve.length - 1].time;
        float halfTime = outTime * 0.5f;
        VolumeParameter<float> volumeParameter = new VolumeParameter<float>();
        volumeParameter.value = 0;
        while (halfTime >= time)
        {
            float timeValue = time / halfTime;
            volumeParameter.value = Mathf.Lerp(30, 0, timeValue);
            dof.gaussianStart.SetValue(volumeParameter);
            volumeParameter.value = Mathf.Lerp(0.5f, 1.5f, timeValue);
            dof.gaussianMaxRadius.SetValue(volumeParameter);
            yield return null;
            time += Time.deltaTime;
        }
        while(outTime >= time)
        {
            float timeValue = time / halfTime;
            volumeParameter.value = Mathf.Lerp(0, 30, timeValue);
            dof.gaussianStart.SetValue(volumeParameter);
            volumeParameter.value = Mathf.Lerp(1.5f, 0.5f, timeValue);
            dof.gaussianMaxRadius.SetValue(volumeParameter);
            yield return null;
            time += Time.deltaTime;
        }
        dof.active = false;
    }
}


