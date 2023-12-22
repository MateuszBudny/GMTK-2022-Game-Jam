using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using StarterAssets;
using AetherEvents;

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

    private Gun gun;

    protected override void Awake()
    {
        base.Awake();
        BombsDropped.AddListener(OnBombsDropping);
    }

    private void Start()
    {
        playerController.FreezeCameraRotation = true;
    }

    public void OnPlayerAction(InputValue inputValue)
    {
        if(inputValue.isPressed && GameplayManager.Instance.State != GameState.GameOver)
        {
            if(GameplayManager.Instance.State == GameState.ClosedEyes)
            {
                GameplayManager.Instance.OpenEyes();
                playerController.FreezeCameraRotation = false;
            }
            else if(IsPlayerHoldingGun)
            {
                gun.TryToShoot();
            }
            else if(GameplayManager.Instance.State == GameState.PlayerTurn)
            {
                diceThrowing.ThrowDices();
                GameplayManager.Instance.PlayerThrewDices();
            }
            else if(GameplayManager.Instance.State == GameState.PlayerCanInteract)
            {
                if(!TryToInteract())
                {
                    PrepareForGame();
                    GameplayManager.Instance.ChangeState(GameState.PlayerTurn);
                }
            }
            else if(GameplayManager.Instance.State == GameState.PlayerAsSatan)
            {
                diceThrowing.SpawnAndThrowDices();
            }
            else if(GameplayManager.Instance.State != GameState.SatanMonolog && GameplayManager.Instance.State != GameState.SatanThrewDices && GameplayManager.Instance.State != GameState.SatanTurn)
            {
                TryToInteract();
            }
        }
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
        if (!StoryManager.Instance.isDuringDialog && !playerAimingAtHimselfMonologues.HasUsedAllLinesOnce)
        {
            StoryManager.Instance.PlayNextMonologue(playerAimingAtHimselfMonologues);
        }

    }

    public void GetShot(Gun gunShooting)
    {
        if (GameplayManager.Instance.State != GameState.PlayerAsSatan)
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

    private void OnBombsDropping(BombsDropped eventData)
    {
        LoseALittleBitOfSanity();
    }

    private void LoseALittleBitOfSanity()
    {
        float insanityProgress = CurrentBombingsDone / (float)droppingBombsNumToGoIntoMadness;
        new ALittleBitOfSanityLost(insanityProgress).Invoke();

        if (CurrentBombingsDone >= droppingBombsNumToGoIntoMadness)
        {
            GameplayManager.Instance.BurzaEnding();
        }
    }

    private void TryToThrowGun()
    {
        if(gun)
        {
            gun.Throw();
        }
    }
}