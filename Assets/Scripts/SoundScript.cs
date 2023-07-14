using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Sprite _soundOn;
    [SerializeField] Sprite _soundOff;

    public void TurnSound(bool turn)
    {
        AudioListener.pause = !turn;
        State.Instance.Sound = turn;
        if (turn)
        {
            gameObject.GetComponent<Image>().sprite = _soundOn;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = _soundOff;
        }
    }

    public void TurnerOfSoundInGame()
    {
        TurnSound(AudioListener.pause);
    }


    void Start()
    {
        TurnSound(State.Instance.Sound);
    }

    // Update is called once per frame
    void Update()
    {

    }
}