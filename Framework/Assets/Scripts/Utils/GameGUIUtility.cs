using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Frameworks
{
    public sealed class GameGUIUtility
    {

        public static long DateToUNIXTime(string str)
        {
            DateTime time2 = new DateTime(0x7b2, 1, 1, 0, 0, 0);
            return ((DateTime.ParseExact(str, "yyyy-MM-dd", null).Ticks - time2.Ticks) / 0x2710L);
        }

        public static string EncodeToUTF8(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Encoding.UTF8.GetString(bytes);
        }

        public static float GameRunTime()
        {
            return (((float)(DateTime.Now.Ticks - GameGlobal.TIME_START_GAME)) / 1E+07f);
        }

        public static byte[] Unicode2UTF8(string value)
        {
            UnicodeEncoding unicodeEncode = new UnicodeEncoding();
            UTF8Encoding utf8Encode = new UTF8Encoding();
            byte[] defaultBytes = unicodeEncode.GetBytes(value);
            byte[] utf8Bytes = Encoding.Convert(unicodeEncode, utf8Encode, defaultBytes);

            return utf8Bytes;
        }

        public static string UTF82Unicode(byte[] utf8Bytes)
        {
            UnicodeEncoding unicodeEncode = new UnicodeEncoding();
            UTF8Encoding utf8Encode = new UTF8Encoding();
            byte[] defaultBytes = Encoding.Convert(utf8Encode, unicodeEncode, utf8Bytes, 0, GameGUIUtility.GetBytesNonLen(utf8Bytes));
            return unicodeEncode.GetString(defaultBytes);
        }
        
        public static int GetBytesNonLen(byte[] bytes)
        {
            int len = bytes.Length;
            for (int n = 0; n < bytes.Length; ++n)
            {
                if (bytes[n] == 0)
                {
                    len = n;
                    break;
                }
            }
            return len;
        }
        public static string UTF82Default(byte[] utf8Bytes)
        {            
            return UTF82Unicode(utf8Bytes);
            //UTF8Encoding utf8Encode = new UTF8Encoding();            
            //byte[] defaultBytes = Encoding.Convert(utf8Encode, Encoding.Default, utf8Bytes, 0, GameGUIUtility.GetBytesNonLen(utf8Bytes));
            //return Encoding.Default.GetString(defaultBytes);
        }

        public static uint MakeDword(ushort hi, ushort low)
        {
            uint value =  ((uint)hi << 16) | low;
            return value;
        }

        public static ushort MakeWord(byte hi, byte low)
        {
            ushort value = (ushort)(((uint)hi << 8) | low);
            return value;
        }

        public static ushort HiWord(uint dword)
        {
            ushort value = (((ushort)((((uint)(dword)) >> 16) & 0xffff)));

            return value;
        }

        public static ushort LowWord(uint dword)
        {
            ushort value = (((ushort)(((uint)(dword)) & 0xffff)));
            return value;
        }

        public static Color ToColor(string color)
        {

            int red, green, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1].ToString() + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2].ToString() + rgb[2].ToString(), 16);
                    return new Color(red, green, blue);
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2].ToString() + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4].ToString() + rgb[5].ToString(), 16);
                    return new Color(red/255.0f, green/255.0f, blue/255.0f);
                default:
                    return new Color();

            }
        }

    }
}