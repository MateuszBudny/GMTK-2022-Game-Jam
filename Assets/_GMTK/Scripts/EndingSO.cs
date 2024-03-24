using UnityEngine;

[CreateAssetMenu(fileName = "_EndingSO", menuName = "AleaIactaEst/EndingSO")]
public class EndingSO : ScriptableObject
{
    [SerializeField]
    private string description;

    public string ID => GetInstanceID().ToString();
    public string Description => description;
    public bool WasEndingAlreadyDone => PlayerPrefs.HasKey(ID);

    public void SetEndingAsDone()
    {
        PlayerPrefs.SetString(ID, "done");
    }
}
