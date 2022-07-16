using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryManager : SingleBehaviour<StoryManager>
{
    [Header("Lines")]
    public SatanLines introLine;
    [SerializeField]
    private List<SatanLines> satanWinOrderedLines;
    [SerializeField]
    private List<SatanLines> satanLoseOrderedLines;
    [SerializeField]
    private List<SatanLines> satanDrawOrderedLines;

    [Header("Other")]
    [SerializeField]
    private MonologueTextLabel textLabel;

    private Queue<SatanLines> SatanWinLinesQueue { get; set; }
    private Queue<SatanLines> SatanLoseLinesQueue { get; set; }
    private Queue<SatanLines> SatanDrawLinesQueue { get; set; } 

    protected override void Awake()
    {
        base.Awake();
        SatanWinLinesQueue = new Queue<SatanLines>(satanWinOrderedLines);
        SatanLoseLinesQueue = new Queue<SatanLines>(satanLoseOrderedLines);
        SatanDrawLinesQueue = new Queue<SatanLines>(satanDrawOrderedLines);
    }

    public void ShowLines(SatanLines lines)
    {
        textLabel.ShowLines(lines);
    }

    public void ShowNextSatanWinLines()
    {
        ShowLines(SatanWinLinesQueue.Dequeue());
    }

    public void ShowNextSatanLoseLines()
    {
        ShowLines(SatanLoseLinesQueue.Dequeue());
    }

    public void ShowNextSatanDrawLines()
    {
        ShowLines(SatanDrawLinesQueue.Dequeue());
    }
}
