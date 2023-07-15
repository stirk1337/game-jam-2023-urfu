using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Language : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern string GetLang();

    public string CurrentLanguage; // ru en
    [SerializeField] public static Language Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            #if UNITY_EDITOR
                //Debug.Log("Unity Editor");
            #else
                CurrentLanguage = GetLang();
            #endif


        }
        else
        {
            Destroy(gameObject);
        }
    }

}
