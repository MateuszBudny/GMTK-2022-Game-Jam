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
    public PostProcessController postProcessController;

    [Header("UI")]
    [SerializeField]
    private BlackScreen blackScreen;
    [SerializeField]
    private float onStartFadeOutDuration = 4f;
    [SerializeField]
    private GameObject openEyesText;
    [SerializeField]
    private TextMeshProUGUI thankYou;

    [Header("Satan Monologues")]
    [SerializeField]
    private SatanThematicMonologuesData introMonologues;
    [SerializeField]
    private SatanThematicMonologuesData burzaEndingMonologues;
    [SerializeField]
    private SatanThematicMonologuesData suggestionsMonologues;
    [SerializeField]
    private SatanThematicMonologuesData satanDiceWinMonologues;
    [SerializeField]
    private SatanThematicMonologuesData satanDiceLoseMonologues;
    [SerializeField]
    private SatanThematicMonologuesData drawMonologues;


    [Header("Other")]
    [SerializeField]
    private Transform crankToRoll;
    [SerializeField]
    private Transform hulkDoorLeft;
    [SerializeField]
    private Transform hulkDoorRight;
    [SerializeField]
    private List<Bomb> bombs;

    [Header("Events")]
    [SerializeField]
    public UnityEvent<CinemachineVirtualCamera> OnVCamChanged;

    public GameState State { get; private set; }
    public CinemachineVirtualCamera CurrentVCam
    {
        get => currentVCam;
        private set
        {
            currentVCam = value;
            OnVCamChanged.Invoke(CurrentVCam);
        }
    }

    public event Action OnPlayerThrewDices;
    public event Action OnSatanThrewDices;

    private CinemachineVirtualCamera currentVCam;

    private void Start()
    {
        State = GameState.ClosedEyes;
    }

    private void OnEnable()
    {
        OnSatanThrewDices += ShowResultAfterSomeSeconds;
    }

    public void OpenEyes()
    {
        openEyesText.SetActive(false);
        blackScreen.FadeOut(onStartFadeOutDuration, true);
        StartCoroutine(PlayTheGameWithDelay());
        CurrentVCam = vcamFollowMouse;
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

    internal void Suicide()
    {
        player.Die();
        SoundManager.Instance.StopAllMusicAndSounds();
        SoundManager.Instance.Play(Audio.Gunshot);
        blackScreen.FadeIn(0f);
        StartCoroutine(ThankYouAfterDelay());
    }

    private IEnumerator ThankYouAfterDelay()
    {
        ChangeState(GameState.GameOver);
        yield return new WaitForSeconds(3f);

        thankYou.gameObject.SetActive(true);
    }

    public void BurzaEnding()
    {
        blackScreen.FadeIn(6f, true, () =>
        {
            satan.gameObject.SetActive(false);
            player.BecomeSatan();
            ChangeState(GameState.PlayerAsSatan);
        });
        StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
        yield return new WaitForSeconds(8f);

        blackScreen.FadeOut(6f);
        postProcessController.SetVignetteSmoothly(0.6f, 6f);
        yield return new WaitForSeconds(10f);

        blackScreen.FadeIn(10f, false, () => StartCoroutine(ThankYouAfterDelay()));
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
        new HullOpeningForBombsDroppingStarted(player.CurrentBombingsDone + 1, player.droppingBombsNumToGoIntoMadness).Invoke();
        SoundManager.Instance.Play(Audio.CrankHullOpening);
        hulkDoorLeft.DOLocalRotate(new Vector3(0f, 0f, -120f), 4f).SetRelative(true);
        hulkDoorRight.DOLocalRotate(new Vector3(0f, 0f, 120f), 4f).SetRelative(true);
        crankToRoll.DOLocalRotate(new Vector3(-3600f, 0f, 0f), 4f, RotateMode.FastBeyond360).SetRelative(true);
        Debug.Log("Hull opened, bombs are falling");
        ChangeState(GameState.BombsAreFalling);

        if (player.IsGonnaSnap)
        {
            StoryManager.Instance.PlayNextMonologue(burzaEndingMonologues);
        }

        StartCoroutine(DropBombsAfterDelay());
    }

    public void CloseHullDoor()
    {
        hulkDoorLeft.DOLocalRotate(new Vector3(0f, 0f, 120f), 4f).SetRelative(true);
        hulkDoorRight.DOLocalRotate(new Vector3(0f, 0f, -120f), 4f).SetRelative(true);
        crankToRoll.DOLocalRotate(new Vector3(3600f, 0f, 0f), 4f, RotateMode.FastBeyond360).SetRelative(true);
    }

    private IEnumerator DropBombsAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        bombs.ForEach(bomb => bomb.Fall());

        StartCoroutine(CloseHullDoorAfterBombing());
        StartCoroutine(PlayTheGameAfterBombsFallen(player.IsGonnaSnap));

        player.CurrentBombingsDone++;

        new BombsDropped(player.CurrentBombingsDone, player.droppingBombsNumToGoIntoMadness).Invoke();
    }

    private IEnumerator CloseHullDoorAfterBombing()
    {
        yield return new WaitForSeconds(4f);
        CloseHullDoor();
    }

    private void ShowResultAfterSomeSeconds()
    {
        StartCoroutine(ShowResultAfterSomeSecondsEnumerator());
    }

    private IEnumerator ShowResultAfterSomeSecondsEnumerator()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Satan score: " + satan.CurrentScore);

        if (player.CurrentScore.sumValue > satan.CurrentScore.sumValue)
        {
            StoryManager.Instance.PlayNextMonologue(satanDiceLoseMonologues);
            Debug.Log("The weather is so bad, that the bombing was canceled.");
            PlayTheGame();
        }
        else if (satan.CurrentScore.sumValue > player.CurrentScore.sumValue)
        {
            StoryManager.Instance.PlayNextMonologue(satanDiceWinMonologues);
            Debug.Log("Aaand there goes the bombs.");
            ChangeState(GameState.PlayerCanInteractNoDice);
            LookAtCrank();
        }
        else
        {
            StoryManager.Instance.PlayNextMonologue(drawMonologues);
            Debug.Log("One more time, then.");
            PlayTheGame();
        }
    }

    private void LookAtCrank()
    {
        vcamCrank.Priority = CurrentVCam.Priority + 1;
        CurrentVCam = vcamCrank;
        StartCoroutine(ReleaseTheCameraAfterDelay());
    }

    private IEnumerator PlayTheGameWithDelay()
    {
        yield return new WaitForSeconds(1f);
        StoryManager.Instance.PlayNextMonologue(introMonologues);
        PlayTheGame();
    }

    private IEnumerator PlayTheGameAfterBombsFallen(bool isPlayerGonnaSnap)
    {
        yield return new WaitForSeconds(4f);

        if (State == GameState.BombsAreFalling && !isPlayerGonnaSnap)
        {
            StoryManager.Instance.PlayNextMonologue(suggestionsMonologues);
        }
        yield return new WaitForSeconds(5f);

        if (State == GameState.BombsAreFalling)
        {
            PlayTheGame();
        }
        bombs.ForEach(bomb => bomb.ResetPos());
    }

    private IEnumerator ReleaseTheCameraAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        vcamFollowMouse.Priority = CurrentVCam.Priority + 1;
        CurrentVCam = vcamFollowMouse;
    }
}

public enum GameState
{
    ClosedEyes,
    SatanMonolog,
    PlayerCanInteractNoDice,
    PlayerCanInteract,
    PlayerTurn,
    PlayerThrewDices,
    SatanTurn,
    SatanThrewDices,
    ResultOfSingleGame,
    BombsAreFalling,
    PlayerAsSatan,
    GameOver,
}
