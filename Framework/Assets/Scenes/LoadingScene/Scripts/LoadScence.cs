using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Frameworks;

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

    private IEnumerator loadScence()
    {
        int displayProgress = 0;
        int toProgress = 50;
        string loadSceneName = GameManager.SceneMgr.curScene;
        async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(loadSceneName);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            toProgress = (int) (async.progress*100);
            while (displayProgress < toProgress)
            {
                displayProgress+=2;
                SetLoadingPrecentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            displayProgress += 2;
            SetLoadingPrecentage(displayProgress);
            yield return new WaitForFixedUpdate();    //WaitForEndOfFrame();
        }
        StopAllCoroutines();
        async.allowSceneActivation = true;
    }

    private void SetLoadingPrecentage(int num)
    {
        float x = num / 100f;
        slider.value = x;
    }
}
