using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satan : DicePlayer, IShootable, IAimable
{
    [Header("Satan monologues")]
    [SerializeField]
    private SatanThematicMonologuesData reaccToPlayerMissingCrateWithDiceMonologues;
    [SerializeField]
    private SatanThematicMonologuesData playerShootingAtSatanMonologues;
    [SerializeField]
    private SatanThematicMonologuesData playerAimingAtSatanMonologues;

    public SatanFaceType CurrentFace => satanFaces.CurrentFace;

    private SatanFaces satanFaces;

    protected override void Awake()
    {
        base.Awake();
        satanFaces = GetComponent<SatanFaces>();
    }

    public void SetFace(SatanFaceType newFaceType) => satanFaces.SetFace(newFaceType);

    public override void ThrowDices()
    {
        Debug.Log("Your score: " + GameplayManager.Instance.player.CurrentScore);
        base.ThrowDices();

        GameplayManager.Instance.SendSignalToGameplayManager(GameplayManager.Instance.satanThrewDicesSignal);
    }

    public void IsAimedAt(Gun gunAiming)
    {
        if (!StoryManager.Instance.IsDuringDialogue && !playerAimingAtSatanMonologues.HasUsedAllLinesOnce)
        {
            StoryManager.Instance.PlayNextMonologue(playerAimingAtSatanMonologues);
        }
    }

    public void GetShot(Gun gunShooting)
    {
        StoryManager.Instance.PlayNextMonologue(playerShootingAtSatanMonologues);
    }
}
