using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Excel;
using UnityEngine;
using UnityEditor;

public class ReadExcel
{
    public static void ExcelRead(string file)
    {
        List<DataTable> tables = AsDataTable(file);
        for (int cnt = 0; cnt < tables.Count; cnt++)
        {
            DataTable data = tables[cnt];
            string classPath = PathConfig.classPath + data.TableName + ".cs";
            string configPath = PathConfig.configPath + data.TableName + ".xml";
            CreateData(configPath, data);//生成序列化后的二进制文件
            ClassCreator.NewAssembly(classPath, data);//写表字段对应的类
            TableManagerAppend.AppendTableManager(data.TableName);//为TableManager增加对应配置表引用
            EnumWrite.WriteEnum(data);
        }
        Debug.Log("Excel表导出成功！");
    }

    private static List<DataTable> AsDataTable(string file)
    {
        List<DataTable> dataList = new List<DataTable>();

        FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        do
        {
            // sheet name
            DataTable data = new DataTable();
            data.TableName = excelReader.Name;
            bool isFirstRowReaded = false;
            while (excelReader.Read())
            {
                DataRow dataRow = data.NewRow();
                for (int i = 0; i < excelReader.FieldCount; i++)
                {
                    string value = excelReader.IsDBNull(i) ? "" : excelReader.GetString(i);
                    if (isFirstRowReaded == false)
                    {
                        data.Columns.Add(value);
                    }
                    else
                    {
                        dataRow[i] = value;
                    }
                }
                if (isFirstRowReaded == true)
                {
                    data.Rows.Add(dataRow);
                }
                isFirstRowReaded = true;
            }
            dataList.Add(data);
        } while (excelReader.NextResult());

        excelReader.Close();

        return dataList;
    }
    private static void ReflectionSetProperty(object objClass, string propertyName, string value)
    {
        FieldInfo infos = objClass.GetType().GetField(propertyName);
        string typeString = infos.FieldType.ToString();
        if (value == "") value = "0";
        switch (typeString)
        {
            case "System.Int32":
                infos.SetValue(objClass, int.Parse(value));
                break;
            case "System.Boolean":
                infos.SetValue(objClass, bool.Parse(value));
                break;
            case "System.Single":
                infos.SetValue(objClass, float.Parse(value));
                break;
            default:
                infos.SetValue(objClass, value);
                break;
        }
    }
    static void CreateData(string classPath, DataTable data)
    {
        if (data == null) return;

        Type t = TypeCreator.Creator(data);

        List<object> list = new List<object>();

        for (int i = 0; i < data.Rows.Count; ++i)
        {
            object d = Activator.CreateInstance(t);
            for (int j = 0; j < data.Columns.Count; ++j)
            {
                ReflectionSetProperty(d, data.Columns[j].ToString().Split(char.Parse("#"))[0], data.Rows[i][j].ToString());
            }
            list.Add(d);
        }

        WriteBatFile(list, classPath);
    }

    private static void WriteBatFile(List<object> list, string fileName)
    {
        CheckDirectory(fileName);

        Stream fStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);

        BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器

        binFormat.Serialize(fStream, list);
        fStream.Close();
    }

    public static void CheckDirectory(string fileName)
    {
        string directory = fileName.Substring(0, fileName.LastIndexOf("\\"));
        if (System.IO.Directory.Exists(directory) == false)
        {
            System.IO.Directory.CreateDirectory(directory);
        }
    }

    public static void ReadBatFile(string fileName)
    {
        BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
        Stream fStream2 = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        List<object> list2 = (List<object>)binFormat.Deserialize(fStream2);//反序列化对象

        foreach (object p in list2)
        {
            Console.WriteLine(p);

        }
    }
}
