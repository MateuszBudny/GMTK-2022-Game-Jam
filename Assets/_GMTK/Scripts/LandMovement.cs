using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMovement : MonoBehaviour
{
    [SerializeField]
    private List<Transform> landsLayers;
    [SerializeField]
    private float zSpeed = 1f;

    void Update()
    {
        ForEveryLandsLayer(landsLayer =>
        {
            if(landsLayer.localPosition.z > 10)
            {
                // move this lands layer to the back
                AdjustZPosition(landsLayer, -2 * landsLayer.localPosition.z * landsLayer.parent.localScale.z);
            } 
        });

        float zChange = zSpeed * Time.deltaTime;
        ForEveryLandsLayer(landsLayer =>
        {
            AdjustZPosition(landsLayer, zChange);
        });
    }

    private void ForEveryLandsLayer(Action<Transform> action)
    {
        foreach(Transform landsLayer in landsLayers)
        {
            action(landsLayer);
        }
    }

    private void AdjustZPosition(Transform transformToAdjust, float zChange)
    {
        transformToAdjust.position = new Vector3(transformToAdjust.position.x, transformToAdjust.position.y, transformToAdjust.position.z + zChange);
    }
}
