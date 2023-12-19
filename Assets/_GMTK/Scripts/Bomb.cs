using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IShootable
{
    [SerializeField]
    private SatanThematicMonologuesData playerShootingAtBombs;
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
    private float airResistanceForce = 1f;

    private Rigidbody rigid;
    private Vector3 worldPositionArmed;
    private Quaternion worldRotationArmed;
    private bool isAirResistancePresent = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        worldPositionArmed = transform.position;
        worldRotationArmed = transform.rotation;
    }

    private void FixedUpdate()
    {
        TryToApplyAirResistance();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.AirResistanceTrigger.ToString()))
        {
            isAirResistancePresent = true;
        }
    }

    public void ResetPos()
    {
        rigid.isKinematic = true;
        transform.position = worldPositionArmed;
        transform.rotation = worldRotationArmed;
        isAirResistancePresent = false;
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

    private void TryToApplyAirResistance()
    {
        if (!isAirResistancePresent)
            return;

        ApplyAirResistance();
    }

    private void ApplyAirResistance()
    {
        rigid.AddForce(new Vector3(0f, 0f, airResistanceForce), ForceMode.Acceleration);
    }

    public void GetShot(Gun gunShooting)
    {
        StoryManager.Instance.PlayNextMonologue(playerShootingAtBombs);
    }
}
