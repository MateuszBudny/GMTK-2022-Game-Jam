using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float backForceWhenFalling;
    [SerializeField]
    private Vector3 minRandomRotationForceOnFall;
    [SerializeField]
    private Vector3 maxRandomRotationForceOnFall;
    [SerializeField]
    private float minTipDownRotationForceOnFall;
    [SerializeField]
    private float maxTipDownRotationForceOnFall;
    [SerializeField]
    private float airResistanceOneTimeForce = 10f;

    private Rigidbody rigid;
    private Vector3 worldPositionArmed;
    private Quaternion worldRotationArmed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        worldPositionArmed = transform.position;
        worldRotationArmed = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.AirResistanceTrigger.ToString()))
        {
            ApplyAirResistanceOneTimeForce();
        }
    }

    public void ResetPos()
    {
        rigid.isKinematic = true;
        transform.position = worldPositionArmed;
        transform.rotation = worldRotationArmed;
    }

    public void Fall()
    {
        rigid.isKinematic = false;
        transform.SetParent(null);

        Vector3 randomRotationForce = minRandomRotationForceOnFall.RandomRange(maxRandomRotationForceOnFall);
        float tipDownRotationForce = Random.Range(minTipDownRotationForceOnFall, maxTipDownRotationForceOnFall);
        rigid.AddForce(new Vector3(0f, 0f, backForceWhenFalling), ForceMode.Force);
        rigid.AddTorque(randomRotationForce, ForceMode.Force);
        rigid.AddTorque(-new Vector3(tipDownRotationForce, 0f, 0f), ForceMode.Force);

        SoundManager.Instance.Play(Audio.BombsFalling); // play for every bomb or just once for them all?
    }

    private void ApplyAirResistanceOneTimeForce()
    {
        Debug.Log("apply force");
        rigid.AddForce(new Vector3(0f, 0f, airResistanceOneTimeForce), ForceMode.VelocityChange);
    }
}
