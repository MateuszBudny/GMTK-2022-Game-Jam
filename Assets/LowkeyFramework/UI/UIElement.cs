using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class UIElement : MonoBehaviour
{
    [Header("Automatic Show/Hide")]
    [SerializeField]
    private bool automaticallyShowHidePanel = true;
    [SerializeField]
    [ShowIf(nameof(automaticallyShowHidePanel))]
    private bool showHideFirstChild = true;
    [SerializeField]
    [ShowIf(nameof(ShouldPanelToChooseToShowHideBeVisible))]
    private GameObject panelToShowHide;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onShowEvent;
    [SerializeField]
    private UnityEvent onHideEvent;

    private bool ShouldPanelToChooseToShowHideBeVisible => automaticallyShowHidePanel && !showHideFirstChild;

    private GameObject firstChild;

    private void Awake()
    {
        firstChild = transform.GetChild(0).gameObject;
    }

    public void Show()
    {
        TryToAutomaticallyShowHide(true);
        onShowEvent.Invoke();
    }

    public void Hide()
    {
        TryToAutomaticallyShowHide(false);
        onHideEvent.Invoke();
    }

    private void TryToAutomaticallyShowHide(bool show)
    {
        if(automaticallyShowHidePanel)
        {
            if(showHideFirstChild)
            {
                firstChild.SetActive(show);
            }
            else
            {
                panelToShowHide.SetActive(show);
            }
        }
    }
}
