using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using StarterAssets;

public class Player : DicePlayer
{
    [SerializeField]
    private float interactionMaxDistance = 10f;
    [SerializeField]
    private FirstPersonController playerController;
    [SerializeField]
    [Range(0f, 1f)]
    private float maxVignetteValue = 0.8f;

    public Transform gunHolder;
    public int droppingBombsNumToGoIntoMadness = 5;

    private int currentBombingsDone = 0;
    public int CurrentBombingsDone 
    {
        get => currentBombingsDone;
        set
        {
            currentBombingsDone = value;
            LoseALittleBitOfSanity();
        }
    }
    
    public bool IsPlayerHoldingGun => gun;
    public bool IsGonnaSnap => CurrentBombingsDone + 1 >= droppingBombsNumToGoIntoMadness;

    private Gun gun;

    public void OnPlayerAction(InputValue inputValue)
    {
        if(inputValue.isPressed && GameplayManager.Instance.State != GameState.GameOver)
        {
            if(IsPlayerHoldingGun)
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
            if(hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact(this);
                return true;
            }
        }

        return false;
    }

    private void LoseALittleBitOfSanity()
    {
        float oneBombDropVignetteValue = maxVignetteValue / droppingBombsNumToGoIntoMadness;
        float vignetteEndValueForNow = oneBombDropVignetteValue * CurrentBombingsDone;
        GameplayManager.Instance.postProcessController.SetVignetteSmoothly(vignetteEndValueForNow, 1.5f);

        if(CurrentBombingsDone >= droppingBombsNumToGoIntoMadness)
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