using DG.Tweening;
using NodeCanvas.Framework;
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
    [SerializeField]
    private int maxAngerLevel = 10;
    [SerializeField]
    private SignalDefinition satanGotAngrySignal;

    public SatanFaceType CurrentFace => satanFaces.CurrentFace;

    public int AngerLevel
    {
        get => angerLevel;
        private set
        {
            angerLevel = value;
            if(AngerLevel >= maxAngerLevel)
            {
                GameplayManager.Instance.SendSignalToGameplayManager(satanGotAngrySignal);
            }
        }
    }

    private SatanFaces satanFaces;
    private int angerLevel;

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

    public void IncreaseAnger()
    {
        AngerLevel++;
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
