using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceFaceTriggerCollider : MonoBehaviour
{
	public event Action OnDiceStoppedOnThisCollider;
    public bool IsRolling { get; set; } = false;

    private float confirmDiceValueAfterThoseSecondsOfBeingStill;
    private float groundTouchedTimestamp = -1f;

    private void OnTriggerEnter(Collider other)
    {
        if(ShouldCheckRolling(other))
        {
            groundTouchedTimestamp = Time.time;
        }
    }

    private void OnTriggerStay(Collider other)
	{
		if(ShouldCheckRolling(other))
        {
            if(groundTouchedTimestamp > 0 && Time.time - groundTouchedTimestamp > confirmDiceValueAfterThoseSecondsOfBeingStill)
            {
                OnDiceStoppedOnThisCollider();
                IsRolling = false;
                groundTouchedTimestamp = -1f;
            }
        }
	}

    private void OnTriggerExit(Collider other)
    {
        if(ShouldCheckRolling(other))
        {
            groundTouchedTimestamp = -1f;
        }
    }

    public void StartRolling(float confirmDiceValueAfterThoseSecondsOfBeingStill)
    {
        IsRolling = true;
        groundTouchedTimestamp = -1f;
        this.confirmDiceValueAfterThoseSecondsOfBeingStill = confirmDiceValueAfterThoseSecondsOfBeingStill;
    }

    private bool ShouldCheckRolling(Collider collidedWith) => IsRolling && collidedWith.CompareTag(Tags.Ground.ToString());
}
