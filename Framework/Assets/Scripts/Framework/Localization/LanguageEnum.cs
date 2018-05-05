namespace Frameworks
{
    using System.Collections.Generic;

    //SystemLanguage
    public enum LanguageEnum
    {
        Unknown = 0,
        Chinese,
        English,
        Japanese,
    }

    [System.Serializable]
    internal class KeyValuesInfo
    {
        public List<KeyValuesNode> ConfigInfo = null;
    }

    [System.Serializable]
    internal class KeyValuesNode
    {
        public string Key = null;
        public string Value = null;
    }
}
