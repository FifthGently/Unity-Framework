using System;
using System.IO;
using System.Text;

public class TableManagerAppend
{
    private static string Temple = "\n    private ConfigTable<int, MySheet> mMySheetTable;"+
   "\n    public ConfigTable<int, MySheet> GetMySheetTable()" +
   "\n    {"+
      "\n       if (mMySheetTable == null)" +
        "\n      {" +
           "\n          mMySheetTable = new ConfigTable<int, MySheet>(\"MySheet\");" +
           "\n          mMySheetTable.Load();" +
        "\n       }" +
       "\n           return mMySheetTable;" +
    "\n     }";

	public static void AppendTableManager(string appendStr)
	{
        string sign = " //##";
        string path = PathConfig.tableManagerPath;
        string text = string.Empty;
        string tableName = "mMySheetTable".Replace("MySheet", appendStr);

        appendStr = Temple.Replace("MySheet", appendStr);

        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                text = sr.ReadToEnd();

                if (text.IndexOf(tableName) > -1)
                {
                    return;
                }

                text = text.Replace(sign, appendStr + "\n" + sign);
            }
        }

        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(text);
            }
        }
	}

    public static void CheckTableFile()
    {
 
    }
    //
}
