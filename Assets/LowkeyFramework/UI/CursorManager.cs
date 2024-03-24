using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private CursorLockMode cursorLockMode = CursorLockMode.Locked;

    private void Start()
    {
        Cursor.lockState = cursorLockMode;
    }

    public void OnApplicationFocus(bool hasFocus)
    {
        if(hasFocus)
        {
            Cursor.lockState = cursorLockMode;
        }
    }
}
