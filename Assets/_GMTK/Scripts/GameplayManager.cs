using Cinemachine;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SingleBehaviour<GameplayManager>
{
    public Player player;
    public Satan satan;
    [SerializeField]
    private GameObject cursor;
    [Header("Camera work")]
    [SerializeField]
    private CinemachineVirtualCamera vcamFollowMouse;
    [SerializeField]
    private CinemachineVirtualCamera vcamCrank;

    public GameState State { get; private set; }

    public event Action OnPlayerThrewDices;
    public event Action OnSatanThrewDices;

    private CinemachineVirtualCamera currentVcam;

    private void Start()
    {
        StartCoroutine(PlayTheGameWithDelay());
        currentVcam = vcamFollowMouse;
    }

    private void OnEnable()
    {
        OnSatanThrewDices += ShowResultAfterSomeSeconds;
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void PlayTheGame()
    {
        Debug.Log("New game started.");
        satan.PrepareForGame();
        ChangeState(GameState.PlayerCanInteract);
    }

    public void ChangeState(GameState newState)
    {
        Debug.Log("Previous state: " + State + ", new state: " + newState);
        cursor.SetActive(newState == GameState.PlayerCanInteract || newState == GameState.PlayerCanInteractNoDice);
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

    public void OpenHullDoor()
    {
        // TODO: animation
        // TODO: bomby spadaja
        Debug.Log("Hull opened, bombs are falling");
        StartCoroutine(PlayTheGameAfterBombsFallen());
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
            StoryManager.Instance.ShowNextSatanLoseLines();
            Debug.Log("The weather is so bad, that the bombing was canceled.");
            PlayTheGame();
        }
        else if(satan.CurrentScore > player.CurrentScore)
        {
            StoryManager.Instance.ShowNextSatanWinLines();
            Debug.Log("Aaand there goes the bombs.");
            ChangeState(GameState.PlayerCanInteractNoDice);
            LookAtCrank();
        }
        else
        {
            StoryManager.Instance.ShowNextSatanDrawLines();
            Debug.Log("One more time, then.");
            PlayTheGame();
        }
    }

    private void LookAtCrank()
    {
        vcamCrank.Priority = currentVcam.Priority + 1;
        currentVcam = vcamCrank;
        StartCoroutine(ReleaseTheCameraAfterDelay());
    }

    private IEnumerator PlayTheGameWithDelay()
    {
        yield return new WaitForSeconds(1f);
        StoryManager.Instance.ShowLines(StoryManager.Instance.introLine);
        PlayTheGame();
    }

    private IEnumerator PlayTheGameAfterBombsFallen()
    {
        yield return new WaitForSeconds(3f);
        PlayTheGame();
    }

    private IEnumerator ReleaseTheCameraAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        vcamFollowMouse.Priority = currentVcam.Priority + 1;
        currentVcam = vcamFollowMouse;
    }
}

public enum GameState
{
    Intro,
    SatanMonolog,
    PlayerCanInteractNoDice,
    PlayerCanInteract,
    PlayerTurn,
    PlayerThrewDices,
    SatanTurn,
    SatanThrewDices,
    ResultOfSingleGame,
    BombsAreFalling,
    PlayerHoldingGun,
}
