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
    [SerializeField]
    private GunNoiseShakeSingleGainOffset noiseAmplitudeGainOffset;
    [SerializeField]
    private GunNoiseShakeSingleGainOffset noiseFrequencyGainOffset;

    public int CurrentAmmo { get; private set; }
    public Rigidbody Rigid { get; private set; }
    public TransformNoiseMovement NoiseMovement { get; private set; }

    private int noAmmoCounter = 0;
    private Transform gunHolder;
    private bool isUnloaded = false;
    private float previousNoiseShakeFrequencyOffset = 0f;
    private float previousNoiseShakeAmplitudeOffset = 0f;

    private void Awake()
    {
        CurrentAmmo = startingAmmo;
        Rigid = GetComponent<Rigidbody>();

        NoiseMovement = GetComponent<TransformNoiseMovement>();
        noiseAmplitudeGainOffset.Init(NoiseMovement,
            noise => noise.movementNoiseShake.amplitudeGain,
            (noise, value) => noise.movementNoiseShake.amplitudeGain = value,
            noise => noise.rotationNoiseShake.amplitudeGain,
            (noise, value) => noise.rotationNoiseShake.amplitudeGain = value);
        noiseFrequencyGainOffset.Init(NoiseMovement,
            noise => noise.movementNoiseShake.frequencyGain,
            (noise, value) => noise.movementNoiseShake.frequencyGain = value,
            noise => noise.rotationNoiseShake.frequencyGain,
            (noise, value) => noise.rotationNoiseShake.frequencyGain = value);
    }

    private void Update()
    {
        if(NoiseMovement.IsActive)
        {
            noiseAmplitudeGainOffset.UpdateGainOffsetValue(gunHolder);
            noiseFrequencyGainOffset.UpdateGainOffsetValue(gunHolder);
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

    [Serializable]
    private class GunNoiseShakeSingleGainOffset
    {
        [SerializeField]
        private AnimationCurve progression;

        private TransformNoiseMovement noiseMovement;
        private Func<TransformNoiseMovement, float> movementNoiseGainOffsetGetter;
        private Action<TransformNoiseMovement, float> movementNoiseGainOffsetSetter;
        private Func<TransformNoiseMovement, float> rotationNoiseGainOffsetGetter;
        private Action<TransformNoiseMovement, float> rotationNoiseGainOffsetSetter;
        private float previousOffset = 0f;

        public void Init(TransformNoiseMovement noiseMovement, Func<TransformNoiseMovement, float> movementNoiseGainOffsetGetter, Action<TransformNoiseMovement, float> movementNoiseGainOffsetSetter, Func<TransformNoiseMovement, float> rotationNoiseGainOffsetGetter, Action<TransformNoiseMovement, float> rotationNoiseGainOffsetSetter)
        {
            this.noiseMovement = noiseMovement;
            this.movementNoiseGainOffsetGetter = movementNoiseGainOffsetGetter;
            this.movementNoiseGainOffsetSetter = movementNoiseGainOffsetSetter;
            this.rotationNoiseGainOffsetGetter = rotationNoiseGainOffsetGetter;
            this.rotationNoiseGainOffsetSetter = rotationNoiseGainOffsetSetter;
        }

        public void UpdateGainOffsetValue(Transform gunHolder)
        {
            float currentOffset = progression.Evaluate(Mathf.InverseLerp(0f, 170f, Mathf.Abs(MathUtils.RecalculateAngleToBetweenMinus180And180(gunHolder.localEulerAngles.y))));
            movementNoiseGainOffsetSetter(noiseMovement, movementNoiseGainOffsetGetter(noiseMovement) + currentOffset - previousOffset);
            previousOffset = currentOffset;
        }
    }
}
