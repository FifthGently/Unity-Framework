using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextHelper 
{
    private static readonly string[] ColumnSplit = new string[] { "\t" };

    public static string GetFullName<T>(string name)
    {
        return GetFullName(typeof(T), name);
    }

    public static string GetFullName(Type type, string name)
    {
        string typeName = type.FullName;
        return string.IsNullOrEmpty(name) ? typeName : string.Format("{0}.{1}", typeName, name);
    }

    /// <summary>
    /// 将要解析的数据表文本分割为数据表行文本。
    /// </summary>
    /// <param name="text">要解析的数据表文本。</param>
    /// <returns>数据表行文本。</returns>
    public static string[] SplitToDataRows(string text)
    {
        List<string> texts = new List<string>();
        string[] rowTexts = SplitToLines(text);
        for (int i = 0; i < rowTexts.Length; i++)
        {
            if (rowTexts[i].Length <= 0 || rowTexts[i][0] == '#')
            {
                continue;
            }
            texts.Add(rowTexts[i]);
        }

        return texts.ToArray();
    }

    /// <summary>
    /// 将文本按行切分。
    /// </summary>
    /// <param name="text">要切分的文本。</param>
    /// <returns>按行切分后的文本。</returns>
    public static string[] SplitToLines(string text)
    {
        List<string> texts = new List<string>();
        int position = 0;
        string rowText = null;
        while ((rowText = ReadLine(text, ref position)) != null)
        {
            texts.Add(rowText);
        }

        return texts.ToArray();
    }

    /// <summary>
    /// 读取一行文本。
    /// </summary>
    /// <param name="text">要读取的文本。</param>
    /// <param name="position">开始的位置。</param>
    /// <returns>一行文本。</returns>
    public static string ReadLine(string text, ref int position)
    {
        if (text == null)
        {
            return null;
        }

        int length = text.Length;
        int offset = position;
        while (offset < length)
        {
            char ch = text[offset];
            switch (ch)
            {
                case '\r':
                case '\n':
                    string str = text.Substring(position, offset - position);
                    position = offset + 1;
                    if (((ch == '\r') && (position < length)) && (text[position] == '\n'))
                    {
                        position++;
                    }

                    return str;
                default:
                    offset++;
                    break;
            }
        }

        if (offset > position)
        {
            string str = text.Substring(position, offset - position);
            position = offset;
            return str;
        }

        return null;
    }

    public static string[] SplitDataRow(string dataRowText)
    {
        return dataRowText.Split(ColumnSplit, StringSplitOptions.None);
    }
}
