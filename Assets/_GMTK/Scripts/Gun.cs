using DG.Tweening;
using System;
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
    [SerializeField]
    private Transform spawnShotPosition;
    [SerializeField]
    private GameObject shotEffectPrefab;
    [SerializeField]
    private AnimationCurve noiseShakeAmplitudeProgression;
    [SerializeField]
    private AnimationCurve noiseShakeFrequencyProgression;

    public int CurrentAmmo { get; private set; }
    public Rigidbody Rigid { get; private set; }
    public TransformNoiseMovement NoiseMovement { get; private set; }

    private int noAmmoCounter = 0;
    private Transform gunHolder;
    private float previousNoiseShakeFrequencyOffset = 0f;
    private float previousNoiseShakeAmplitudeOffset = 0f;

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
            float currentMovementFrequencyOffset = noiseShakeFrequencyProgression.Evaluate(Mathf.InverseLerp(0f, 170f, Mathf.Abs(MathUtils.RecalculateAngleToBetweenMinus180And180(gunHolder.localEulerAngles.y))));
            float currentMovementAmplitudeOffset = noiseShakeAmplitudeProgression.Evaluate(Mathf.InverseLerp(0f, 170f, Mathf.Abs(MathUtils.RecalculateAngleToBetweenMinus180And180(gunHolder.localEulerAngles.y))));
            NoiseMovement.MovementFrequencyGain += currentMovementFrequencyOffset - previousNoiseShakeFrequencyOffset;
            NoiseMovement.MovementAmplitudeGain += currentMovementAmplitudeOffset - previousNoiseShakeAmplitudeOffset;
            NoiseMovement.RotationFrequencyGain += currentMovementFrequencyOffset - previousNoiseShakeFrequencyOffset;
            NoiseMovement.RotationAmplitudeGain += currentMovementAmplitudeOffset - previousNoiseShakeAmplitudeOffset;

            previousNoiseShakeFrequencyOffset = currentMovementFrequencyOffset;
            previousNoiseShakeAmplitudeOffset = currentMovementAmplitudeOffset;
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


    private void Shoot()
    {
        CurrentAmmo--;
        Instantiate(shotEffectPrefab, spawnShotPosition.transform.position, Quaternion.identity, transform);
        SoundManager.Instance.Play(Audio.Gunshot);

        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
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

    private void NoAmmo()
    {
        if(noAmmoCounter == 2)
        {
            Throw();
        }
        else
        {
            noAmmoCounter++;
            SoundManager.Instance.Play(Audio.NoAmmo);
        }
    }
}
