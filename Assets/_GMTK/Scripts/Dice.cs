using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField]
    private float confirmDiceValueAfterThoseSecondsOfBeingStill = 1;
	[SerializeField]
	private List<DiceFace> diceFaces;

    public int CurrentValue { get; private set; }
    public bool IsRolling => CurrentValue == 0;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void OnEnable()
    {
        diceFaces.ForEach(face => face.OnDiceStoppedOnThisFace += OnDiceStoppedRolling);
    }

    public void OnDisable()
    {
        diceFaces.ForEach(face => face.OnDiceStoppedOnThisFace -= OnDiceStoppedRolling);
    }

    public void Roll(Vector3 throwForceMin, Vector3 throwForceMax, Vector3 throwTorqueMin, Vector3 throwTorqueMax)
    {
        Roll(throwForceMin.RandomRange(throwForceMax), throwTorqueMin.RandomRange(throwTorqueMax));
    }

    public void Roll(Vector3 rollForce, Vector3 rollTorque)
    {
        CurrentValue = 0;
        diceFaces.ForEach(face => face.StartRolling(confirmDiceValueAfterThoseSecondsOfBeingStill));
        rigid.AddForce(rollForce);
        rigid.AddTorque(rollTorque);
    }

    public void TogglePhysicsAffection(bool active) => rigid.isKinematic = !active;

    private void OnDiceStoppedRolling(int valueRolled)
    {
        CurrentValue = valueRolled;
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void DebugRoll() => Roll(new Vector3(0f, 0.4f, 0f), new Vector3(37f, 21f, 47f));

    //private void Update()
    //{
    //    Debug.LogWarning("value: " + CurrentValue);
    //}
}
