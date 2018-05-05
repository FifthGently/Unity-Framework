using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

public class TypeCreator
{
    public static Type Creator(DataTable data)
    {
        string ClassName = data.TableName;
        IDictionary<string, Type> Properties = new Dictionary<string, Type>();
        //读出字段名
        for (int i = 0; i < data.Columns.Count; ++i)
        {
            string[] arr = data.Columns[i].ToString().Split(char.Parse("#"));
            if (arr.Length == 1)
            {
                Properties.Add(new KeyValuePair<string, Type>(data.Columns[i].ToString(), GetType(data.Rows[0][i].ToString())));
            }
            else
            {
                Properties.Add(new KeyValuePair<string, Type>(arr[0], GetType2(arr[1])));
            }
        }
        return Creator(ClassName, Properties);
    }

    public static string GetTypeString(string flag)
    {
        switch (flag)
        {
            case "f":
                return "float";
            case "i":
                return "int";
            case "b":
                return "bool";
            default:
                return "string";
        }
    }

    public static Type GetType2(string flag)
    {
        switch (flag)
        {
            case "f":
                return typeof(float);
            case "i":
                return typeof(int);
            case "b":
                return typeof(bool);
            default:
                return typeof(string);
        }
    }

    private static Type GetType(string message)
    {
        message = message.ToLower();
        if (message == "true" || message == "false")
        {
            return typeof(bool);
        }
        else if (isNumberic(message))
        {
            return typeof(int);
        }
        else
        {
            return typeof(string);
        }
    }
    public static bool isNumberic(string message)
    {
        System.Text.RegularExpressions.Regex rex =
        new System.Text.RegularExpressions.Regex(@"^\d+$");
        if (rex.IsMatch(message))
        {
            return true;
        }
        else
            return false;
    }
    public static Type Creator(string ClassName, IDictionary<string, Type> Properties)
    {
        AppDomain currentDomain = System.Threading.Thread.GetDomain(); //AppDomain.CurrentDomain;
        TypeBuilder typeBuilder = null;
        ModuleBuilder moduleBuilder = null;
        //PropertyBuilder propertyBuilder = null;
        //FieldBuilder fieldBuilder = null;
        AssemblyBuilder assemblyBuilder = null;
        //CustomAttributeBuilder cab = null;

        //Define a Dynamic Assembly
        assemblyBuilder = currentDomain.DefineDynamicAssembly(new AssemblyName("Assembly-CSharp"), AssemblyBuilderAccess.Run);//AssemblyBuilder.GetCallingAssembly().FullName

        //Define a Dynamic Module
        moduleBuilder = assemblyBuilder.DefineDynamicModule("ModuleName", true);

        //Define a runtime class with specified name and attributes.
        typeBuilder = moduleBuilder.DefineType(ClassName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable);


        foreach (KeyValuePair<string, Type> kv in Properties)
        {
            // Add the class variable, such as "m_strIPAddress"
            typeBuilder.DefineField(kv.Key, kv.Value, FieldAttributes.Public);

            typeBuilder.DefineProperty(kv.Key, System.Reflection.PropertyAttributes.HasDefault, kv.Value, null);
             
        }
        //Create Class 
        return typeBuilder.CreateType();

    }
}
