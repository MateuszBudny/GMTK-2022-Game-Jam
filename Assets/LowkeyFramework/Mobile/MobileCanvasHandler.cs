using UnityEngine;

public class MobileCanvasHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject mobileUI;

    private void Awake()
    {
#if UNITY_ANDROID
        mobileUI.SetActive(true);
#else
        mobileUI.SetActive(false);
#endif
    }
}
