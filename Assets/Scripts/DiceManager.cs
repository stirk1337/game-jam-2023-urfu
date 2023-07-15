using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DiceManager : MonoBehaviour
{
    public enum DiceState
    {
        Hit,
        Miss,
        Crit,
        Electro,
        Wind
    }

    [SerializeField] Player player;
    [SerializeField] Image diceSprite;
    [SerializeField] Image diceRange;
    [SerializeField] Image currentCube;
    [SerializeField] TextMeshProUGUI cubeText;
    [SerializeField] GameObject cubeCanvas;

    [SerializeField] Sprite defaultDiceRangeMiss2;
    [SerializeField] Sprite defaultDiceRangeMiss1;
    [SerializeField] Sprite defaultDiceRangeMiss0;
    [SerializeField] Sprite fireDiceRange;
    [SerializeField] Sprite electroDiceRange;
    [SerializeField] Sprite windDiceRange;


    public List<DiceState> defaultDice;
    public List<DiceState> playerDice;
    public List<DiceState> fireDice;
    public List<DiceState> electroDice;
    public List<DiceState> windDice;

    [SerializeField] Canvas enemyCanvas;
    Localisation localisation;

    public void SelectDice(string diceElement)
    {
        Enum.TryParse(diceElement, out player.diceElement);
    }

    public void TurnCanvases()
    {
        enemyCanvas.gameObject.SetActive(false);
        cubeCanvas.SetActive(true);
    }


    void Start()
    {
        localisation = FindObjectOfType<Localisation>();
        defaultDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Miss, DiceState.Miss };
        playerDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Miss, DiceState.Miss };
        fireDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Crit, DiceState.Crit, DiceState.Miss };
        electroDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Electro, DiceState.Electro };
        windDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Crit, DiceState.Wind, DiceState.Wind };
    }

    void UpdateVisualCurrentCube()
    {
        diceSprite.sprite = currentCube.sprite;

        switch (player.diceElement)
        {
            case Player.AbilityElement.Default:
                cubeText.text = localisation.GetTranslate("Обычный кубик");
                var countOfMisses = playerDice.Where(x => x == DiceState.Miss);
                if (countOfMisses.Count() == 2)
                {
                    diceRange.sprite = defaultDiceRangeMiss2;
                }
                else if (countOfMisses.Count() == 1)
                {
                    diceRange.sprite = defaultDiceRangeMiss1;
                }
                else
                {
                    diceRange.sprite = defaultDiceRangeMiss0;
                }
                break;
            case Player.AbilityElement.Fire:
                cubeText.text = localisation.GetTranslate("Огненный кубик");
                diceRange.sprite = fireDiceRange;
                break;
            case Player.AbilityElement.Electro:
                cubeText.text = localisation.GetTranslate("Электрический кубик"); ;
                diceRange.sprite = electroDiceRange;
                break;
            case Player.AbilityElement.Wind:
                cubeText.text = localisation.GetTranslate("Ветряной кубик");
                diceRange.sprite = windDiceRange;
                break;
            
        }
    }

    void Update()
    {
        UpdateVisualCurrentCube();
    }
}
