using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimHelper : MonoBehaviour
{
    [SerializeField]
    private Gun gun;
    [SerializeField]
    private List<BulletShell> bulletShells;

    public void FireTheBullet()
    {
        gun.FireTheBullet();
    }

    public void UnloadingFinished()
    {
        gun.OnUnloadingFinished();
    }

    public void UnloadingShellsDrop()
    {
        bulletShells.ForEach(shell => shell.OnUnloadingDrop());
    }
}
