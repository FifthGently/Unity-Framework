using System;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;
using System.Data;
using System.IO;

public class ClassCreator
{
    public static void NewAssembly(string classPath, DataTable data)
    {
        //创建动态代码。   
        StringBuilder classSource = new StringBuilder();
        classSource.Append("using System;\n[Serializable]\n");
        classSource.Append("public   class   " + data.TableName + " :TableInfo<int> \n");
        classSource.Append("{\n");

        string toString = "\n" + "  override public string ToString()\n  {\n     return ";

        //读出字段名
        for (int i = 0; i < data.Columns.Count; ++i)
        {
            Console.Write("{0} ", data.Columns[i]);
            string[] arr = data.Columns[i].ToString().Split(char.Parse("#"));
            if (arr.Length == 1)
            {
                classSource.Append(propertyString(data.Columns[i].ToString(), GetType(data.Rows[0][i].ToString())));
            }
            else
            {
                classSource.Append(propertyString(arr[0], TypeCreator.GetTypeString(arr[1])));
            }
            if (i == 0)
            {
                toString += "\"" + arr[0] + ":\" + " + arr[0];
            }
            else
            {
                toString += " + \" , " + arr[0] + ":\" + " + arr[0];
            }
        }
        classSource.Append("\n  public int GetID() { return ID; }\n");
        //创建属性。   
        classSource.Append(toString + ";\n  }");

        classSource.Append("\n}");

        Write(classSource.ToString(), classPath);

        //编译代码。   
        //CompilerResults result = provider.CompileAssemblyFromSource(paras, classSource.ToString());

        //获取编译后的程序集。   
        //Assembly assembly = result.CompiledAssembly;

    }
    private static string GetType(string message)
    {
        message = message.ToLower();
        if (message == "true" || message == "false")
        {
            return "bool  ";
        }
        else if (TypeCreator.isNumberic(message))
        {
            return "int   ";
        }
        else
        {
            return "string";
        }
    }

    private static void Write(string text, string path)
    {
        FileStream fs = new FileStream(path, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs, Encoding.ASCII);
        sw.Write(text);
        sw.Close();
        fs.Close();
    }

    private static string propertyString(string propertyName, string type)
    {
        StringBuilder sbProperty = new StringBuilder();
        sbProperty.Append(" public   " + type + " " + propertyName + ";\n");
        //sbProperty.Append(" public   " + type  + " " + propertyName + "\n");
        //sbProperty.Append(" {\n");
        //sbProperty.Append("     get{   return   _" + propertyName + ";}   \n");
        //sbProperty.Append("     set{   _" + propertyName + "   =   value;   }\n");
        //sbProperty.Append(" }");
        return sbProperty.ToString();
    }
}
