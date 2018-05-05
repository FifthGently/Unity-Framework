using System;
using System.IO;

public class FileWriter
{
    private string _fileFullPath = "";
    private FileStream _fs;
    private bool _isLogDate;
    private StreamWriter _sw;

    public FileWriter(string fullPath)
    {
        this._fileFullPath = fullPath;
        this.Open();
    }

    public void Close()
    {
        if (this._sw != null)
        {
            this._sw.Close();
            this._sw = null;
        }
        if (this._fs != null)
        {
            this._fs.Close();
            this._fs = null;
        }
    }

    ~FileWriter()
    {
        this.Close();
    }

    public bool isLogDate
    {
        get { return this._isLogDate; }
        set { this._isLogDate = value; }
    }

    public void Flush()
    {
        this._sw.Flush();
    }

    private void Open()
    {
        try
        {
            this._fs = new FileStream(this._fileFullPath, FileMode.Append, FileAccess.Write, FileShare.Read);
            this._sw = new StreamWriter(this._fs);
            File.SetAttributes(this._fileFullPath, FileAttributes.Normal);
        }
        catch (UnauthorizedAccessException)
        {
            this.Close();
            throw;
        }
        catch (Exception)
        {
            this.Close();
            throw;
        }
    }

    public void Write(string text)
    {
        this._sw.Write(text);
    }

    public void WriteLine(string text)
    {
        if (this._isLogDate)
        {
            this._sw.WriteLine(DateTime.Now.ToLocalTime().ToString() + " " + text);
        }
        else
        {
            this._sw.WriteLine(text);
        }
    }
}
