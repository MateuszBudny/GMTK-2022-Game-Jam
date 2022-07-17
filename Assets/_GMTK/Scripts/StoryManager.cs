using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryManager : SingleBehaviour<StoryManager>
{
    [Header("Lines")]
    public SatanLines introLine;
    public SatanLines waitingLine;
    public SatanLines playerAimingWithGunAtHimself;
    public SatanLines playerAimingWithGunAtSatan;
    [SerializeField]
    private List<SatanLines> playerShootingAtSatanOrderedLines;
    [SerializeField]
    private List<SatanLines> satanWinOrderedLines;
    [SerializeField]
    private List<SatanLines> satanLoseOrderedLines;
    [SerializeField]
    private List<SatanLines> satanDrawOrderedLines;
    [SerializeField]
    private List<SatanLines> satanSuggestions;

    [Header("Other")]
    [SerializeField]
    private MonologueTextLabel textLabel;

    private Queue<SatanLines> SatanWinLinesQueue { get; set; }
    private Queue<SatanLines> SatanLoseLinesQueue { get; set; }
    private Queue<SatanLines> SatanDrawLinesQueue { get; set; } 
    private Queue<SatanLines> SatanSuggestionsQueue { get; set; }
    private Queue<SatanLines> PlayerShootingAtSatanLinesQueue { get; set; } 

    protected override void Awake()
    {
        base.Awake();
        SatanWinLinesQueue = new Queue<SatanLines>(satanWinOrderedLines);
        SatanLoseLinesQueue = new Queue<SatanLines>(satanLoseOrderedLines);
        SatanDrawLinesQueue = new Queue<SatanLines>(satanDrawOrderedLines);
        SatanSuggestionsQueue = new Queue<SatanLines>(satanSuggestions);
        PlayerShootingAtSatanLinesQueue = new Queue<SatanLines>(playerShootingAtSatanOrderedLines);
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

    public void ShowNextSatanSuggestion()
    {
        ShowLines(SatanSuggestionsQueue.Dequeue());
    }

    public void ShowNextPlayerShootingAtSatanLines()
    {
        ShowLines(PlayerShootingAtSatanLinesQueue.Dequeue());
    }
}
