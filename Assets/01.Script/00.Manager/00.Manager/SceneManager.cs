using Firebase.Database;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : Singleton<SceneManager>
{
    [SerializeField] Image fade;
    [SerializeField] Image timeRewindImg;
    [SerializeField] Image atkedImg;
    [SerializeField] Slider loadingBar;
    [SerializeField] float fadeTime;
    [SerializeField] Sprite[] loadingImgs;
    private BaseScene curScene;
    public bool onFading;

    public BaseScene GetCurScene()
    {
        if (curScene == null)
            curScene = FindObjectOfType<BaseScene>();
        
        return curScene;
    }

    public T GetCurScene<T>() where T : BaseScene
    {
        if (curScene == null)
            curScene = FindObjectOfType<BaseScene>();

        return curScene as T;
    }

    /// <summary>
    /// 로딩 이미지의 배열이 존재한다면 로딩이미지를 Fade 이미지에 추가합니다.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadingImgIdx"></param>
    public void LoadScene(string sceneName, int loadingImgIdx = 0)
    {
        if(loadingImgs != null)
            fade.sprite = loadingImgs[loadingImgIdx];

        StartCoroutine(LoadingRoutine(sceneName));
    }

    IEnumerator LoadingRoutine(string sceneName)
    {
        fade.gameObject.SetActive(true);
        yield return FadeOut(fade,1);

        Manager.Pool.ClearPool();
        Manager.Sound.StopSFX();
        Manager.UI.ClearPopUpUI();
        Manager.UI.ClearWindowUI();
        Manager.UI.CloseInGameUI();

        Time.timeScale = 0f;
        loadingBar.gameObject.SetActive(true);

        AsyncOperation oper = UnitySceneManager.LoadSceneAsync(sceneName);
        while (oper.isDone == false)
        {
            loadingBar.value = oper.progress;
            yield return null;
        }

        Manager.UI.EnsureEventSystem();

        BaseScene curScene = GetCurScene();
        yield return curScene.LoadingRoutine();

        loadingBar.gameObject.SetActive(false);
        Time.timeScale = 1f;

        yield return FadeIn(fade,1);
        fade.gameObject.SetActive(false);

    }
    
    IEnumerator FadeOut(Image img,float f)
    {
        float rate = 0;
        Color fadeOutColor = new Color(img.color.r, img.color.g, img.color.b, f);
        Color fadeInColor = new Color(img.color.r, img.color.g, img.color.b, 0f);

        while (rate <= 1)
        {
            rate += Time.deltaTime / fadeTime;
            img.color = Color.Lerp(fadeInColor, fadeOutColor, rate);
            yield return null;
        }
    }

    IEnumerator FadeIn(Image img,float f)
    {
        onFading = true;
        float rate = 0;
        Color fadeOutColor = new Color(img.color.r, img.color.g, img.color.b, f);
        Color fadeInColor = new Color(img.color.r, img.color.g, img.color.b, 0f);

        while (rate <= 1)
        {
            rate += Time.deltaTime / fadeTime;
            img.color = Color.Lerp(fadeOutColor, fadeInColor, rate);
            yield return null;
        }
        onFading = false;
        fadeInRoutine = null;
    }
    public IEnumerator AtkedEffect()
    {
        float rate = 0;
        Color littleRedColor = new Color(1, fade.color.g, fade.color.b, 0.3f);
        Color fadeInColor = new Color(fade.color.r, fade.color.g, fade.color.b, 0f);
        while (rate <= 1f)
        {
            rate += Time.deltaTime / 0.5f;
            fade.color = Color.Lerp(littleRedColor, fadeInColor, rate);
            yield return null;
        }
        fade.color = new Color(0, fade.color.g, fade.color.b, 0f);
    }
    public Coroutine StartFadeOut()
    {
      
       return StartCoroutine(FadeOut(fade,1));
    }
    Coroutine fadeInRoutine;
    public void StartFadeIn()
    {
        
        if (fadeInRoutine != null)
            StopCoroutine(fadeInRoutine);

        fadeInRoutine = StartCoroutine(FadeIn(fade, 1));
    }
    public Coroutine RewindOut()
    {

        return StartCoroutine(FadeOut(timeRewindImg,0.7f));
    }

    Coroutine rewindRoutine;
    public void RewindIn()
    {

        if (rewindRoutine != null)
            StopCoroutine(rewindRoutine);

        rewindRoutine = StartCoroutine(FadeIn(timeRewindImg,0.7f));
    }

}
