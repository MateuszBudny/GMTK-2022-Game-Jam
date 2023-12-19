using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.HID;

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
    private bool isShootingCalled = false;

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
        if(!isShootingCalled && Physics.Raycast(spawnShotPosition.position, spawnShotPosition.forward, out RaycastHit hit))
        {
            IAimable aimable = hit.collider.GetComponentInParent<IAimable>();
            if (aimable != null)
            {
                aimable.IsAimedAt(this);
            }
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

    public async void FireTheBullet()
    {
        isShootingCalled = true;
        CurrentAmmo--;
        Instantiate(shotEffectPrefab, spawnShotPosition.transform.position, Quaternion.identity, transform);
        SoundManager.Instance.Play(Audio.Gunshot);
        if(isShootingCalled && Physics.Raycast(spawnShotPosition.position, spawnShotPosition.forward, out RaycastHit hit))
        {
            IShootable shootable = hit.collider.GetComponentInParent<IShootable>();
            if (shootable != null)
            {
                shootable.GetShot(this);

            }
        }
        await Task.Delay(1500);
        isShootingCalled = false;
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
