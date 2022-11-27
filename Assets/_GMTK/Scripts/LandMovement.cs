using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMovement : MonoBehaviour
{
    [SerializeField]
    private float maxZPosition = 600f;
    [SerializeField]
    private float minZPosition = -600f;
    [SerializeField]
    private float zSpeed = 1f;

    void Update()
    {
        if(transform.position.z < minZPosition)
        {
            AdjustZPosition(maxZPosition);
        }
        else if(transform.position.z > maxZPosition)
        {
            AdjustZPosition(minZPosition);
        }

        AdjustZPosition(transform.position.z + zSpeed * Time.deltaTime);
    }

    private void AdjustZPosition(float zPosition)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
    }
}
