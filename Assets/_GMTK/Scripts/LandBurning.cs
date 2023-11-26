using AetherEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandBurning : MonoBehaviour
{
    [SerializeField]
    private Material burninglandMaterial;
    [SerializeField]
    private int onAfterWhichDroppingChangeToBurning = -1;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        HullOpeningForBombsDroppingStarted.AddListener(OnHullOpeningForBombingStarted);
    }

    private void OnHullOpeningForBombingStarted(HullOpeningForBombsDroppingStarted eventData)
    {
        int compareWith = onAfterWhichDroppingChangeToBurning > 0 ? onAfterWhichDroppingChangeToBurning : eventData.DroppingsNumToGoIntoMadness + onAfterWhichDroppingChangeToBurning;
        if(eventData.WhichDroppingIsThis == compareWith)
        {
            ChangeToBurning();
        }
    }

    private void ChangeToBurning()
    {
        meshRenderer.material = burninglandMaterial;
    }
}
