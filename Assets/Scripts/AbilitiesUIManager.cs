using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<GameObject> buttons;

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
        
    }
}
