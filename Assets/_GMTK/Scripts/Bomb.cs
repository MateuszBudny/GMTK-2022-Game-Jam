using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Vector3 worldPositionArmed;

    void Start()
    {
        worldPositionArmed = transform.position;
    }
    
    public void ResetPos()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        transform.position = worldPositionArmed;
    }
}
