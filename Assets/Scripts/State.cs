using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    public static State Instance;
    [SerializeField] public bool IsPlayerTurn;

    private void Awake()
    {
        if (Instance == null)
        {
            IsPlayerTurn = true;
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