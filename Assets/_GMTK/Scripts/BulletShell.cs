using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    [SerializeField]
    private float maxDelayInDrop = 1f;
    [SerializeField]
    private Vector3 minRandomRotationForceOnFall = Vector3.zero;
    [SerializeField]
    private Vector3 maxRandomRotationForceOnFall;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void OnUnloadingDrop()
    {
        float delay = Random.Range(0f, maxDelayInDrop);
        StartCoroutine(DelayedDrop(delay));
    }

    private IEnumerator DelayedDrop(float delay)
    {
        yield return new WaitForSeconds(delay);

        Drop();
    }

    private void Drop()
    {
        transform.parent = null;
        rigid.isKinematic = false;

        Vector3 randomRotationForce = minRandomRotationForceOnFall.RandomRange(maxRandomRotationForceOnFall);
        rigid.AddTorque(randomRotationForce, ForceMode.Force);
    }
}
