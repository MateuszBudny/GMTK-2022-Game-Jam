using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DicesGroup : MonoBehaviour
{
    [SerializeField]
    private float dicesSoundCooldown = 2f;
    [SerializeField]
    private List<DiceRecord> dicesRecords;

    public (int sumValue, int dicesNum) CurrentValue => (dicesRecords.Sum(record => record.dice.CurrentValue), dicesRecords.Where(dice => dice.dice.CurrentValue > 0).Count());
    public int AllDicesNum => dicesRecords.Count;

    private float dicesSoundTimestamp = -1f;

    private void Start()
    {
        Release();
    }

    private void OnEnable()
    {
        dicesRecords.ForEach(record => record.dice.OnPlayDiceSound += PlayDicesSound);
    }

    private void OnDisable()
    {
        dicesRecords.ForEach(record => record.dice.OnPlayDiceSound -= PlayDicesSound);
    }

    public void Roll(Vector3 throwForceMin, Vector3 throwForceMax, Vector3 throwTorqueMin, Vector3 throwTorqueMax)
    {
        Release();
        dicesRecords.ForEach(record => record.dice.Roll(throwForceMin, throwForceMax, throwTorqueMin, throwTorqueMax));
    }

    public void Hold()
    {
        dicesRecords.ForEach(record =>
        {
            record.dice.TogglePhysicsAffection(false);
            record.dice.transform.SetParent(transform);
        });
        Regroup();
    }

    public void Release()
    {
        dicesRecords.ForEach(record =>
        {
            record.dice.TogglePhysicsAffection(true);
            record.dice.transform.SetParent(null);
        });
    }

    private void Regroup()
    {
        dicesRecords.ForEach(record => record.dice.transform.DOLocalMove(record.diceBaseLocalPosition, 0.3f));
    }

    private void PlayDicesSound()
    {
        if(Time.time - dicesSoundTimestamp > dicesSoundCooldown)
        {
            SoundManager.Instance.Play(Audio.DiceBounce);
            dicesSoundTimestamp = Time.time;
        }
    }

    [Serializable]
    private class DiceRecord
    {
        public Dice dice;
        public Vector3 diceBaseLocalPosition;
    }
}