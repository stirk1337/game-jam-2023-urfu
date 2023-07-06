using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    // Start is called before the first frame update

    public enum DiceState
    {
        Hit,
        Miss,
        Crit,
        ElectroHit,
        WindHit
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
        electroDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Crit, DiceState.ElectroHit, DiceState.ElectroHit };
        windDice = new List<DiceState>() { DiceState.Hit, DiceState.Hit, DiceState.Hit, DiceState.Crit, DiceState.WindHit, DiceState.WindHit };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
