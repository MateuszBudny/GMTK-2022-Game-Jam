using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : MonoBehaviour, IInteractable
{
    public void Interact(Player player)
    {
        if(GameplayManager.Instance.State != GameState.BombsAreFalling)
        {
            GameplayManager.Instance.OpenHullDoor();
        }
    }
}
