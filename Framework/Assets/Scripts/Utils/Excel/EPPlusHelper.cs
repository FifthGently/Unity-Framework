using System;
using System.Data;
using OfficeOpenXml;
using System.IO;
using UnityEngine;

/// <summary>
/// 使用 EPPlus 第三方的组件读取Excel
/// </summary>
public class EPPlusHelper
{
    private static string GetString(object obj)
    {
        try
        {
            return obj.ToString();
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    /// <summary>
    /// 将指定的Excel的文件转换成DataTable （Excel的第一个sheet）
    /// </summary>
    /// <param name="fullFielPath">文件的绝对路径</param>
    /// <returns></returns>
    public static DataTable ReadExcelToDataTable(string fullFielPath)
    {
        try
        {
            FileInfo existingFile = new FileInfo(fullFielPath);

            ExcelPackage package = new ExcelPackage(existingFile);
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];//选定 指定页

            return ReadExcelToDataTable(worksheet);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// 将worksheet转成datatable
    /// </summary>
    /// <param name="worksheet">待处理的worksheet</param>
    /// <returns>返回处理后的datatable</returns>
    private static DataTable ReadExcelToDataTable(ExcelWorksheet worksheet)
    {
        //获取worksheet的行数
        int rows = worksheet.Dimension.End.Row;
        //获取worksheet的列数
        int cols = worksheet.Dimension.End.Column;

        DataTable dt = new DataTable(worksheet.Name);
        DataRow dr = null;
        for (int i = 1; i <= rows; i++)
        {
            if (i > 1)
                dr = dt.Rows.Add();

            for (int j = 1; j <= cols; j++)
            {
                //默认将第一行设置为datatable的标题
                if (i == 1)
                    dt.Columns.Add(GetString(worksheet.Cells[i, j].Value));
                //剩下的写入datatable
                else
                    dr[j - 1] = GetString(worksheet.Cells[i, j].Value);
            }
        }
        return dt;
    }

    /// <summary>
    /// 将excel中的数据导入到DataTable中
    /// </summary>
    /// <param name="fileName">excel文件的名称</param>
    /// <param name="sheetName">excel工作薄sheet的名称</param>
    /// <param name="isFirstRowColumn">第一行是否是属性</param>
    /// <returns>返回的DataTable</returns>
    public static DataTable ReadExcelToDataTable(string fileName, string sheetName, bool isFirstRowColumn)
    {
        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(sheetName)) return null;
        try
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var package = new ExcelPackage(fs);
                DataTable data = new DataTable();

                ExcelWorkbook workBook = package.Workbook;
                if (workBook != null)
                {
                    if (workBook.Worksheets.Count > 0)
                    {
                        ExcelWorksheet currentWorksheet = workBook.Worksheets[sheetName];

                        int lastRow = currentWorksheet.Dimension.End.Row;
                        int lastColumn = currentWorksheet.Dimension.End.Column;

                        int columnCount = 1;
                        while (columnCount <= lastColumn)
                        {
                            data.Columns.Add(Convert.ToString(currentWorksheet.Cells[1, columnCount].Value));
                            columnCount++;
                        }

                        int rowCount = 0;
                        if (isFirstRowColumn) rowCount = currentWorksheet.Dimension.Start.Row + 1;
                        else rowCount = currentWorksheet.Dimension.Start.Row;

                        while (rowCount <= lastRow)
                        {
                            columnCount = 1;
                            DataRow newRow = data.NewRow();
                            while (columnCount <= lastColumn)
                            {
                                newRow[data.Columns[columnCount - 1]] =
                                    Convert.ToString(currentWorksheet.Cells[rowCount, columnCount].Value);
                                columnCount++;
                            }
                            rowCount++;
                            data.Rows.Add(newRow);
                        }
                    }
                    fs.Close();
                    fs.Dispose();
                }
                return data;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return null;
        }
    }

    public static void WriteExcelFromDateTable(string fullFielPath, DataTable dt)
    {
        try
        {
            FileInfo existingFile = new FileInfo(fullFielPath);

            ExcelPackage package = new ExcelPackage(existingFile);
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];//选定 指定页

            int rows = dt.Rows.Count;
            int cols = dt.Columns.Count;

            worksheet.InsertRow(2, rows);
            //worksheet.InsertColumn(1, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols - 1; j++)
                {
                    worksheet.SetValue(i + 2, j + 1, dt.Rows[i][j].ToString());
                }
            }

            package.Save();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
