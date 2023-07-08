using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

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

    public List<DiceState> defaultDice;
    public List<DiceState> fireDice;
    public List<DiceState> electroDice;
    public List<DiceState> windDice;


    public void SelectDice(string diceElement)
    {
        Enum.TryParse(diceElement, out player.diceElement);
    }

    void Start()
    {
        defaultDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Miss, DiceState.Miss };
        fireDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Crit, DiceState.Crit, DiceState.Miss };
        electroDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Electro, DiceState.Electro };
        windDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Crit, DiceState.Wind, DiceState.Wind };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
