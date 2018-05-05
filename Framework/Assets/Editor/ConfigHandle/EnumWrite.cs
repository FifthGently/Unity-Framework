using System;
using System.Data;
using System.Text;
using System.IO;

/// <summary>
/// Description of Singleton1
/// </summary>
public sealed class EnumWrite
{
    public static void WriteEnum(DataTable data)
    { 
          //读出字段名
        for (int i = 0; i < data.Columns.Count; ++i)
        {
            string[] arr = data.Columns[i].ToString().Split(char.Parse("#"));
            if(arr.Length > 1 && arr[0] != "")
            {
                Console.Write(arr[0]);
                WriteOneEnum(data,arr[0].ToString(),i);
                break;
            }
        }
    }

    private static void WriteOneEnum(DataTable data, string p,int index)
    {
        string fileName = data.TableName + p + "Enum";
        StringBuilder classSource = new StringBuilder();
        classSource.Append("public   class   " + fileName + "\n");
        classSource.Append("{\n");
        for (int i = 0; i < data.Rows.Count; ++i)
        {
            classSource.AppendLine("    public static int " + data.Rows[i][index] + " = " + data.Rows[i][0] + " ;");
        }

        classSource.Append("}\n");

        Write(classSource.ToString(), PathConfig.classPath + "Enum\\" + fileName + ".cs");
    }
    public static void Write(string text, string path)
    {
        ReadExcel.CheckDirectory(path);

        FileStream fs = new FileStream(path, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
        sw.Write(text);
        sw.Close();
        fs.Close();
    }
}
