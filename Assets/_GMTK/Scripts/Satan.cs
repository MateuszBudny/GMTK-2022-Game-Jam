using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satan : DicePlayer, IShootable
{
    private SatanFaces satanFaces;

    public SatanFaceType CurrentFace => satanFaces.CurrentFace;

    protected override void Awake()
    {
        base.Awake();
        satanFaces = GetComponent<SatanFaces>();
    }

    private void Start()
    {
        GameplayManager.Instance.OnPlayerThrewDices += OnPlayerThrewDices;
    }

    private void OnDisable()
    {
        GameplayManager.Instance.OnPlayerThrewDices -= OnPlayerThrewDices;
    }

    public void SetFace(SatanFaceType newFaceType) => satanFaces.SetFace(newFaceType);

    private void OnPlayerThrewDices()
    {
        StartCoroutine(AfterPlayerThrewDicesEnumerator());
    }

    private IEnumerator AfterPlayerThrewDicesEnumerator()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("Your score: " + GameplayManager.Instance.player.CurrentScore);
        if(GameplayManager.Instance.player.CurrentScore.dicesNum < GameplayManager.Instance.player.AllDicesNum)
        {
            StoryManager.Instance.ShowNextSatanReaccToPlayerDiceNotFallingOnTheCrate();
            yield return new WaitForSeconds(3f);
        }

        GameplayManager.Instance.ChangeState(GameState.SatanTurn);
        diceThrowing.ThrowDices();
        GameplayManager.Instance.SatanThrewDices();
    }

    public void GetShot(Gun gunShooting)
    {
        StoryManager.Instance.ShowNextPlayerShootingAtSatanLines();
    }
}
