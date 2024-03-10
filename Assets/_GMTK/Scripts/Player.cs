using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using StarterAssets;
using AetherEvents;
using NodeCanvas.Framework;

public class Player : DicePlayer, IShootable, IAimable
{
    [SerializeField]
    private float interactionMaxDistance = 10f;
    [SerializeField]
    private FirstPersonController playerController;
    [SerializeField]
    private SatanThematicMonologuesData playerAimingAtHimselfMonologues;

    public Transform gunHolder;
    public int droppingBombsNumToGoIntoMadness = 5;

    public int CurrentBombingsDone { get; set; } = 0;

    public bool IsPlayerHoldingGun => gun;
    public bool IsGonnaSnap => CurrentBombingsDone + 1 >= droppingBombsNumToGoIntoMadness;
    public bool IsPlayerGoingInsaneNow => CurrentBombingsDone >= droppingBombsNumToGoIntoMadness;

    private Gun gun;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        playerController.FreezeCameraRotation = true;
    }

    public void OnPlayerAction(InputValue inputValue)
    {
        if(inputValue.isPressed && !GameplayManager.Instance.IsGameInState(GameplayManager.Instance.gameOverState) && !GameplayManager.Instance.IsGameInState(GameplayManager.Instance.satanIsWreckingHavoc))
        {
            if (GameplayManager.Instance.IsGameInState(GameplayManager.Instance.closedEyesState))
            {
                GameplayManager.Instance.SendSignalToGameplayManager(GameplayManager.Instance.eyesStartingToOpenSignal);
                playerController.FreezeCameraRotation = false;
            }
            else if (IsPlayerHoldingGun)
            {
                gun.TryToShoot();
            }
            else if (GameplayManager.Instance.IsGameInState(GameplayManager.Instance.playerTurnState))
            {
                ThrowDices();
                GameplayManager.Instance.SendSignalToGameplayManager(GameplayManager.Instance.playerThrewDicesSignal);
            }
            else if (GameplayManager.Instance.IsGameInState(GameplayManager.Instance.playerCanInteractState))
            {
                if(!TryToInteract())
                {
                    PrepareForGame();
                    GameplayManager.Instance.SendSignalToGameplayManager(GameplayManager.Instance.playerTookDicesSignal);
                }
            }
            else if(GameplayManager.Instance.IsGameInState(GameplayManager.Instance.playerBecomesSatanState))
            {
                diceThrowing.SpawnAndThrowDices();
            }
            else if(!GameplayManager.Instance.IsGameInState(GameplayManager.Instance.satanStartingMonologueState) && !GameplayManager.Instance.IsGameInState(GameplayManager.Instance.satanTurnState))
            {
                TryToInteract();
            }
        }
    }

    public void OnBombsDropped()
    {
        CurrentBombingsDone++;
        new BombsDropped(CurrentBombingsDone, droppingBombsNumToGoIntoMadness).Invoke();
        LoseALittleBitOfSanity();
    }

    public void TakeGun(Gun gun)
    {
        playerController.FreezeCameraRotation = true;
        gun.Take(gunHolder, () =>
        {
            this.gun = gun;
            playerController.FreezeCameraRotation = false;
        });
    }

    public void IsAimedAt(Gun gunAiming)
    {
        if (!StoryManager.Instance.IsDuringDialogue && !playerAimingAtHimselfMonologues.HasUsedAllLinesOnce)
        {
            StoryManager.Instance.PlayNextMonologue(playerAimingAtHimselfMonologues);
        }
    }

    public void GetShot(Gun gunShooting)
    {
        if (!GameplayManager.Instance.IsGameInState(GameplayManager.Instance.playerBecomesSatanState))
        {
            GameplayManager.Instance.Suicide();
        }
    }

    public void BecomeSatan()
    {
        TryToThrowGun();
    }

    public void Die()
    {
        TryToThrowGun();
    }

    private bool TryToInteract()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        //int interactabeLayerMask = 1 << (int)Layers.Interactable;
        if(Physics.Raycast(cameraRay, out RaycastHit hit, interactionMaxDistance))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if(interactable != null)
            {
                interactable.Interact(this);
                return true;
            }
        }

        return false;
    }

    private void LoseALittleBitOfSanity()
    {
        float insanityProgress = CurrentBombingsDone / (float)droppingBombsNumToGoIntoMadness;
        new ALittleBitOfSanityLost(insanityProgress).Invoke();
    }

    private void TryToThrowGun()
    {
        if(gun)
        {
            gun.Throw();
        }
    }
}