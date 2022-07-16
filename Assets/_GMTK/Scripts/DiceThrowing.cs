using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceThrowing : MonoBehaviour
{
    [SerializeField]
    private DicesGroup dicesGroup;
    [SerializeField]
    private float throwForceScaleMin = 0.2f;
    [SerializeField]
    private float throwForceScaleMax = 0.5f;
    [SerializeField]
    private Transform throwRangeMin;
    [SerializeField]
    private Transform throwRangeMax;
    [SerializeField]
    private Vector3 throwTorqueMin = new Vector3(5f, 5f, 5f);
    [SerializeField]
    private Vector3 throwTorqueMax = new Vector3(20f, 20f, 20f);
    [SerializeField]
    private Transform showDicesPosition;
    [SerializeField]
    private Transform throwDicesPosition;

    public int CurrentScore => dicesGroup.CurrentValue;

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void ThrowDices()
    {
        dicesGroup.transform.DOMove(throwDicesPosition.position, 0.2f).OnComplete(() =>
        {
            dicesGroup.Roll(throwRangeMin.forward * throwForceScaleMin, throwRangeMax.forward * throwForceScaleMax, throwTorqueMin, throwTorqueMax);
        });
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void TakeDices()
    {
        dicesGroup.Hold();
        dicesGroup.transform.DOMove(showDicesPosition.position, 0.1f);
    }
}
