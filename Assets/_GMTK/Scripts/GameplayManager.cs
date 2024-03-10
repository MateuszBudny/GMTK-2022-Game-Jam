using Cinemachine;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using AetherEvents;
using UnityEngine.Events;
using NodeCanvas.StateMachines;
using NodeCanvas.Framework;
using NodeCanvas.BehaviourTrees;

public class GameplayManager : SingleBehaviour<GameplayManager>
{
    [SerializeField]
    private FSMOwner fsmOwner;
    public Player player;
    public Satan satan;
    [Header("Camera work")]
    public CinemachineVirtualCamera vcamFollowMouse;
    [SerializeField]
    private CinemachineVirtualCamera vcamCrank;

    [Header("States")]
    public BehaviourTree closedEyesState;
    public BehaviourTree satanStartingMonologueState;
    public BehaviourTree playerCanInteractState;
    public BehaviourTree playerTurnState;
    public BehaviourTree satanTurnState;
    public BehaviourTree playerCanInteractNoDiceMustDropBombsState;
    public BehaviourTree playerIsDroppingBombsState;
    public BehaviourTree playerBecomesSatanState;
    public BehaviourTree satanIsWreckingHavoc;
    public BehaviourTree gameOverState;

    [Header("Signals")]
    public SignalDefinition eyesStartingToOpenSignal;
    public SignalDefinition playerTookDicesSignal;
    public SignalDefinition playerThrewDicesSignal;
    public SignalDefinition satanThrewDicesSignal;
    public SignalDefinition playerKilledThemselfSignal;

    [Header("Events")]
    [SerializeField]
    private UnityEvent<CinemachineVirtualCamera> OnVCamChanged;

    public FSMState CurrentState => (fsmOwner.GetCurrentState() as FSMState);
    public CinemachineVirtualCamera CurrentVCam
    {
        get => currentVCam;
        set
        {
            if(CurrentVCam)
            {
                value.Priority = CurrentVCam.Priority + 1;
            }
            currentVCam = value;
            OnVCamChanged.Invoke(CurrentVCam);
        }
    }

    private CinemachineVirtualCamera currentVCam;

    protected override void Awake()
    {
        base.Awake();
        fsmOwner = GetComponent<FSMOwner>();
    }

    public bool IsGameInState(BehaviourTree btState) => CurrentState != null ? // is it possible to get rid of this null check?
        (CurrentState as NestedBTState).subGraph.name == btState.name :
        false;

    public void SendSignalToGameplayManager(SignalDefinition signal, Transform sender = null, params object[] args)
    {
        signal.Invoke(sender, transform, false, args);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void PlayTheGame()
    {
        Debug.Log("New game started.");
        satan.PrepareForGame();
    }

    public void LookAtCrank()
    {
        CurrentVCam = vcamCrank;
        StartCoroutine(ReleaseTheCameraAfterDelay());
    }

    public void Suicide()
    {
        player.Die();
        SoundManager.Instance.StopAllMusicAndSounds();
        SoundManager.Instance.Play(Audio.Gunshot);
        SendSignalToGameplayManager(playerKilledThemselfSignal);
    }

    public ResultOfDices CheckTheGameResult()
    {
        if (player.CurrentScore.sumValue > satan.CurrentScore.sumValue)
        {
            Debug.Log("The weather is so bad, that the bombing was canceled.");
            return ResultOfDices.PlayerWon;
        }
        else if (player.CurrentScore.sumValue < satan.CurrentScore.sumValue)
        {
            Debug.Log("Aaand there goes the bombs.");
            return ResultOfDices.SatanWon;
        }
        else
        {
            Debug.Log("One more time, then.");
            return ResultOfDices.Draw;
        }
    }

    private IEnumerator ReleaseTheCameraAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        CurrentVCam = vcamFollowMouse;
    }
}

public enum ResultOfDices
{
    PlayerWon,
    SatanWon,
    Draw,
}
