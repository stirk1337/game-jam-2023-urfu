using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ExpRune : MonoBehaviour
{
    Player player;
    [SerializeField] int expValue;
    [SerializeField] int hpValue;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] int playerRangeUpgradedTimes;
    [SerializeField] int abilityCooldownUpgradedTimes;
    [SerializeField] int abilityRangeUpgradedTimes;
    [SerializeField] int diceUpgradedTimes;
    [SerializeField] int abilityDamageUpdradedTimes;

    
    Ability[] abilities;
    DiceManager diceManager;
    ExpRuneManager expRuneManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Collect();
            Destroy(gameObject);
        }
    }

    void Collect()
    {
        player.health = Mathf.Clamp(player.health + hpValue, -10, player.maxHealth);
        player.exp += expValue;
        expRuneManager.CollectSound();
        

        if (player.exp >= 10)
        {
            Upgrade();
            
            player.exp -= 10;
        }
    }


    void Upgrade()
    {
        int random = Random.Range(0, 8);
        expRuneManager.SoundLevelUp();
        //random = 4;

        switch (random)
        {
            case (0):
                if (playerRangeUpgradedTimes < 2)
                {
                    player.range += 1;
                    playerRangeUpgradedTimes += 1;
                    player.LevelUpString = "+1 к дальности атаки";
                }
                else
                {
                    Upgrade();
                }
                break;

            case (1):
                if (abilityCooldownUpgradedTimes < 1)
                {
                    foreach (Ability ability in abilities)
                    {
                        ability.cooldown -= 5;
                        abilityCooldownUpgradedTimes += 1;
                        player.LevelUpString = "-5 к скорости перезарядке способностей";
                    }
                }
                else
                {
                    Upgrade();
                }
                break;

            case (2):
                if (abilityRangeUpgradedTimes < 1)
                {
                    foreach (Ability ability in abilities)
                    {
                        ability.range += 1;
                        abilityRangeUpgradedTimes += 1;
                        player.LevelUpString = "+1 к дальности атаки способностей";
                    }
                }
                else
                {
                    Upgrade();
                }
                break;

            case (3):
                hpValue += 5;
                player.LevelUpString = "+5 здоровья от рун";
                break;

            case (4):
                if (diceUpgradedTimes < 2)
                {
                    for(int i = 0; i < diceManager.playerDice.Count; i++)
                    {
                        if (diceManager.playerDice[i] == DiceManager.DiceState.Miss)
                        {
                            diceManager.playerDice[i] = DiceManager.DiceState.Hit;
                            break;
                        }                 
                    }
                    diceUpgradedTimes += 1;
                    player.LevelUpString = "Улучшение обычного кубика";
                }
                else
                {
                    Upgrade();
                }
                break;
            case (5):
                player.health = player.maxHealth;
                player.LevelUpString = "Исцеление";
                break;
            case (6):
                player.damage += 10;
                player.LevelUpString = "+10 к урону";
                break;
            case (7):
                if (abilityDamageUpdradedTimes < 1)
                {
                    foreach (Ability ability in abilities)
                    {
                        ability.damage += 5;
                        abilityDamageUpdradedTimes += 1;
                        player.LevelUpString = "+5 к урону способностей";
                    }
                }
                else
                {
                    Upgrade();
                }
                break;

        }
        
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        abilities = FindObjectsOfType<Ability>();
        diceManager = FindObjectOfType<DiceManager>();
        expRuneManager = FindObjectOfType<ExpRuneManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
