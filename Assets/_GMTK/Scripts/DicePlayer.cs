using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePlayer : MonoBehaviour
{
    public (int sumValue, int dicesNum) CurrentScore => diceThrowing.CurrentScore;
    public int AllDicesNum => diceThrowing.AllDicesNum;
    public bool HasAnyDiceMissed => CurrentScore.dicesNum < AllDicesNum;

    protected DiceThrowing diceThrowing;

    protected virtual void Awake()
    {
        diceThrowing = GetComponent<DiceThrowing>();
    }

    public void PrepareForGame()
    {
        diceThrowing.TakeDices();
    }

    public virtual void ThrowDices()
    {
        diceThrowing.ThrowDices();
    }
}
