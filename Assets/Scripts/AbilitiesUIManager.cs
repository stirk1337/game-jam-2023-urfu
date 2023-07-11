using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public List<GameObject> buttons;
    [SerializeField] Player player;

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
        
    }

    // Update is called once per frame
    void Update()
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
}
