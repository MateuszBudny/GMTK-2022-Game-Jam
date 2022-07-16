using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satan : DicePlayer
{
    private void Start()
    {
        GameplayManager.Instance.OnPlayerThrewDices += OnPlayerThrewDices;
    }

    private void OnDisable()
    {
        GameplayManager.Instance.OnPlayerThrewDices -= OnPlayerThrewDices;
    }

    private void OnPlayerThrewDices()
    {
        StartCoroutine(AfterPlayerThrewDicesEnumerator());
    }

    private IEnumerator AfterPlayerThrewDicesEnumerator()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Your score: " + GameplayManager.Instance.player.CurrentScore);
        GameplayManager.Instance.ChangeState(GameState.SatanTurn);
        diceThrowing.ThrowDices();
        GameplayManager.Instance.SatanThrewDices();
    }
}
