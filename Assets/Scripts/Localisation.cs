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
            ["Рунный волк"] = "Rune wolf",
            ["Как играть?"] = "How to play?",
            ["Играть"] = "Play",
            ["Волна"] = "Wave",
            ["Обычный кубик"] = "Default dice",
            ["Огненный кубик"] = "Fire dice",
            ["Электрический кубик"] = "Electro dice",
            ["Ветряной кубик"] = "Wind dice",
            ["Способность"] = "Ability",
            ["Мечник"] = "Swordsman",
            ["Маг"] = "Mage",
            ["Рыцарь"] = "Knight",
            ["Огненный мечник"] = "Fire Swordsman",
            ["Маг молнии"] = "Electro mage",
            ["В меню"] = "Main Menu",
            ["Оценить игру!"] = "Rate Game!",
            ["Вы проиграли"] = "You lose",
            ["Вы прожили волн: "] = "Waves you lived: ",
            ["Понял"] = "Got it",
            ["Авторизация"] = "Authorization",
            ["Для работы лидерборда необходимо авторизоваться!"] = "You must be logged in to use the leaderboard!",
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
