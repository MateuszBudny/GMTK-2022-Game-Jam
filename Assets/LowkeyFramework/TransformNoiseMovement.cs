using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformNoiseMovement : MonoBehaviour
{
    [SerializeField]
    private Transform transformToNoiseMove;
    [SerializeField]
    private bool isActive = true;
    [SerializeField]
    private float movementAmplitudeGain = 1f;
    [SerializeField]
    private float movementFrequencyGain = 1f;
    [SerializeField]
    private float rotationAmplitudeGain = 0.1f;
    [SerializeField]
    private float rotationFrequencyGain = 0.1f;

    public bool IsActive { get => isActive; private set => isActive = value; }
    public float MovementAmplitudeGain { get => movementAmplitudeGain; set => movementAmplitudeGain = value; }
    public float MovementFrequencyGain { get => movementFrequencyGain; set => movementFrequencyGain = value; }
    public float RotationAmplitudeGain { get => rotationAmplitudeGain; set => rotationAmplitudeGain = value; }
    public float RotationFrequencyGain { get => rotationFrequencyGain; set => rotationFrequencyGain = value; }

    private Vector3 previousNoiseMovement = Vector3.zero;
    private Vector3 previousNoiseRotation = Vector3.zero;
    private Vector3 movementSeeds = Vector3.zero;
    private Vector3 rotationSeeds = Vector3.zero;
    private float movementElapsedTime = 0f;
    private float rotationElapsedTime = 0f;

    private void Awake()
    {
        movementSeeds = Random.insideUnitSphere;
        rotationSeeds = Random.insideUnitSphere;
    }

    private void Update()
    {
        if(!IsActive)
            return;

        movementElapsedTime += Time.deltaTime * movementFrequencyGain;
        rotationElapsedTime += Time.deltaTime * rotationFrequencyGain;

        Vector3 currentNoiseMovement = MathUtils.RandomSmoothOffsetNoise(movementElapsedTime, MovementAmplitudeGain, 1f, movementSeeds.x, movementSeeds.y, movementSeeds.z);
        Vector3 currentNoiseRotation = MathUtils.RandomSmoothOffsetNoise(rotationElapsedTime, RotationAmplitudeGain, 1f, rotationSeeds.x, rotationSeeds.y, rotationSeeds.z);
        transformToNoiseMove.Translate(currentNoiseMovement - previousNoiseMovement);
        transformToNoiseMove.Rotate(currentNoiseRotation - previousNoiseRotation);
        previousNoiseMovement = currentNoiseMovement;
        previousNoiseRotation = currentNoiseRotation;
    }

    public void SetActive(bool active)
    {
        if(IsActive && !active)
        {
            transformToNoiseMove.Translate(-previousNoiseMovement);
            transformToNoiseMove.Rotate(-previousNoiseRotation);
            previousNoiseMovement = Vector3.zero;
            previousNoiseRotation = Vector3.zero;
        }

        IsActive = active;
    }
}
