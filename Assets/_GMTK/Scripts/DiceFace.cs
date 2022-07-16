using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceFace : MonoBehaviour
{
	[SerializeField]
	private DiceFaceTriggerCollider triggerCollider;
    [SerializeField]
    private int faceValue;

    public event Action<int> OnDiceStoppedOnThisFace;

    private void OnEnable()
    {
		triggerCollider.OnDiceStoppedOnThisCollider += OnDiceStoppedOnThisFaceCollider;
    }

    private void OnDisable()
    {
        triggerCollider.OnDiceStoppedOnThisCollider -= OnDiceStoppedOnThisFaceCollider;
    }

    public void StartRolling(float confirmDiceValueAfterThoseSecondsOfBeingStill)
    {
        triggerCollider.StartRolling(confirmDiceValueAfterThoseSecondsOfBeingStill);
    }

    private void OnDiceStoppedOnThisFaceCollider()
	{
        OnDiceStoppedOnThisFace(faceValue);
	}
}
