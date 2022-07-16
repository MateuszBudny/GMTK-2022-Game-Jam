using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceFace : MonoBehaviour
{
	[SerializeField]
	private DiceFaceTriggerCollider triggerCollider;

	private void Start()
	{
		triggerCollider.OnDiceStoppedOnThisFace += OnDiceStoppedOnThisFace;
	}

	private void OnDiceStoppedOnThisFace()
	{

	}
}
