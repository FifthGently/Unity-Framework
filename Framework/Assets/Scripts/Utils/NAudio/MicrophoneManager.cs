/*
 * 描述：     MicrophoneManager
 * 作者：     FifthGently
 * 版本：     v1.00
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.IO;

public class MicrophoneManager
{
    private static MicrophoneManager m_instance;

    /// <summary>
    /// 录音设备列表
    /// </summary>
    private string[] micArray = null;

    private AudioClip audioClip;

    private int maxClipLength = 300;
    public int MaxClipLength
    {
        get { return maxClipLength; }
        set { maxClipLength = value; }
    }

    /// <summary>
    /// 录音采样率
    /// </summary>
    private const int Samplerate = 44100;

    private string filepath = string.Empty;

    private int _sampleWindow = 128;

    public MicrophoneManager()
    {
        micArray = Microphone.devices;
        if (micArray.Length == 0)
            Debug.LogError("no mic device");

        //foreach (string deviceStr in Microphone.devices)
        //    Debug.Log("device name = " + deviceStr);
    }

    public static MicrophoneManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new MicrophoneManager();

            return m_instance;
        }
    }

    /// <summary>
    /// 开始录音
    /// </summary>
    public void StartRecord()
    {
        if (micArray.Length == 0)
        {
            Debug.Log("No Record Device!");
            return;
        }
        filepath = string.Empty;

        //录音时先停掉录音
        Microphone.End(null);
        //录音参数为null时采用默认的录音驱动
        audioClip = Microphone.Start(null, false, maxClipLength, Samplerate);
        while (!(Microphone.GetPosition(null) > 0)) { }

        Debug.Log("StartRecord");
    }

    /// <summary>
    /// 停止录音
    /// </summary>
    public string StopRecord()
    {
        if (micArray.Length == 0)
        {
            Debug.Log("No Record Device!");
            return null;
        }

        if (!Microphone.IsRecording(null))
            return null;

        //获取录音样本中的位置(帧数)
        int position = Microphone.GetPosition(null);
        float[] soundData = new float[audioClip.samples * audioClip.channels];
        //用audioClip中的示例数据填充soundData数组,0：从起始位置开始
        audioClip.GetData(soundData, 0);

        //录音文件实际大小的数组，从soundData中copy
        var newData = new float[position * audioClip.channels];
        for (int i = 0; i < newData.Length; i++)
        {
            newData[i] = soundData[i];
        }

        //创建一个适当长度的新AudioClip
        audioClip = AudioClip.Create(audioClip.name,
                                        position,              //帧数
                                        audioClip.channels,     //通道数量
                                        audioClip.frequency,    //频率（赫兹）
                                        false);                 //是否是流式传输

        audioClip.SetData(newData, 0);
        Microphone.End(null);
        Debug.Log("StopRecord");

        SaveToFile();

        Debug.Log(filepath);
        return filepath;
    }

    /// <summary>
    ///播放录音 
    /// </summary>
    public void PlayRecord()
    {
        if (Microphone.IsRecording(null)) return;

        PlayRecord(audioClip);
    }

    public void PlayRecord(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.Log("audioClip is null");
            return;
        }
        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        Debug.Log("PlayRecord ");
    }
    
    public byte[] SaveToBytes()
    {
        if (audioClip == null) return null;
        return WavUtility.FromAudioClip(audioClip);
    }

    public void SaveToFile()
    {
        if (audioClip == null)
        {
            Debug.Log("clip is null,can't be saved");
            return;
        }
        //路径为项目根目录/recordeings(默认的文件夹名)
        WavUtility.FromAudioClip(audioClip, out filepath);
    }

    public AudioClip Read(string path)
    {
        return WavUtility.ToAudioClip(path);
    }

    public AudioClip Read(byte[] data)
    {
        return WavUtility.ToAudioClip(data);
    }

    /// <summary>
    /// 获取麦克风音量
    /// </summary>
    /// <returns></returns>
    public float GetLevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        //NULL表示第一个麦克风
        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1);
        if (micPosition < 0) return 0;
        audioClip.GetData(waveData, micPosition);

        // 在最后128个样本中获得一个峰值
        for (int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }


    /// <summary>
    /// 为了方便回放，又不额外引入其他文件来记录回放时间与录音的对应信息，将录音时间作为文件名（ex:170515-114114-676）,并在开始的时候录一个空录音作为起始时间，回放代码如下：
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayAudioClip()
    {
        string[] files = Directory.GetFiles(filepath);
        if (files.Length > 0)
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Contains(".meta"))
                    temp.Add(files[i]);
            }
            string[] audioFiles = temp.ToArray();
            Array.Sort(audioFiles);
            float interval = 0f;
            for (int i = 0; i < audioFiles.Length - 1; i++)
            {
                AudioClip clip = MicrophoneManager.Instance.Read(audioFiles[i]);
                MicrophoneManager.Instance.PlayRecord(clip);
                //interval = (float)(GetTime(audioFiles[i + 1]) - GetTime(audioFiles[i])).TotalSeconds;
                Debug.Log(interval);
                yield return new WaitForSeconds(interval);
            }
            MicrophoneManager.Instance.PlayRecord
            (
               MicrophoneManager.Instance.Read(audioFiles[audioFiles.Length - 1])
            );
        }
    }
}
