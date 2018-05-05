using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Frameworks;
using SpeechLib;


namespace Frameworks
{
    public class SoundManager
    {
        private float m_defaultVloume = 1.0f;
        private bool m_bBGMEnable = true;
        private bool m_bSoundEnable = true;
        private AudioListener m_cAudioListener;
        private AudioSource m_cAudioSource;
        private AudioClip m_cReadyAudioClip;
        private float m_fMaxScaleTime = 2f;
        private float m_fPitch = 1f;
        private float m_fStartScaleTime;
        private float m_fVloume = 1f;
        private int m_iCurBGMType;
        private string m_strLastBGMMusic = string.Empty;
        private bool m_bLoadingBGM = false;
        private AudioClip _MainTheme;
        private float BestVoulme = 1f;
        private float MinVoulme = 0.3f;

        private SpVoice _voice = null;
        private bool voiceEnble = true;
        private SpVoice voice
        {
            get
            {
                if (voiceEnble == false) return null;

                if (_voice == null)
                {
                    try
                    {
                        _voice = new SpVoice();
                        
                    }
                    catch (Exception)
                    {
                        voiceEnble = false;
                    }
                }
                return _voice;
            }
        }

        private static SoundManager m_soundManager = null;
        public static SoundManager GetSoundManager()
        {
            if (m_soundManager == null)
            {
                m_soundManager = new SoundManager();
            }
            return m_soundManager;
        }

        public void Initialize()
        {
            _MainTheme = new AudioClip();
            this.m_cAudioListener = UnityEngine.Object.FindObjectOfType(typeof(AudioListener)) as AudioListener;
            if (this.m_cAudioListener == null)
            {
                Camera main = Camera.main;
                if (main == null)
                {
                    main = UnityEngine.Object.FindObjectOfType(typeof(Camera)) as Camera;
                }
                if (main != null)
                {
                    this.m_cAudioListener = main.gameObject.AddComponent<AudioListener>();
                }
            }
            //this.m_cAudioSource = this.m_cAudioListener.audio;
            if (this.m_cAudioSource == null)
            {
                this.m_cAudioSource = this.m_cAudioListener.gameObject.AddComponent<AudioSource>();
            }

            if (GameGlobal.LoadOptionSound() == 0)
            {
                EnableSoundEffect = false;
            }

            if (GameGlobal.LoadOptionMusic() == 0)
            {
                this.EnableBGM = false;
            }
        }

        public AudioSource PlayBGM(int type, string snd)
        {
            m_strLastBGMMusic = snd;

            if (!this.m_bBGMEnable) return null;

            if (type != this.m_iCurBGMType)
            {
                this.m_iCurBGMType = type;
            }
            if (m_bLoadingBGM)
                return this.m_cAudioSource;

            if (snd == "BGM_Theme" && _MainTheme != null)
            {
                if (this.m_cAudioSource.isPlaying)
                {
                    this.m_cReadyAudioClip = _MainTheme;
                    this.m_fStartScaleTime = GameGUIUtility.GameRunTime();
                }
                else
                {
                    this.m_cAudioSource.clip = _MainTheme;
                    this.m_cAudioSource.volume = this.m_fVloume;
                    this.m_cAudioSource.loop = true;
                    this.m_cAudioSource.Play();
                }
                return m_cAudioSource;
            }

            m_bLoadingBGM = true;

            return this.m_cAudioSource;
        }
        public List<string> TextList=new List<string>();
        bool isSpeak = false;


