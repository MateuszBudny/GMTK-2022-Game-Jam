using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StoryManager : SingleBehaviour<StoryManager>
{
    [SerializeField]
    private List<AbstractThematicMonologuesData> thematicMonologuesDataList;

    [SerializeField]
    private MonologueTextLabel textLabel;

    public bool IsDuringDialogue => textLabel != null && textLabel.IsTextLabelActiveAndEnabled;

    protected override void Awake()
    {
        base.Awake();

        thematicMonologuesDataList.ForEach(monologues => monologues.Init());
    }

    public void PlayNextMonologue(AbstractThematicMonologuesData monologuesData)
    {
        monologuesData.PlayNextMonologue(textLabel);
    }
}
