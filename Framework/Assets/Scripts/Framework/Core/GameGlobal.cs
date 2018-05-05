using System;
using System.Collections.Generic;
using UnityEngine;

public class GameGlobal
{
    public static bool debugMode = false;
    public static long TIME_START_GAME = 0;
    public static string LAST_GAME_SERVER_ADDRESS = string.Empty;
    public static ushort LAST_GAME_SERVER_PORT = 0;
    public static ushort LAST_GAME_SERVER_ZONE = 0;

    public static ulong UserAccountNo = 0;
    public static ulong GuestUserAccountNo = 0;

    public static string VERSION = "V1.0.0";
    public static int PLATFORM_CODE = 9;


    public static void LoadGameAccount()
    {
        string strUserAccountNo = PlayerPrefs.GetString("USER_ACCOUNT_NO");
        if (strUserAccountNo != string.Empty)
            UserAccountNo = Convert.ToUInt64(strUserAccountNo);
        else
            UserAccountNo = 0;

        strUserAccountNo = PlayerPrefs.GetString("GUEST_ACCOUNT_NO");
        if (strUserAccountNo != string.Empty)
            GuestUserAccountNo = Convert.ToUInt64(strUserAccountNo);
        else
            GuestUserAccountNo = 0;
    }

    public static void Clearup()
    {
        PlayerPrefs.DeleteAll();
        GameGlobal.UserAccountNo = 0;
        GameGlobal.GuestUserAccountNo = 0;
    }

    public static void SaveGameAccount()
    {
        string strUserAccountNo = Convert.ToString(UserAccountNo);
        PlayerPrefs.SetString("USER_ACCOUNT_NO", strUserAccountNo);

        strUserAccountNo = Convert.ToString(GuestUserAccountNo);
        PlayerPrefs.SetString("GUEST_ACCOUNT_NO", strUserAccountNo);

        PlayerPrefs.Save();
    }

    public static void SaveLastGameServerZone()
    {
        PlayerPrefs.SetInt("LAST_GAME_SERVER_ZONE", (int)LAST_GAME_SERVER_ZONE);
        PlayerPrefs.Save();
    }

    public static void SaveLoginUserList(string userInfoList)
    {
        PlayerPrefs.SetString("LOGIN_USER_LIST", userInfoList);
        PlayerPrefs.Save();
    }

    public static string LoadLoginUserList()
    {
        return PlayerPrefs.GetString("LOGIN_USER_LIST");
    }

    public static int LoadOptionMusic()
    {
        return PlayerPrefs.GetInt("USER_OPT_MUSIC", 1);
    }

    public static void SaveOptionMusic(bool value)
    {
        PlayerPrefs.SetInt("USER_OPT_MUSIC", value ? 1 : 0);
    }

    public static int LoadOptionSound()
    {
        return PlayerPrefs.GetInt("USER_OPT_SOUND", 1);
    }

    public static void SaveOptionSound(bool value)
    {
        PlayerPrefs.SetInt("USER_OPT_SOUND", value ? 1 : 0);
    }

    public static void SetShieldingPlayer(string StrPlayer)
    {
        PlayerPrefs.SetString("SHIELDPLATER", StrPlayer);
        PlayerPrefs.Save();
    }

    public static string GetShieldingPlayer()
    {
        return PlayerPrefs.GetString("SHIELDPLATER");
    }

    public static void SetFirstBuyChat(int bIsfirstCount)
    {
        PlayerPrefs.SetInt("BISFIRSTBUY", bIsfirstCount);
        PlayerPrefs.Save();
    }

    public static int GetFirstBuyChat()
    {
        return PlayerPrefs.GetInt("BISFIRSTBUY");
    }
}

