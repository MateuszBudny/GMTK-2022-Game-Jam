using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimHelper : MonoBehaviour
{
    [SerializeField]
    private Gun gun;

    public void FireTheBullet()
    {
        gun.FireTheBullet();
    }

    public void UnloadingFinished()
    {
        gun.OnUnloadingFinished();
    }
}
