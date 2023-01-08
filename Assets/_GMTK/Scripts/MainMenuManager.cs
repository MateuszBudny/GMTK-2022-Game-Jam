using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private BlackScreen blackScreen;
    [SerializeField]
    private float fadeOutDuration = 4f;
    [SerializeField]
    private float fadeInDuration = 4f;

    private void Start()
    {
        blackScreen.FadeOut(fadeOutDuration, true);
    }

    public void OpenBomberScene()
    {
        blackScreen.FadeIn(fadeInDuration, false, () => SceneManager.LoadScene("Bomber"));
    }
}
