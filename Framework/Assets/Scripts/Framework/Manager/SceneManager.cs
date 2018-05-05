namespace Frameworks
{
    public class SceneManager
    {
        public string curScene = "";            //��ǰ��������
        private string loadingSceneName = "";   //������ʾ�ĳ�������

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
        /// ���ü��ؽ��������ɣ�ֱ�Ӽ���ָ������
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