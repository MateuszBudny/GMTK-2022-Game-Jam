using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatanFaces : MonoBehaviour
{
    [SerializeField]
    private Transform facesTransform;

    public SatanFaceType CurrentFace { get; private set; } = SatanFaceType.Joy;

    private float facesBaseRotation;
    private Tween facesRotationTween;

    private void Awake()
    {
        facesBaseRotation = facesTransform.localEulerAngles.y;
    }

    public void SetFace(SatanFaceType newFaceType)
    {
        bool isDuringRotation = false;
        if (facesRotationTween != null && facesRotationTween.IsPlaying())
        {
            facesRotationTween.Kill();
            isDuringRotation = true;
        }
        if (CurrentFace == newFaceType || isDuringRotation)
        {
            // if faces are still, then rotate by 360 degrees
            // but if they are during rotation, then rotate them to the target value by the longer rotation
            float rotateYBy = getFaceEndYRotationValue(newFaceType) - facesTransform.localEulerAngles.y;
            rotateYBy = MathUtils.RecalculateAngleToBetweenMinus180And180(rotateYBy);
            rotateYBy += rotateYBy > 0f ? -360f : 360f;
            doLocalRotateFaces(new Vector3(0f, rotateYBy, 0f), 3f, RotateMode.LocalAxisAdd, Audio.SatanSetFaceLonger);
        }
        else
        {
            float rotateYTo = getFaceEndYRotationValue(newFaceType);
            Vector3 endRotationValue = new Vector3(facesTransform.localEulerAngles.x, rotateYTo, facesTransform.localEulerAngles.z);
            doLocalRotateFaces(endRotationValue, 2f, RotateMode.Fast, Audio.SatanSetFaceShorter);
        }
        CurrentFace = newFaceType;

        void doLocalRotateFaces(Vector3 endRotationValue, float duration, RotateMode rotateMode, Audio audioToPlay)
        {
            facesRotationTween = facesTransform.DOLocalRotate(endRotationValue, duration, rotateMode);
            SoundManager.Instance.Play(audioToPlay);
        }

        float getFaceEndYRotationValue(SatanFaceType satanFace) => MathUtils.RecalculateAngleToBetweenMinus180And180(facesBaseRotation + 90f * (int)satanFace);
    }
}
