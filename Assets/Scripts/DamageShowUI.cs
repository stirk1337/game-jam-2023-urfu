using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageShowUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] Image diceStateImageIcon;
    [SerializeField] Sprite hit;
    [SerializeField] Sprite electro;
    [SerializeField] Sprite miss;
    [SerializeField] Sprite crit;
    [SerializeField] Sprite wind;

    IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }


    public void Init(Tuple<int, DiceManager.DiceState> tuple)
    {
        damageText.text = tuple.Item1.ToString();

        switch (tuple.Item2)
        {
            case DiceManager.DiceState.Hit:
                diceStateImageIcon.sprite = hit;
                break;

            case DiceManager.DiceState.Crit:
                diceStateImageIcon.sprite = crit;
                break;

            case DiceManager.DiceState.Electro:
                diceStateImageIcon.sprite = electro;
                break;

            case DiceManager.DiceState.Wind:
                diceStateImageIcon.sprite = wind;
                break;

            case DiceManager.DiceState.Miss:
                diceStateImageIcon.sprite = miss;
                break;

        }
        StartCoroutine(DestroyAfter());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
