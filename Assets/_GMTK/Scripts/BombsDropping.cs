using AetherEvents;
using DG.Tweening;
using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombsDropping : MonoBehaviour
{
    [SerializeField]
    private Transform crankToRoll;
    [SerializeField]
    private Transform hulkDoorLeft;
    [SerializeField]
    private Transform hulkDoorRight;
    [SerializeField]
    private List<Bomb> bombs;

    [Header("Signals")]
    [SerializeField]
    private SignalDefinition hullOpeningForBombsDroppingStartedSignal;
    [SerializeField]
    private SignalDefinition bombsDroppedSignal;
    [SerializeField]
    private SignalDefinition hullDoorsClosedSignal;
    [SerializeField]
    private SignalDefinition bombsResetSignal;

    public void OpenHullDoorAndDropBombs()
    {
        StartCoroutine(DropBombsSequence());
    }

    private IEnumerator DropBombsSequence()
    {
        OpenHullDoors();
        yield return new WaitForSeconds(2f);

        DropBombs();
        yield return new WaitForSeconds(4f);

        GameplayManager.Instance.SendSignalToGameplayManager(bombsDroppedSignal);
        CloseHullDoor();

        yield return new WaitForSeconds(5f);
        ResetBombsPositions();
    }

    private void OpenHullDoors()
    {
        GameplayManager.Instance.SendSignalToGameplayManager(hullOpeningForBombsDroppingStartedSignal);
        new HullOpeningForBombsDroppingStarted(GameplayManager.Instance.player.CurrentBombingsDone + 1, GameplayManager.Instance.player.droppingBombsNumToGoIntoMadness).Invoke();
        SoundManager.Instance.Play(Audio.CrankHullOpening);
        hulkDoorLeft.DOLocalRotate(new Vector3(0f, 0f, -120f), 4f).SetRelative(true);
        hulkDoorRight.DOLocalRotate(new Vector3(0f, 0f, 120f), 4f).SetRelative(true);
        crankToRoll.DOLocalRotate(new Vector3(-3600f, 0f, 0f), 4f, RotateMode.FastBeyond360).SetRelative(true);
        Debug.Log("Hull opened, bombs are falling");
    }

    private void CloseHullDoor()
    {
        hulkDoorLeft.DOLocalRotate(new Vector3(0f, 0f, 120f), 4f).SetRelative(true);
        hulkDoorRight.DOLocalRotate(new Vector3(0f, 0f, -120f), 4f).SetRelative(true)
            .OnComplete(() => GameplayManager.Instance.SendSignalToGameplayManager(hullDoorsClosedSignal));
        crankToRoll.DOLocalRotate(new Vector3(3600f, 0f, 0f), 4f, RotateMode.FastBeyond360).SetRelative(true);
    }

    private void DropBombs()
    {
        bombs.ForEach(bomb => bomb.Fall());
    }

    private void ResetBombsPositions()
    {
        bombs.ForEach(bomb => bomb.ResetPos());
        GameplayManager.Instance.SendSignalToGameplayManager(bombsResetSignal);
    }
}
