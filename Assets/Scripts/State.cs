using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    public static State Instance;
    [SerializeField] public bool IsPlayerTurn;
    [SerializeField] public bool FreeMove;

    private void Awake()
    {
        if (Instance == null)
        {
            IsPlayerTurn = true;
            //Random.seed = System.DateTime.Now.Millisecond;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}