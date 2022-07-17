using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonologueTextLabel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textLabel;
    [SerializeField]
    private float showDuration = 4f;

    private float showStartedTimestamp = -1f;
    private SatanLines currentLines;

    public void ShowLines(SatanLines lines)
    {
        textLabel.enabled = true;
        currentLines = lines;
        currentLines.Prepare();

        ShowSingleLine(lines.LinesQueue.Dequeue());
    }

    private void ShowSingleLine(SatanLineRecord singleLine)
    {
        textLabel.text = singleLine.text;
        GameplayManager.Instance.satan.SetFace(singleLine.satanFaceType);
        showStartedTimestamp = Time.time;
    }

    private void Update()
    {
        if(textLabel.enabled && showStartedTimestamp > 0)
        {
            if(Time.time - showStartedTimestamp > showDuration)
            {
                if(currentLines.LinesQueue.Count > 0)
                {
                    ShowSingleLine(currentLines.LinesQueue.Dequeue());
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
