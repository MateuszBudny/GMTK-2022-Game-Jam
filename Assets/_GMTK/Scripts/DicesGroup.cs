using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DicesGroup : MonoBehaviour
{
    [SerializeField]
    private List<DiceRecord> dicesRecords;

    public int CurrentValue => dicesRecords.Sum(record => record.dice.CurrentValue);

    private void Start()
    {
        Release();
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

    [Serializable]
    private class DiceRecord
    {
        public Dice dice;
        public Vector3 diceBaseLocalPosition;
    }
}