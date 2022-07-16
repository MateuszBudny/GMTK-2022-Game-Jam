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

    private void Update()
    {
        Debug.Log(CurrentValue);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void Roll()
    {
        CurrentValue = 0;
		diceFaces.ForEach(face => face.StartRolling(confirmDiceValueAfterThoseSecondsOfBeingStill));
        rigid.AddForce(new Vector3(0.3f, 2f, 0.2f));
        rigid.AddTorque(new Vector3(7f, 8f, 10f));
    }

    private void OnDiceStoppedRolling(int valueRolled)
    {
        CurrentValue = valueRolled;
    }
}
