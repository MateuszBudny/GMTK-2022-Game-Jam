using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceFaceTriggerCollider : MonoBehaviour
{
	public event Action OnDiceStoppedOnThisFace;

	private void OnTriggerEnter(Collider other)
	{
		
	}
}
