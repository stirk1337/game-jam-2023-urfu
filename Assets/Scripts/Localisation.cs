using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Localisation : MonoBehaviour
{
    // Start is called before the first frame update
    Dictionary<string, string> eng;
    [SerializeField] Image learnToPlay;
    [SerializeField] Sprite learnToPlayEng;

    void Start()
    {
        eng = new Dictionary<string, string>()
        {
            ["������ ����"] = "Rune wolf",
            ["��� ������?"] = "How to play?",
            ["������"] = "Play",
            ["�����"] = "Wave",
            ["������� �����"] = "Default dice",
            ["�������� �����"] = "Fire dice",
            ["������������� �����"] = "Electro dice",
            ["�������� �����"] = "Wind dice",
            ["�����������"] = "Ability",
            ["������"] = "Swordsman",
            ["���"] = "Mage",
            ["������"] = "Knight",
            ["�������� ������"] = "Fire Swordsman",
            ["��� ������"] = "Electro mage",
            ["� ����"] = "Main Menu",
            ["������� ����!"] = "Rate Game!",
            ["�� ���������"] = "You lose",
            ["�� ������� ����: "] = "Waves you lived: ",
            ["�����"] = "Got it",
            ["�����������"] = "Authorization",
            ["��� ������ ���������� ���������� ��������������!"] = "You must be logged in to use the leaderboard!",
        };

        if (Language.Instance.CurrentLanguage == "en")
        {
            TranslateTMP();
            if (learnToPlayEng != null)
                learnToPlay.sprite = learnToPlayEng;
        }
    }

    public string GetTranslate(string text)
    {
        if (Language.Instance.CurrentLanguage == "en")
        {
            return eng[text];
        }
        return text;
    }

    void TranslateTMP() 
    {
        TextMeshProUGUI[] texts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            //Debug.Log(text.text);
            if (eng.ContainsKey(text.text))
            {
                //Debug.Log(text.text);
                text.text = eng[text.text];
                //Debug.Log(text.text);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
