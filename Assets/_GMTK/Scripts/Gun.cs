using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IInteractable
{
    public int startingAmmo = 6;
    [SerializeField]
    private float throwForce = 500f;
    [SerializeField]
    private float throwTorque = 20f;

    public int CurrentAmmo { get; private set; }
    public Rigidbody Rigid { get; private set; }

    private int noAmmoCounter = 0;
    private GameState stateBeforeTakingGun;

    private void Awake()
    {
        Rigid = GetComponent<Rigidbody>();
        CurrentAmmo = startingAmmo;
    }

    public void Interact(Player player)
    {
        stateBeforeTakingGun = GameplayManager.Instance.State;
        player.TakeGun(this);
    }

    public void TryToShoot()
    {
        if(CurrentAmmo > 0)
        {
            Shoot();
        }
        else
        {
            NoAmmo();
        }
    }

    private void Shoot()
    {
        CurrentAmmo--;
        Debug.LogWarning("bang");
    }

    private void NoAmmo()
    {
        if(noAmmoCounter == 3)
        {
            Throw();
        }
        noAmmoCounter++;
    }

    private void Throw()
    {
        transform.SetParent(null);
        Rigid.isKinematic = false;
        Rigid.AddForce(transform.forward * throwForce);
        Rigid.AddTorque(new Vector3(throwTorque, 0f, 0f));
        GameplayManager.Instance.ChangeState(stateBeforeTakingGun);
        Destroy(this);
    }
}
