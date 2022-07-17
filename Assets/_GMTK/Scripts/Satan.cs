using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satan : DicePlayer
{
    [SerializeField]
    private Transform facesTransform;

    public SatanFaceType CurrentFace { get; private set; } = SatanFaceType.Joy;

    private void Start()
    {
        GameplayManager.Instance.OnPlayerThrewDices += OnPlayerThrewDices;
    }

    private void OnDisable()
    {
        GameplayManager.Instance.OnPlayerThrewDices -= OnPlayerThrewDices;
    }

    public void SetFace(SatanFaceType newFaceType)
    {
        //if(newFaceType == CurrentFace)
        //{
        //    return;
        //}


        int diff = (int)CurrentFace - (int)newFaceType;
        float currentRotation = facesTransform.eulerAngles.y;
        Vector3 endRotationValue = new Vector3(facesTransform.eulerAngles.x, currentRotation - 90f * diff, facesTransform.eulerAngles.z);
        if(diff == 0)
        {
            diff = 4;
            facesTransform.DORotate(new Vector3(0f, 360f, 0f), 2f, RotateMode.FastBeyond360).SetRelative(true);
        }
        else
        {
            facesTransform.DORotate(endRotationValue, 2f);
        }
        CurrentFace = newFaceType;
    }

    private void OnPlayerThrewDices()
    {
        StartCoroutine(AfterPlayerThrewDicesEnumerator());
    }

    private IEnumerator AfterPlayerThrewDicesEnumerator()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Your score: " + GameplayManager.Instance.player.CurrentScore);
        GameplayManager.Instance.ChangeState(GameState.SatanTurn);
        diceThrowing.ThrowDices();
        GameplayManager.Instance.SatanThrewDices();
    }
}
