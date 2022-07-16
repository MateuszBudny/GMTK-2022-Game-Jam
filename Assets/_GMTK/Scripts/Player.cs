using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField]
    private DicesGroup dicesGroup;
    [SerializeField]
    private float throwForceScaleMin = 0.1f;
    [SerializeField]
    private float throwForceScaleMax = 2f;
    [SerializeField]
    private Transform throwRangeMin;
    [SerializeField]
    private Transform throwRangeMax;
    [SerializeField]
    private Vector3 throwTorqueMin;
    [SerializeField]
    private Vector3 throwTorqueMax;
    [SerializeField]
    private Transform showDicesPosition;
    [SerializeField]
    private Transform throwDicesPosition;

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void ThrowDices()
    {
        dicesGroup.Roll(throwRangeMin.forward * throwForceScaleMin, throwRangeMax.forward * throwForceScaleMax, throwTorqueMin, throwTorqueMax);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void TakeDices()
    {
        dicesGroup.Hold();
        //dicesGroup.transform.DOMove(showDicesPosition.position, 2f);
    }
}