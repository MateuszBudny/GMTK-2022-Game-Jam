using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonologueTextLabel : MonoBehaviour
{
    public TextMeshProUGUI textLabel;
    [SerializeField]
    private float showDuration = 4f;

    private float showStartedTimestamp = -1f;
    private Queue<MonologueLineRecord> currentLinesQueue;

    public bool IsTextLabelActiveAndEnabled
    {
        get 
        { 
            return textLabel != null && textLabel.isActiveAndEnabled; 
        }
    }

    public void ShowLines<T>(ThematicMonologuesData<T> linesData) where T : MonologueLineRecord
    {
        textLabel.enabled = true;
        currentLinesQueue = new Queue<MonologueLineRecord>(linesData.CreateLinesQueue);

        ShowSingleLine(currentLinesQueue.Dequeue());
    }

    private void ShowSingleLine(MonologueLineRecord singleLine)
    {
        singleLine.Play(this);
        showStartedTimestamp = Time.time;
    }

    private void Update()
    {
        if(textLabel.enabled && showStartedTimestamp > 0)
        {
            if(Time.time - showStartedTimestamp > showDuration)
            {
                if(currentLinesQueue.Count > 0)
                {
                    ShowSingleLine(currentLinesQueue.Dequeue());
                }
                else
                {
                    textLabel.enabled = false;
                    showStartedTimestamp = -1f;
                }
            }
        }
    }
}
