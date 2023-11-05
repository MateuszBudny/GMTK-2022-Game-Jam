using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IInteractable
{
    public int startingAmmo = 6;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float throwForce = 500f;
    [SerializeField]
    private float throwTorque = 20f;
    [SerializeField]
    private Transform spawnShotPosition;
    [SerializeField]
    private GameObject shotEffectPrefab;

    public int CurrentAmmo { get; private set; }
    public Rigidbody Rigid { get; private set; }
    public TransformNoiseMovement NoiseMovement { get; private set; }

    private int noAmmoCounter = 0;
    private Transform gunHolder;
    private bool isUnloaded = false;

    private void Awake()
    {
        CurrentAmmo = startingAmmo;
        Rigid = GetComponent<Rigidbody>();

        NoiseMovement = GetComponent<TransformNoiseMovement>();
    }

    private void Update()
    {
        if(NoiseMovement.IsActive)
        {
            float noiseProgress = Mathf.InverseLerp(0f, 170f, Mathf.Abs(MathUtils.RecalculateAngleToBetweenMinus180And180(gunHolder.localEulerAngles.y)));
            NoiseMovement.UpdateProgress(noiseProgress);
        }
    }

    public void Interact(Player player)
    {
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

    public void Take(Transform gunHolder, Action onTakingAnimationFinished)
    {
        this.gunHolder = gunHolder;
        Rigid.isKinematic = true;
        transform.SetParent(this.gunHolder);
        transform.DOLocalMove(Vector3.zero, 1f);
        transform.DOLocalRotate(Vector3.zero, 1f);
        Tweener gunHolderTweener = this.gunHolder.transform.DOLocalRotate(new Vector3(0f, this.gunHolder.parent.localEulerAngles.y, 0f), 1f)
            .OnComplete(() =>
            {
                NoiseMovement.SetActive(true);
                onTakingAnimationFinished();
            });
    }

    public void Throw()
    {
        transform.SetParent(null);
        Rigid.isKinematic = false;
        Rigid.AddForce(transform.forward * throwForce);
        Rigid.AddTorque(new Vector3(throwTorque, 0f, 0f));
        NoiseMovement.SetActive(false);
        Destroy(this);
    }

    public void OnUnloadingFinished()
    {
        isUnloaded = true;
    }
    
    public void FireTheBullet()
    {
        CurrentAmmo--;
        Instantiate(shotEffectPrefab, spawnShotPosition.transform.position, Quaternion.identity, transform);
        SoundManager.Instance.Play(Audio.Gunshot);

        if(Physics.Raycast(spawnShotPosition.position, spawnShotPosition.forward, out RaycastHit hit))
        {
            if(hit.collider.CompareTag(Tags.Player.ToString()))
            {
                if(GameplayManager.Instance.State != GameState.PlayerAsSatan)
                {
                    GameplayManager.Instance.Suicide();
                }
            }
            if(hit.collider.CompareTag(Tags.Satan.ToString()))
            {
                StoryManager.Instance.ShowNextPlayerShootingAtSatanLines();
            }
        }
    }

    private void Shoot()
    {
        animator.SetTrigger("Shoot");
    }

    private void NoAmmo()
    {
        if(noAmmoCounter == 2)
        {
            if(!isUnloaded)
            {
                animator.SetTrigger("Unload");
            }
            else
            {
                Throw();
            } 
        }
        else
        {
            noAmmoCounter++;
            SoundManager.Instance.Play(Audio.NoAmmo);
        }
    }
}
