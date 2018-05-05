using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class CSVFileHelper
{
    /// <summary>
    /// 将DataTable中数据写入到CSV文件中
    /// </summary>
    /// <param name="dt">提供保存数据的DataTable</param>
    /// <param name="fileName">CSV的文件路径</param>
    public static void DataTableToCSV(DataTable dt, string fullPath)
    {
        FileInfo fi = new FileInfo(fullPath);
        if (!fi.Directory.Exists) { fi.Directory.Create(); }

        FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
        string data = "";
        // 写出列名称
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            data += dt.Columns[i].ColumnName.ToString();
            if (i < dt.Columns.Count - 1) data += ",";
        }
        sw.WriteLine(data);
        // 写出各行数据
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            data = "";
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                string str = dt.Rows[i][j].ToString();
                str = str.Replace("\"", "\"\"");
                if (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"))
                    str = string.Format("\"{0}\"", str);

                data += str;
                if (j < dt.Columns.Count - 1) data += ",";
            }
            sw.WriteLine(data);
        }

        sw.Close();
        fs.Close();
    }

    /// <summary>
    /// 将CSV文件的数据转成datatable
    /// </summary>
    /// <param name="csvfilePath">CSV文件路径</param>
    /// <param name="firstIsRowHead">是否将第一行作为字段名</param>
    /// <returns></returns>
    public static DataTable CSVToDataTable(string csvfilePath, bool firstIsRowHead)
    {
        Encoding encoding = GetType(csvfilePath); //Encoding.ASCII

        DataTable dtResult = null;
        if (File.Exists(csvfilePath))
        {
            string csvstr = File.ReadAllText(csvfilePath, encoding);
            if (!string.IsNullOrEmpty(csvstr))
            {
                dtResult = ToDataTable(csvstr, firstIsRowHead);
            }
        }
        return dtResult;
    }

    /// <summary>
    /// 将CSV数据转换为DataTable
    /// </summary>
    /// <param name="csv">包含以","分隔的CSV数据的字符串</param>
    /// <param name="isRowHead">是否将第一行作为字段名</param>
    /// <returns></returns>
    private static DataTable ToDataTable(string csv, bool isRowHead)
    {
        DataTable dt = null;
        if (!string.IsNullOrEmpty(csv))
        {
            dt = new DataTable();
            string[] csvRows = csv.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] csvColumns = null;
            if (csvRows != null)
            {
                if (csvRows.Length > 0)
                {
                    //第一行作为字段名,添加第一行记录并删除csvRows中的第一行数据
                    if (isRowHead)
                    {
                        csvColumns = FromCsvLine(csvRows[0]);
                        csvRows[0] = null;
                        for (int i = 0; i < csvColumns.Length; i++)
                        {
                            dt.Columns.Add(csvColumns[i]);
                        }
                    }

                    for (int i = 0; i < csvRows.Length; i++)
                    {
                        if (csvRows[i] != null)
                        {
                            csvColumns = FromCsvLine(csvRows[i]);
                            //检查列数是否足够,不足则补充
                            if (dt.Columns.Count < csvColumns.Length)
                            {
                                int columnCount = csvColumns.Length - dt.Columns.Count;
                                for (int c = 0; c < columnCount; c++)
                                {
                                    dt.Columns.Add();
                                }
                            }
                            dt.Rows.Add(csvColumns);
                        }
                    }
                }
            }
        }

        return dt;
    }

    /// <summary>
    /// 解析一行CSV数据
    /// </summary>
    /// <param name="csv">csv数据行</param>
    /// <returns></returns>
    private static string[] FromCsvLine(string csv)
    {
        List<string> csvLiAsc = new List<string>();
        List<string> csvLiDesc = new List<string>();

        if (!string.IsNullOrEmpty(csv))
        {
            //顺序查找
            int lastIndex = 0;
            int quotCount = 0;

            //剩余的字符串
            string lstr = string.Empty;
            for (int i = 0; i < csv.Length; i++)
            {
                char temp1 = csv[i];

                if (csv[i] == '"')
                {
                    quotCount++;
                }
                else if (csv[i] == ',' && quotCount % 2 == 0)
                {
                    csvLiAsc.Add(ReplaceQuote(csv.Substring(lastIndex, i - lastIndex)));
                    lastIndex = i + 1;
                }
                if (i == csv.Length - 1 && lastIndex < csv.Length)
                {
                    lstr = csv.Substring(lastIndex, i - lastIndex + 1);
                }
            }

            if (!string.IsNullOrEmpty(lstr))
            {
                //倒序查找
                lastIndex = 0;
                quotCount = 0;
                string revStr = Reverse(lstr);
                for (int i = 0; i < revStr.Length; i++)
                {
                    if (revStr[i] == '"')
                    {
                        quotCount++;
                    }
                    else if (revStr[i] == ',' && quotCount % 2 == 0)
                    {
                        csvLiDesc.Add(ReplaceQuote(Reverse(revStr.Substring(lastIndex, i - lastIndex))));
                        lastIndex = i + 1;
                    }
                    if (i == revStr.Length - 1 && lastIndex < revStr.Length)
                    {
                        csvLiDesc.Add(ReplaceQuote(Reverse(revStr.Substring(lastIndex, i - lastIndex + 1))));
                        lastIndex = i + 1;
                    }

                }
                string[] tmpStrs = csvLiDesc.ToArray();
                Array.Reverse(tmpStrs);
                csvLiAsc.AddRange(tmpStrs);
            }
        }

        return csvLiAsc.ToArray();
    }

    /// <summary>
    /// 反转字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string Reverse(string str)
    {
        string revStr = string.Empty;
        foreach (char chr in str)
        {
            revStr = chr.ToString() + revStr;
        }
        return revStr;
    }

    /// <summary>
    /// 替换CSV中的双引号转义符为正常双引号,并去掉左右双引号
    /// </summary>
    /// <param name="csvValue">csv格式的数据</param>
    /// <returns></returns>
    private static string ReplaceQuote(string csvValue)
    {
        string rtnStr = csvValue;
        if (!string.IsNullOrEmpty(csvValue))
        {
            //首尾都是"
            Match m = Regex.Match(csvValue, "^\"(.*?)\"$");
            if (m.Success)
            {
                rtnStr = m.Result("${1}").Replace("\"\"", "\"");
            }
            else
            {
                rtnStr = rtnStr.Replace("\"\"", "\"");
            }
        }
        return rtnStr;
    }

    /// <summary>
    /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>文件的编码类型</returns>
    private static Encoding GetType(string filePath)
    {
        FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        Encoding r = GetType(fs);
        fs.Close();
        return r;
    }

    /// <summary>
    /// 通过给定的文件流，判断文件的编码类型
    /// </summary>
    /// <param name="fs">文件流</param>
    /// <returns>文件的编码类型</returns>
    private static Encoding GetType(FileStream fs)
    {
        //byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
        //byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
        //byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM  
        Encoding reVal = Encoding.Default;

        BinaryReader r = new BinaryReader(fs, Encoding.Default);
        int i;
        int.TryParse(fs.Length.ToString(), out i);
        byte[] ss = r.ReadBytes(i);
        if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
        {
            reVal = Encoding.UTF8;
        }
        else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
        {
            reVal = Encoding.BigEndianUnicode;
        }
        else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
        {
            reVal = Encoding.Unicode;
        }
        r.Close();
        return reVal;
    }

    /// <summary>
    /// 判断是否是不带 BOM 的 UTF8 格式
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static bool IsUTF8Bytes(byte[] data)
    {
        int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数  
        byte curByte; //当前分析的字节.  
        for (int i = 0; i < data.Length; i++)
        {
            curByte = data[i];
            if (charByteCounter == 1)
            {
                if (curByte >= 0x80)
                {
                    // 判断当前  
                    while (((curByte <<= 1) & 0x80) != 0)
                    {
                        charByteCounter++;
                    }
                    // 标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　  
                    if (charByteCounter == 1 || charByteCounter > 6)
                    {
                        return false;
                    }
                }
            }
            else
            {
                // 若是UTF-8 此时第一位必须为1  
                if ((curByte & 0xC0) != 0x80)
                {
                    return false;
                }
                charByteCounter--;
            }
        }
        if (charByteCounter > 1)
        {
            throw new Exception("非预期的byte格式");
        }
        return true;
    }

    /// <summary>
    /// 将CSV文件的数据转成Dictionary
    /// </summary>
    /// <param name="csvfilePath">CSV文件路径</param>
    /// <param name="key">第几列作为Key</param>
    /// <param name="value">第几列作为value</param>
    /// <returns></returns>
    public static Dictionary<string, string> CSVToDictionary(string csvfilePath, int key, int value)
    {
        Encoding encoding = GetType(csvfilePath);

        Dictionary<string, string> dic = null;
        if (File.Exists(csvfilePath))
        {
            string csvstr = File.ReadAllText(csvfilePath, encoding);
            if (!string.IsNullOrEmpty(csvstr))
            {
                dic = new Dictionary<string, string>();
                string[] csvRows = csvstr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                string[] csvColumns = null;
                if (csvRows != null && csvRows.Length > 0)
                {
                    for (int i = 0; i < csvRows.Length; i++)
                    {
                        if (csvRows[i] != null)
                        {
                            csvColumns = FromCsvLine(csvRows[i]);
                            if (csvColumns.Length > key && csvColumns.Length > value)
                                dic.Add(csvColumns[key], csvColumns[value]);
                        }
                    }
                }
            }
        }
        return dic;
    }

    /// <summary>
    /// 我们需要保存历史数据 或者实时的知道那个文件被修改 
    /// 可以通过改变文件的名称 如加上当天的日期等等
    /// </summary>
    /// <param name="oldPath"></param>
    /// <param name="newPath"></param>
    /// <returns></returns>
    public static bool ChangeFileName(string oldPath, string newPath)
    {
        bool re = false;
        try
        {
            if (File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
                re = true;
            }
        }
        catch
        {
            re = false;
        }
        return re;
    }

    /// <summary>
    /// 直接在网页表单提交数据保存在csv文件中 直接写入文件
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="Data"></param>
    /// <returns></returns>
    public static bool SaveCSV(string fullPath, string Data)
    {
        bool re = true;
        try
        {
            FileStream fileStream = new FileStream(fullPath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fileStream, Encoding.UTF8);
            sw.WriteLine(Data);
            // 清空缓冲区  
            sw.Flush();
            // 关闭流  
            sw.Close();
            fileStream.Close();
        }
        catch
        {
            re = false;
        }
        return re;
    }
}
