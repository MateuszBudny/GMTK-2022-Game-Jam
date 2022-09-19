using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Rigidbody rigid;
    private Vector3 worldPositionArmed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        worldPositionArmed = transform.position;
    }
    
    public void ResetPos()
    {
        rigid.isKinematic = true;
        transform.position = worldPositionArmed;
    }

    public void Fall()
    {
        rigid.isKinematic = false;
        transform.SetParent(null);
        SoundManager.Instance.Play(Audio.BombsFalling); // play for every bomb or just once for them all?
    }
}
