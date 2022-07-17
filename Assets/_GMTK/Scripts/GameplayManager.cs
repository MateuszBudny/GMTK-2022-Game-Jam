using Cinemachine;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField]
    private Transform crankToRoll;
    [SerializeField]
    private Transform hulkDoorLeft;
    [SerializeField]
    private Transform hulkDoorRight;
    [SerializeField]
    private Image blackScreen;
    [SerializeField]
    private TextMeshProUGUI thankYou;
    [SerializeField]
    private List<Bomb> bombs;

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

    internal void Suicide()
    {
        SoundManager.Instance.StopAllMusic();
        blackScreen.gameObject.SetActive(true);
        StartCoroutine(ThankYouAfterDelay());
    }

    private IEnumerator ThankYouAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        ChangeState(GameState.GameOver);
        thankYou.gameObject.SetActive(true);
    }

    public void BurzaEnding()
    {
        blackScreen.color = new Color(0f, 0f, 0f, 0f);
        DOTween.To(() => blackScreen.color.a, setter => blackScreen.color = new Color(0f, 0f, 0f, setter), 1f, 6f)
            .OnComplete(() => {
                satan.gameObject.SetActive(false);
                ChangeState(GameState.PlayerAsSatan);
            });
        blackScreen.gameObject.SetActive(true);
        StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
        yield return new WaitForSeconds(8f);
        DOTween.To(() => blackScreen.color.a, setter => blackScreen.color = new Color(0f, 0f, 0f, setter), 0f, 6f);
        yield return new WaitForSeconds(10f);
        DOTween.To(() => blackScreen.color.a, setter => blackScreen.color = new Color(0f, 0f, 0f, setter), 1f, 10f)
            .OnComplete(() => StartCoroutine(ThankYouAfterDelay()));
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
        SoundManager.Instance.Play(Audio.CrankHullOpening);
        hulkDoorLeft.DOLocalRotate(new Vector3(0f, 0f, -120f), 4f).SetRelative(true);
        hulkDoorRight.DOLocalRotate(new Vector3(0f, 0f, 120f), 4f).SetRelative(true);
        crankToRoll.DOLocalRotate(new Vector3(-3600f, 0f, 0f), 4f, RotateMode.FastBeyond360).SetRelative(true);
        Debug.Log("Hull opened, bombs are falling");
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
        bombs.ForEach(bomb => {
            bomb.GetComponent<Rigidbody>().isKinematic = false;
            bomb.transform.SetParent(null);
            SoundManager.Instance.Play(Audio.BombsFalling);
        });
        StartCoroutine(PlayTheGameAfterBombsFallen());
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
        yield return new WaitForSeconds(4f);
        StoryManager.Instance.ShowNextSatanSuggestion();
        yield return new WaitForSeconds(6f);
        bombs.ForEach(bomb => bomb.ResetPos());
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
    PlayerAsSatan,
    GameOver,
}
