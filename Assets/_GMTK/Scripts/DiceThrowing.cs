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
    [SerializeField]
    private float moveFromDicesShowToThrowPosition = 0.2f;
    [SerializeField]
    private DicesGroup dicesGroupToSpawnPrefab;

    public (int sumValue, int dicesNum) CurrentScore => dicesGroup.CurrentValue;
    public int AllDicesNum => dicesGroup.AllDicesNum;

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void ThrowDices()
    {
        dicesGroup.transform.DOMove(throwDicesPosition.position, moveFromDicesShowToThrowPosition).OnComplete(() =>
        {
            dicesGroup.Roll(throwRangeMin.forward * throwForceScaleMin, throwRangeMax.forward * throwForceScaleMax, throwTorqueMin, throwTorqueMax);
        });
    }

    public void SpawnAndThrowDices()
    {
        DicesGroup spawned = Instantiate(dicesGroupToSpawnPrefab, throwDicesPosition.position, Quaternion.identity);
        spawned.Roll(throwRangeMin.forward * throwForceScaleMin, throwRangeMax.forward * throwForceScaleMax, throwTorqueMin, throwTorqueMax);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void TakeDices()
    {
        dicesGroup.Hold();
        dicesGroup.transform.DOMove(showDicesPosition.position, 0.1f);
    }
}
