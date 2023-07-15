using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Yandex : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern void RateGame();

    [DllImport("__Internal")]
    private static extern void Auth();

    public void RateGameButton()
    {
        RateGame();
    }

    public void AuthButton()
    {
        Auth();
    }
}
