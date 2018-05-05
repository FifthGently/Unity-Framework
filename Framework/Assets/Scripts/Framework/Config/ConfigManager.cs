namespace Frameworks
{
    public class ConfigManager
    {
        public static ConfigManager Instance { get { return Singleton<ConfigManager>.Instance; } }

        // 路径常量
        public const string CONFIG_PATH_XML = "Xml/";
        public const string CONFIG_PATH_JSON = "Json/";
        public const string CONFIG_PATH_TXT = "Txt/";

        public const string CONFIG_PATH_CANVAS = "UIPrefabs/Canvas";
        public const string CONFIG_PATH_CONFIG_INFO = "SysConfigInfo";

        // 文件名
        public const string xmlName_server = "server";
        public const string txtname = "word1";
        public const string chinese = "server";

        // 标签常量
        public const string SYS_TAG_CANVAS = "_TagCanvas";

        // 摄像机层深的常量

        // 委托的定义

        // 全局性的方法

        public string GetResString(string key)
        {
            string value = key;
            // ChineseBase chBase = TableManager.GetInstance().GetChineseTable().Find(key);
            // if (chBase != null)
            //  value = chBase.Text;

            return value;
        }

        // TODO: HARD CODE
        public static string[] NumToText = new string[] { "", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

        public static string String(string key)
        {
            return Instance.GetResString(key);
        }

        public static string String(string key, params object[] args)
        {
            return string.Format(String(key), args);
        }
    }
}

