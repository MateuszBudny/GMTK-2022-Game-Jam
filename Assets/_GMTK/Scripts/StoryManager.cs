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
        if(SatanWinLinesQueue.Count > 0)
        {
            ShowLines(SatanWinLinesQueue.Dequeue());
        }
        else
        {
            GameplayManager.Instance.BurzaEnding();
        }
    }

    public void ShowNextSatanLoseLines()
    {
        if(SatanLoseLinesQueue.Count > 0)
        {
            ShowLines(SatanLoseLinesQueue.Dequeue());
        }
        else
        {
            ShowLines(satanLoseOrderedLines.GetRandomElement());
        }
    }

    public void ShowNextSatanDrawLines()
    {
        if(SatanDrawLinesQueue.Count > 0)
        {
            ShowLines(SatanDrawLinesQueue.Dequeue());
        }
        else
        {
            ShowLines(satanDrawOrderedLines.GetRandomElement());
        }
    }

    public void ShowNextSatanSuggestion()
    {
        if(SatanSuggestionsQueue.Count > 0)
        {
            ShowLines(SatanSuggestionsQueue.Dequeue());
        } 
        else
        {
            ShowLines(satanSuggestions.GetRandomElement());
        }
    }

    public void ShowNextPlayerShootingAtSatanLines()
    {
        ShowLines(PlayerShootingAtSatanLinesQueue.Dequeue());
    }
}
