using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class Player : DicePlayer
{
    [SerializeField]
    private float interactionMaxDistance = 10f;
    public Transform gunHolder;

    private Gun gun;

    public void OnPlayerAction(InputValue inputValue)
    {
        if(inputValue.isPressed)
        {
            if(GameplayManager.Instance.State == GameState.PlayerTurn)
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
            else if(GameplayManager.Instance.State == GameState.PlayerHoldingGun)
            {
                gun.TryToShoot();
            }
            else if(GameplayManager.Instance.State == GameState.PlayerAsSatan)
            {
                diceThrowing.SpawnAndThrowDices();
                TryToInteract();
            }
            else if(GameplayManager.Instance.State != GameState.SatanMonolog && GameplayManager.Instance.State != GameState.SatanThrewDices && GameplayManager.Instance.State != GameState.SatanTurn && GameplayManager.Instance.State != GameState.GameOver)
            {
                TryToInteract();
            }
        }
    }

    public void TakeGun(Gun gun)
    {
        this.gun = gun;
        gun.Rigid.isKinematic = true;
        gun.transform.SetParent(gunHolder);
        gun.transform.DOLocalMove(Vector3.zero, 1f);
        gun.transform.DOLocalRotate(Vector3.zero, 1f);
        GameplayManager.Instance.ChangeState(GameState.PlayerHoldingGun);
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
}