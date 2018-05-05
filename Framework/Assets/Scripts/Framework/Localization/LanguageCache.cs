namespace Frameworks
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class LanguageCache
    {
        public const string SAVE_LANGUAGE = "LANGUAGE";

        public const string PATH_LANGUAGE_CHINESE = "LauguageJSONConfig_CN";
        public const string PATH_LANGUAGE_ENGLISH = "LauguageJSONConfig_EN";
        public const string PATH_LANGUAGE_JAPANESE = "LauguageJSONConfig_CN";

        /// <summary>
        /// 语言翻译的缓存集合
        /// </summary>
        private Dictionary<string, string> LanguageData;

        private UnityAction RefreshGUIText;

        public LanguageCache()
        {
            LanguageData = new Dictionary<string, string>();
            this.SetLanguage((LanguageEnum)this.GetLanguage());
        }

        /// <summary>
        /// 到显示文本信息
        /// </summary>
        /// <param name="languageID">语言的ID</param>
        /// <returns></returns>
        public string ShowText(string languageID)
        {
            if (string.IsNullOrEmpty(languageID)) return null;

            //查询文本
            string strQueryResult = string.Empty;
            if (LanguageData != null && LanguageData.Count > 0)
            {
                LanguageData.TryGetValue(languageID, out strQueryResult);
                if (!string.IsNullOrEmpty(strQueryResult))
                {
                    return strQueryResult;
                }
            }
            return null;
        }

        public void SetLanguage(LanguageEnum language)
        {
            switch (language)
            {
                case LanguageEnum.Unknown: this.InitLanguageByJson(PATH_LANGUAGE_CHINESE); break;
                case LanguageEnum.Chinese: this.InitLanguageByJson(PATH_LANGUAGE_CHINESE); break;
                case LanguageEnum.English: this.InitLanguageByJson(PATH_LANGUAGE_ENGLISH); break;
                case LanguageEnum.Japanese: this.InitLanguageByJson(PATH_LANGUAGE_JAPANESE); break;
                default: this.InitLanguageByJson(PATH_LANGUAGE_CHINESE); break;
            }

            SaveLanguage((int)language);
            if (RefreshGUIText != null) RefreshGUIText();
        }

        public void AddEvent(UnityAction callback)
        {
            RefreshGUIText += callback;
        }

        public void RemoveEvent(UnityAction callback)
        {
            RefreshGUIText -= callback;
        }

        public void RemoveAllEvent()
        {
            System.Delegate[] dels = RefreshGUIText.GetInvocationList();
            foreach (System.Delegate d in dels)
                RefreshGUIText -= d as UnityAction;
        }

        public int GetLanguageCacheMaxNumber()
        {
            if (LanguageData != null && LanguageData.Count > 0)
            {
                return LanguageData.Count;
            }
            else return 0;
        }

        private void SaveLanguage(int language)
        {
            if (GetLanguage() == language) return;

            PlayerPrefs.SetInt(SAVE_LANGUAGE, language);
            PlayerPrefs.Save();
        }

        private int GetLanguage()
        {
            return PlayerPrefs.GetInt(SAVE_LANGUAGE, 0);
        }

        #region LanguageCache by Json

        public void InitLanguageByJson(string jsonPath)
        {
            if (string.IsNullOrEmpty(jsonPath)) return;

            if (LanguageData == null)
                LanguageData = new Dictionary<string, string>();
            LanguageData.Clear();

            TextAsset configInfo = null;
            KeyValuesInfo keyvalueInfoObj = null;
            try
            {
                configInfo = Resources.Load<TextAsset>(jsonPath);
                keyvalueInfoObj = JsonUtility.FromJson<KeyValuesInfo>(configInfo.text);
            }
            catch
            {
                throw new System.SystemException("LanguageCache.InitLanguageByJson() Json Analysis Exception !jsonPath=" + jsonPath);
            }

            foreach (KeyValuesNode nodeInfo in keyvalueInfoObj.ConfigInfo)
            {
                LanguageData.Add(nodeInfo.Key, nodeInfo.Value);
            }
        }

        #endregion

        #region LanguageCache by CSV

        public void InitLanguageByCSV(string csvPath)
        {
            if (string.IsNullOrEmpty(csvPath)) return;

            if (LanguageData == null)
                LanguageData = new Dictionary<string, string>();
            LanguageData.Clear();

            LanguageData = CSVFileHelper.CSVToDictionary(csvPath, 0, 1);
        }

        #endregion

        private string GetWinReadPath(string fileName)
        {
            string filePath = Application.streamingAssetsPath + "/Localization/" + fileName + ".csv";
            return filePath;
        }

        private string GetWinSavePath(string fileName)
        {
            string filePath = Application.streamingAssetsPath + "/Localization/" + fileName + ".txt";
            return filePath;
        }

        public override string ToString()
        {
            string result = "LanguageType:" + ((LanguageEnum)GetLanguage()).ToString();
            List<string> tempKeys = new List<string>(LanguageData.Keys);
            for (int i = 0; i < tempKeys.Count; ++i)
            {
                result += "\nKey:[" + tempKeys[i] + "]|Value:[" + LanguageData[tempKeys[i]] + "]";
            }
            return result;
        }
    }
}