        public void TextToSoundPlay(string txt, int _Speaker = 0, int _Rate = -1, int _Volume = 100) 
        {
           
            if (txt == null) { return; }
            if (voice.Status.RunningState == SpeechRunState.SRSEIsSpeaking) return;
            if (voiceEnble == false || voice == null) { Debug.LogError(voiceEnble + "\t" + voice); return; }
            try
            {
                //voice.Voice = voice.GetVoices(string.Empty, string.Empty).Item(_Speaker);
                voice.Rate = _Rate;
                voice.Volume = _Volume;
                voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                voice.Speak(txt, SpeechVoiceSpeakFlags.SVSFlagsAsync);
                isSpeak = true;
                //voice.Status = SpeechRunState.SRSEIsSpeaking;
            }
            catch (Exception e)
            {
                GameLog.LogError("Error:" + e);
            }
        }
        public void TextToSoundStop() {
            voice.Rate = 50;
            voice.Volume = 0;
        }
        public void TextToSoundPlay1(string txt, int _Speaker=0, int _Rate=-1, int _Volume=100)
        {
            TextList.Add(txt);
           // Debug.LogError(TextList.Count);
            //if (txt ==null) { return; }
            //if (voice.Status.RunningState == SpeechRunState.SRSEIsSpeaking) return;
            //if (voiceEnble == false || voice == null) { Debug.LogError(voiceEnble + "\t" + voice); return; }
            //try
            //{               
            //    voice.Voice = voice.GetVoices(string.Empty, string.Empty).Item(_Speaker);
            //    voice.Rate = _Rate;
            //    voice.Volume = _Volume;              
            //    voice.Speak(string.Empty, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);                
            //    voice.Speak(txt, SpeechVoiceSpeakFlags.SVSFlagsAsync);                           
            //}
            //catch (Exception e)
            //{
            //    GameLog.LogError("Error:" + e);
            //}
        }

