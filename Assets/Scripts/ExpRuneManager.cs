using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpRuneManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject expRunePrefab;



    public void Spawn(Transform target)
    {
        Instantiate(expRunePrefab, target.position, transform.rotation);
    }



}
