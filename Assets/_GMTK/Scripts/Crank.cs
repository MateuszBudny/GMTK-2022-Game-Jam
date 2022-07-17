using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : MonoBehaviour, IInteractable
{
    public void Interact(Player player)
    {
        GameplayManager.Instance.OpenHullDoor();
    }
}
