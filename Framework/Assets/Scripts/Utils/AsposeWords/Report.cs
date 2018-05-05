/*
 * 描述：     Report
 * 作者：     FifthGently
 * 版本：     v1.00
 */
using Aspose.Words;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Report
{
    private string templatePath = Application.dataPath + @"\Template\";
    private string reportPath = Application.dataPath + @"\Report";

    private DocumentBuilder wordApp = null;
    private Document wordDoc = null;

    public DocumentBuilder WordApp
    {
        get { return wordApp; }
        set { wordApp = value; }
    }

    public Document WordDoc
    {
        get { return wordDoc; }
        set { wordDoc = value; }
    }

    /// <summary>
    /// 通过模板创建新文档
    /// </summary>
    /// <param name="fileName"></param>
    public void OpenWithTemplate(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            wordDoc = new Document(templatePath + fileName);
        }
    }

    public void Open()
    {
        wordDoc = new Document();
    }

    public void Builder()
    {
        WordApp = new DocumentBuilder(wordDoc);
    }

    /// <summary>
    /// 保存新文件
    /// </summary>
    /// <param name="filePath"></param>
    public void SaveDocument(string fileName)
    {
        if(!Directory.Exists(reportPath))
            Directory.CreateDirectory(reportPath);

        wordDoc.Save(reportPath + @"\" + fileName, SaveFormat.Docx);
    }

    /// <summary>
    /// 在书签处插入值
    /// </summary>
    /// <param name="bookmark"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool InsertValue(string bookmark, string value)
    {
        if (wordDoc.Range.Bookmarks[bookmark] != null)
        {
            wordDoc.Range.Bookmarks[bookmark].Text = value;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置纸张
    /// </summary>
    /// <param name="papersize"></param>
    public void SetPaperSize(string papersize)
    {
        switch (papersize)
        {
            case "A4":
                foreach (Aspose.Words.Section section in wordDoc)
                {
                    section.PageSetup.PaperSize = PaperSize.A4;
                    section.PageSetup.Orientation = Orientation.Portrait;
                    section.PageSetup.VerticalAlignment = Aspose.Words.PageVerticalAlignment.Top;
                }
                break;
            case "A4H"://A4横向 
                foreach (Aspose.Words.Section section in wordDoc)
                {
                    section.PageSetup.PaperSize = PaperSize.A4;
                    section.PageSetup.Orientation = Orientation.Landscape;
                    section.PageSetup.TextColumns.SetCount(2);
                    section.PageSetup.TextColumns.EvenlySpaced = true;
                    section.PageSetup.TextColumns.LineBetween = true;
                    //section.PageSetup.LeftMargin = double.Parse("3.35"); 
                    //section.PageSetup.RightMargin =double.Parse("0.99"); 
                }
                break;
            case "A3":
                foreach (Aspose.Words.Section section in wordDoc)
                {
                    section.PageSetup.PaperSize = PaperSize.A3;
                    section.PageSetup.Orientation = Orientation.Portrait;
                }
                break;
            case "A3H"://A3横向 
                foreach (Aspose.Words.Section section in wordDoc)
                {
                    section.PageSetup.PaperSize = PaperSize.A3;
                    section.PageSetup.Orientation = Orientation.Landscape;
                    section.PageSetup.TextColumns.SetCount(2);
                    section.PageSetup.TextColumns.EvenlySpaced = true;
                    section.PageSetup.TextColumns.LineBetween = true;
                }
                break;
            case "16K":
                foreach (Aspose.Words.Section section in wordDoc)
                {
                    section.PageSetup.PaperSize = PaperSize.B5;
                    section.PageSetup.Orientation = Orientation.Portrait;

                }
                break;
            case "8KH":
                foreach (Aspose.Words.Section section in wordDoc)
                {
                    section.PageSetup.PageWidth = double.Parse("36.4 ");//纸张宽度 
                    section.PageSetup.PageHeight = double.Parse("25.7");//纸张高度 
                    section.PageSetup.Orientation = Orientation.Landscape;
                    section.PageSetup.TextColumns.SetCount(2);
                    section.PageSetup.TextColumns.EvenlySpaced = true;
                    section.PageSetup.TextColumns.LineBetween = true;
                    //section.PageSetup.LeftMargin = double.Parse("3.35"); 
                    //section.PageSetup.RightMargin = double.Parse("0.99"); 
                }
                break;
            default:break;
        }
    }

    public void PrintWord()
    {
        wordDoc.Print();
    }
}
