using UnityEngine;

namespace Frameworks
{
    public class StreamString
    {
        public string TempStr;
        public string[] TempArStr;
        public string[] Arstr;

        public StreamString() { }

        public void ReadData()
        {
            string strConfig = ConfigManager.CONFIG_PATH_TXT + ConfigManager.txtname;
            TextAsset text = Resources.Load(strConfig) as TextAsset;
            TempStr = text.ToString();
            TempArStr = TempStr.Split(',');
            int Index = 0;
            Arstr = new string[(TempArStr.Length / 500) + 1];
            for (int i = 0; i < TempArStr.Length; i++)
            {
                //string Str = TempArStr[i].ToString().Replace("(", ReplaceString);
                //if (TempArStr[i].Length!=Str.Length)
                //{
                //    GameLog.LogError("    " + TempArStr[i] + "     " + i);
                //    Str = Str.ToString().Replace(")", ReplaceString);
                //    GameLog.LogError("    " + TempArStr[i] + "     " + i + "    " + Str);
                //    continue;
                //}
                if (i < (Index + 1) * 500)
                {
                    Arstr[Index] += @"(" + TempArStr[i] + @")|";
                }
                else
                {
                    Index++;
                    Arstr[Index] += @"(" + TempArStr[i] + @")|";
                }
            }
        }
    }
}
