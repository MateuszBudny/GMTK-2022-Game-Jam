using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryManager : SingleBehaviour<StoryManager>
{
    [SerializeField]
    private List<AbstractThematicMonologuesData> thematicMonologuesDataList;

    [SerializeField]
    private MonologueTextLabel textLabel;

    public bool isDuringDialog { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        thematicMonologuesDataList.ForEach(monologues => monologues.Init());
    }

    public void PlayNextMonologue(AbstractThematicMonologuesData monologuesData)
    {
        isDuringDialog = true;
        monologuesData.PlayNextMonologue(textLabel);
        isDuringDialog = false;
    }
}
