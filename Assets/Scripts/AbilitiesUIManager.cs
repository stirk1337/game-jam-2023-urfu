using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public List<GameObject> buttons;
    [SerializeField] Player player;
    [SerializeField] Sprite defaultCube;
    [SerializeField] Sprite electroCube;
    [SerializeField] Sprite fireCube;
    [SerializeField] Sprite windCube;
    [SerializeField] public Image currentCubeSprite;
    Ability[] abilities;

    public void TurnInteractable(GameObject current)
    {
        Button currentButton = current.GetComponent<Button>();
        foreach(GameObject gameObj in buttons)
        {
            Button button = gameObj.GetComponent<Button>();
            if (button != currentButton)
                button.interactable = !button.interactable;

        }
    }


    void Start()
    {
        abilities = FindObjectsOfType<Ability>();

    }

    void UpdateVisualCooldowns()
    {
        foreach (Ability ability in abilities)
        {
            if (ability.abilityState == Ability.AbilityState.Cooldown)
            {
                ability.cooldownText.text = (ability.currentCooldown - player.currentTurn).ToString();
                ability.cooldownText.gameObject.SetActive(true);
                ability.buttonCanvas.SetActive(false);
                ability.cooldownImage.SetActive(true);
            }
            else
            {
                ability.cooldownText.text = "";
                ability.cooldownText.gameObject.SetActive(false);
                ability.buttonCanvas.SetActive(true);
                ability.cooldownImage.SetActive(false);
            }
        }
    }

    void UpdateVisualButtonInterctable()
    {

        if (player.state[Enemy.ElementState.Electro] > 0)
        {
            Button button1 = buttons[0].GetComponent<Button>();
            Button button2 = buttons[1].GetComponent<Button>();
            Button button3 = buttons[2].GetComponent<Button>();
            Button button4 = buttons[3].GetComponent<Button>();
            button1.interactable = false;
            button2.interactable = false;
            button3.interactable = false;
            button4.interactable = false;
        }
        if (player.state[Enemy.ElementState.Electro] == 0)
        {
            Button button1 = buttons[0].GetComponent<Button>();
            Button button2 = buttons[1].GetComponent<Button>();
            Button button3 = buttons[2].GetComponent<Button>();
            Button button4 = buttons[3].GetComponent<Button>();
            button1.interactable = true;
            button2.interactable = true;
            button3.interactable = true;
            button4.interactable = true;
            player.state[Enemy.ElementState.Electro] = -1;
        }
    }

    void UpdateVisualRuneColor() 
    {
        foreach (Ability ability in abilities)
        {
            Button button = ability.buttonCanvas.GetComponent<Button>();
            switch (player.abilityElement)
            {
                case Player.AbilityElement.Default:
                    button.GetComponent<Image>().sprite = ability.DefaultImage;
                    ability.cooldownImage.GetComponent<Image>().sprite = ability.DefaultImage;
                    break;
                case Player.AbilityElement.Electro:
                    button.GetComponent<Image>().sprite = ability.ElectroImage;
                    ability.cooldownImage.GetComponent<Image>().sprite = ability.ElectroImage;
                    break;
                case Player.AbilityElement.Fire:
                    button.GetComponent<Image>().sprite = ability.FireImage;
                    ability.cooldownImage.GetComponent<Image>().sprite = ability.FireImage;
                    break;
                case Player.AbilityElement.Wind:
                    button.GetComponent<Image>().sprite = ability.WindImage;
                    ability.cooldownImage.GetComponent<Image>().sprite = ability.WindImage;
                    break;
            }
        }
    }

    void UpdateVisualCubeAppearence()
    {
        switch (player.diceElement)
        {
            case Player.AbilityElement.Default:
                currentCubeSprite.sprite = defaultCube;
                break;
            case Player.AbilityElement.Fire:
                currentCubeSprite.sprite = fireCube;
                break;
            case Player.AbilityElement.Electro:
                currentCubeSprite.sprite = electroCube;
                break;
            case Player.AbilityElement.Wind:
                currentCubeSprite.sprite = windCube;
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        UpdateVisualButtonInterctable();
        UpdateVisualCooldowns();
        UpdateVisualRuneColor();
        UpdateVisualCubeAppearence();
       
    }
}
