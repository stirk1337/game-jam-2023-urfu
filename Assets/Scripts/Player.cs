using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Enemy;

public class Player : MonoBehaviour
{
    public enum AbilityElement
    {
        Default,
        Fire,
        Electro,
        Wind
    }


    [SerializeField] public int maxHealth;
    [SerializeField] public int health;
    [SerializeField] public int exp;
    [SerializeField] public int damage;
    [SerializeField] public int shield;
    [SerializeField] public int range;
    [SerializeField] public int currentTurn;
    [SerializeField] public PlayerMove playerMove;
    [SerializeField] public AbilityElement abilityElement;
    [SerializeField] public AbilityElement diceElement;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI shieldText;
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] TextMeshProUGUI rangeText;



    [SerializeField] TextMeshProUGUI runeText;
    [SerializeField] TextMeshProUGUI expText;
    [SerializeField] TextMeshProUGUI levelUpText;
    [SerializeField] public string LevelUpString;


    [SerializeField] public Dictionary<Enemy.ElementState, int> state;
    [SerializeField] GameObject fireImage;
    [SerializeField] GameObject electroImage;
    [SerializeField] GameObject windImage;
    [SerializeField] GameObject defaultCubeImage;
    [SerializeField] GameObject fireCubeImage;
    [SerializeField] GameObject electroCubeImage;
    [SerializeField] GameObject windCubeImage;
    public Dictionary<AbilityElement, GameObject> currentDices;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            playerMove.targetPos = new Vector2(MathF.Floor(playerMove.transform.position.x) + 0.5f, MathF.Floor(playerMove.transform.position.y) + 0.5f);
        }
    }

    public void TakeDamageWithCube(Tuple<int, DiceManager.DiceState> tuple)
    {
        health -= Mathf.Clamp(tuple.Item1 - shield, 0, 100000);
        //Debug.Log(tuple.Item1 + tuple.Item2.ToString());
        switch (tuple.Item2)
        {
            case DiceManager.DiceState.Electro:
                state[ElementState.Electro] = 3;
                break;
            case DiceManager.DiceState.Wind:
                //State.Instance.FreeMove = true;
                break;
        }
        if (health <= 0)
            Die();
    }

    public void TakeDamageWithoutCube(int damage)
    {
        health -= Mathf.Clamp(damage, 0, 100000);
        //Debug.Log(tuple.Item1 + tuple.Item2.ToString());
        if (health <= 0)
            Die();
    }

    void UpdateVisual()
    {
        healthText.text = health.ToString();
        shieldText.text = shield.ToString();
        damageText.text = damage.ToString();
        rangeText.text = range.ToString();
        runeText.text = "агЭР: " + abilityElement.ToString();
        expText.text = exp.ToString() + "/10";
        levelUpText.text = LevelUpString;

        var stateImages = new Dictionary<ElementState, GameObject>()
        {
            [ElementState.Fire] = fireImage,
            [ElementState.Electro] = electroImage,
            [ElementState.Wind] = windImage,
        };
        foreach (var st in state)
        {
            if (st.Key == ElementState.Default)
                continue;
            if (st.Value > 0)
            {
                stateImages[st.Key].SetActive(true);
            }
            else
            {
                stateImages[st.Key].SetActive(false);
            }
        }
    }

    private void Awake()
    {
        state = new Dictionary<ElementState, int>()
        {
            [ElementState.Electro] = 0,
            [ElementState.Fire] = 0,
            [ElementState.Wind] = 0,
            [ElementState.Default] = 0,
        };
        currentDices = new Dictionary<AbilityElement, GameObject>()
        {
            [AbilityElement.Default] = defaultCubeImage,
            [AbilityElement.Fire] = fireCubeImage,
            [AbilityElement.Wind] = windCubeImage,
            [AbilityElement.Electro] = electroCubeImage
        };
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVisual();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
