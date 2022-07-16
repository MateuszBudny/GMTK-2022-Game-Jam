using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePlayer : MonoBehaviour
{
    public int CurrentScore => diceThrowing.CurrentScore;

    protected DiceThrowing diceThrowing;

    protected void Awake()
    {
        diceThrowing = GetComponent<DiceThrowing>();
    }

    public void PrepareForGame()
    {
        diceThrowing.TakeDices();
    }
}
