using Frameworks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadScence : MonoBehaviour
{
    //private float time;
    //private int nowfram;
    public Slider slider;
    private AsyncOperation async;

    void Start ()
	{
	    StartCoroutine(loadScence());
	}

    private void Update()
    {
        LoadingText();
        LoadingSpinner();
    }

    private IEnumerator loadScence()
    {
        int displayProgress = 0;
        int toProgress = 0;
        string loadSceneName = GameManager.SceneMgr.curScene;
        async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(loadSceneName);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            toProgress = (int)(async.progress * 100);
            while (displayProgress < toProgress)
            {
                displayProgress += 2;
                SetLoadingPrecentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            displayProgress += 2;
            SetLoadingPrecentage(displayProgress);
            yield return new WaitForFixedUpdate();
        }
        StopAllCoroutines();
        async.allowSceneActivation = true;
    }

    private void SetLoadingPrecentage(int num)
    {
        float x = num / 100f;
        slider.value = x;
    }

    #region LoadingText
    public Text loadingText;
    private float lastUpdate = 0;
    private int numElipses = 1;

    private void LoadingText()
    {
        if (lastUpdate == 0 || Time.unscaledTime > (lastUpdate + 0.3f))
        {
            string t = "Loading";
            for (int i = 0; i < numElipses; i++)
            {
                t += ".";
            }
            loadingText.text = t;
            numElipses = numElipses == 3 ? 0 : numElipses + 1;

            lastUpdate = Time.unscaledTime;
        }
    }
    #endregion

    #region LoadingSpinner
    public Transform loadingSpinner;
    private Quaternion targetRotation = Quaternion.AngleAxis(180, Vector3.forward);

    private void LoadingSpinner()
    {
        if (loadingSpinner.gameObject.activeSelf)
        {
            loadingSpinner.rotation = Quaternion.Slerp(loadingSpinner.rotation, targetRotation, 0.05f);
            if (Quaternion.Angle(loadingSpinner.rotation, targetRotation) < 1)
            {
                loadingSpinner.rotation = Quaternion.AngleAxis(0, Vector3.forward);
            }
        }
    }
    #endregion
}
