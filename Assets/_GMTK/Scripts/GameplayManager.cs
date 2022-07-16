using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SingleBehaviour<GameplayManager>
{
    public Player player;
    public Satan satan;

    public GameState State { get; private set; }

    public event Action OnPlayerThrewDices;
    public event Action OnSatanThrewDices;

    private void Start()
    {
        StartCoroutine(PlayTheGameWithDelay());
    }

    private void OnEnable()
    {
        OnSatanThrewDices += ShowResultAfterSomeSeconds;
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void PlayTheGame()
    {
        Debug.Log("New game started.");
        player.PrepareForGame();
        satan.PrepareForGame();
        ChangeState(GameState.PlayerTurn);
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log("Previous state: " + State + ", new state: " + newState);
        State = newState;
    }

    public void PlayerThrewDices()
    {
        ChangeState(GameState.PlayerThrewDices);
        OnPlayerThrewDices();
    }

    public void SatanThrewDices()
    {
        ChangeState(GameState.SatanThrewDices);
        OnSatanThrewDices();
    }

    private void ShowResultAfterSomeSeconds()
    {
        StartCoroutine(ShowResultAfterSomeSecondsEnumerator());
    }

    private IEnumerator ShowResultAfterSomeSecondsEnumerator()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Satan score: " + satan.CurrentScore);

        if(player.CurrentScore > satan.CurrentScore)
        {
            Debug.Log("The weather is so bad, that the bombing was canceled.");
        }
        else if(satan.CurrentScore > player.CurrentScore)
        {
            Debug.Log("Aaand there goes the bombs.");
        }
        else
        {
            Debug.Log("One more time, then.");
        }

        PlayTheGame();
    }
    private IEnumerator PlayTheGameWithDelay()
    {
        yield return new WaitForSeconds(1f);
        PlayTheGame();
    }
}

public enum GameState
{
    Intro,
    SatanMonolog,
    PlayerTurn,
    PlayerThrewDices,
    SatanTurn,
    SatanThrewDices,
    ResultOfSingleGame,
}
