using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : MonoBehaviour, IInteractable
{
    [SerializeField]
    private BombsDropping bombsDropping;

    public void Interact(Player player)
    {
        if(GameplayManager.Instance.IsGameInState(GameplayManager.Instance.playerCanInteractState)
            || GameplayManager.Instance.IsGameInState(GameplayManager.Instance.playerCanInteractNoDiceMustDropBombsState))
        {
            bombsDropping.OpenHullDoorAndDropBombs();
        }
    }
}
