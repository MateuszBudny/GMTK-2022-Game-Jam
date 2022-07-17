using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTest : MonoBehaviour, IInteractable
{
    public void Interact(Player player)
    {
        Debug.LogWarning("interaction!");
    }
}