        IEnumerator TextToSound(string txt)
        {
            voice.Speak(txt, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            yield return new WaitForSeconds(1000f);
        }

        public AudioSource PlayResBGM(int type, string snd)
        {
            m_strLastBGMMusic = snd;
            if (!this.m_bBGMEnable)
            {
                return null;
            }
            if (type != this.m_iCurBGMType)
            {
                this.m_iCurBGMType = type;
            }
            else if (this.m_cAudioSource.isPlaying)
            {
                return null;
            }
            AudioClip clip = Resources.Load("music/" + snd, typeof(AudioClip)) as AudioClip;
            if (clip == null)
            {
                // GameLog.LogError("Sound not found!!");
                return null;
            }
            if (this.m_cAudioListener == null)
            {
                return null;
            }
            if (this.m_cAudioSource.isPlaying)
            {
                this.m_cReadyAudioClip = clip;
                this.m_fStartScaleTime = GameGUIUtility.GameRunTime();
            }
            else
            {
                this.m_cAudioSource.clip = clip;
                this.m_cAudioSource.volume = this.m_fVloume;
                this.m_cAudioSource.loop = true;
                this.m_cAudioSource.Play();
            }
            return this.m_cAudioSource;
        }

        public void DelayPlaySound(string snd, float delay)
        {
            GameManager.Main.StartCoroutine(EnumPlaySound(snd, delay));
        }

        IEnumerator EnumPlaySound(string snd, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySound(snd);
        }

        public AudioSource PlayVoice(string snd)
        {
            return PlaySound(snd);
        }

        public void StopPlayVoice()
        {
            m_cAudioSource.Stop();
        }

        public AudioSource PlaySound(string snd)
        {
            if (!this.EnableSoundEffect)
            {
                return null;
            }
            if (snd == string.Empty)
            {
                return null;
            }

            /* MainClient.StartCoroutine(ResourcesManager.CoroutineLoad(Global.LOCAL_RES_URL + "music/sound/" + snd + ".mp3", loader =>
             {
                 AudioClip clip = loader.audioClip;
                 if (clip == null)
                 {
                     GameLog.LogError("Sound not found!!");
                     return;
                 }
                 if (this.m_cAudioListener != null)
                 {
                     this.m_cAudioSource.PlayOneShot(clip, this.m_defaultVloume);
                 }
             }));   */
            return this.m_cAudioSource;
        }

        public AudioSource PlayResSound(string snd)
        {
            if (!this.EnableSoundEffect)
                return null;

            AudioClip clip = Resources.Load("music/sound/" + snd, typeof(AudioClip)) as AudioClip;
            if (clip == null)
            {
                GameLog.LogError("Sound not found!!");
                return null;
            }
            if (this.m_cAudioSource.isPlaying)
            {
                if (this.m_cAudioListener != null)
                {
                    GameManager.Main.StartCoroutine(GraduallyChange(clip));
                }
            }
            else
            {
                if (this.m_cAudioListener != null)
                {
                    this.m_cAudioSource.PlayOneShot(clip, this.m_defaultVloume);
                    return this.m_cAudioSource;
                }
            }
            return null;
        }

        public void StopBGM()
        {
            if ((this.m_cAudioSource != null) && this.m_cAudioSource.isPlaying)
            {
                this.m_cAudioSource.Stop();
            }
        }

        public void Update()
        {
           // Debug.LogError(TextList.Count);
            if (TextList.Count!=0)
            {
               // Debug.LogError(TextList[0]);
                TextToSoundPlay1(TextList[0]);
                if (voice.Status.RunningState == SpeechRunState.SRSEDone && isSpeak)
                {
                   // Debug.LogError("suowanle");
                    //TextList.RemoveAt(0);
                    isSpeak = false;
                } 
            }

            if (this.m_cAudioSource != null)
            {
                if (this.m_cReadyAudioClip != null)
                {
                    float num = GameGUIUtility.GameRunTime() - this.m_fStartScaleTime;
                    float num2 = 1f - (num / this.m_fMaxScaleTime);
                    if (num2 <= 0f)
                    {
                        this.m_cAudioSource.clip = this.m_cReadyAudioClip;
                        this.m_cAudioSource.volume = this.m_fVloume;
                        this.m_cAudioSource.Play();
                        this.m_cReadyAudioClip = null;
                    }
                    else
                    {
                        float num3 = this.m_fVloume * num2;
                        if (Mathf.Abs((float)(this.m_cAudioSource.volume - num3)) > 0.1f)
                        {
                            this.m_cAudioSource.volume = this.m_fVloume * num2;
                        }
                    }
                }
            }
        }
        IEnumerator GraduallyChange(AudioClip m_clip)
        {
            while (m_cAudioSource.volume > MinVoulme)
            {
                m_cAudioSource.volume -= 0.01f;
                yield return new WaitForSeconds(0.03f);
            }
            m_cAudioSource.Stop();
            yield return new WaitForSeconds(0.5f);
            m_cAudioSource.PlayOneShot(m_clip);
            while (m_cAudioSource.volume < BestVoulme)
            {
                m_cAudioSource.volume += 0.01f;
                yield return new WaitForSeconds(0.03f);
            }
        }

        public bool BGMEnable
        {
            get
            {
                return this.m_bBGMEnable;
            }
            set
            {
                this.m_bBGMEnable = value;
            }
        }

        public float FPitch
        {
            get
            {
                return this.m_fPitch;
            }
            set
            {
                this.m_fPitch = value;
            }
        }

        public float FVloume
        {
            get
            {
                return this.m_fVloume;
            }
            set
            {
                this.m_fVloume = value;
                if (m_cAudioSource != null)
                {
                    m_cAudioSource.volume = FVloume;
                }
            }
        }
        public void OpenVloume(bool play)
        {
            if (play)
                FVloume = m_defaultVloume;
            else
                FVloume = 0.0f;
        }

        public bool EnableSoundEffect
        {
            set
            {
                m_bSoundEnable = value;
            }
            get
            {
                return m_bSoundEnable;
            }
        }

        public bool EnableBGM
        {
            set
            {
                m_bBGMEnable = value;
                if (!m_bBGMEnable)
                    StopBGM();
                else
                {
                    if (m_strLastBGMMusic != string.Empty)
                        PlayBGM(0, m_strLastBGMMusic);
                }
            }
            get
            {
                return m_bBGMEnable;
            }
        }
    }
}