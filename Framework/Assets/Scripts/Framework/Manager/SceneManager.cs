namespace Frameworks
{
    public class SceneManager
    {
        public string curScene = "";            //当前场景名称
        private string loadingSceneName = "";   //进度显示的场景名称

        public void SetupLoadingScene(string _loadingSceneName = "")
        {
            loadingSceneName = _loadingSceneName;
        }

        public void LoadScene(string sceneName)
        {
            curScene = sceneName;
            if (string.IsNullOrEmpty(loadingSceneName))
                UnityEngine.SceneManagement.SceneManager.LoadScene(curScene);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(loadingSceneName);
        }

        public void LoadSceneByLoading(string sceneName,string _loadingSceneName)
        {
            loadingSceneName = _loadingSceneName;
            this.LoadScene(sceneName);
        }
        /// <summary>
        /// 不用加载进度条过渡，直接加载指定场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneDirect(string sceneName)
        {
            curScene = sceneName;
            UnityEngine.SceneManagement.SceneManager.LoadScene(curScene);
        }

        public void LoadCurrentScene()
        {
            this.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}