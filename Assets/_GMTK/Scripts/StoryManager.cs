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
    public SatanLines burzaEndingStarts;
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

    protected void Start()
    {
        SatanLines firstSatanWinLines = satanWinOrderedLines[0];
        int satanWinLinesToRemoveNum = satanWinOrderedLines.Count - GameplayManager.Instance.player.droppingBombsNumToGoIntoMadness;
        for(int i = 0; i < satanWinLinesToRemoveNum; i++)
        {
            satanWinOrderedLines.Remove(satanWinOrderedLines.GetRandomElement());
        }

        if(!satanWinOrderedLines.Contains(firstSatanWinLines))
        {
            satanWinOrderedLines.Remove(satanWinOrderedLines.GetRandomElement());
            satanWinOrderedLines.Insert(0, firstSatanWinLines);
        }

        SatanWinLinesQueue = new Queue<SatanLines>(satanWinOrderedLines);
        SatanLoseLinesQueue = new Queue<SatanLines>(satanLoseOrderedLines);
        SatanDrawLinesQueue = new Queue<SatanLines>(satanDrawOrderedLines);
        SatanSuggestionsQueue = new Queue<SatanLines>(satanSuggestions);
        PlayerShootingAtSatanLinesQueue = new Queue<SatanLines>(playerShootingAtSatanOrderedLines);
    }

    public void ShowLines(SatanLines lines)
    {
        textLabel.ShowLines(lines);
        SoundManager.Instance.PlaySatanTalking();
    }

    public void ShowNextSatanWinLines()
    {
        ShowNextSatanLines(SatanWinLinesQueue, satanWinOrderedLines);
    }

    public void ShowNextSatanLoseLines()
    {
        ShowNextSatanLines(SatanLoseLinesQueue, satanLoseOrderedLines);
    }

    public void ShowNextSatanDrawLines()
    {
        ShowNextSatanLines(SatanDrawLinesQueue, satanDrawOrderedLines);
    }

    public void ShowNextSatanSuggestion()
    {
        ShowNextSatanLines(SatanSuggestionsQueue, satanSuggestions);
    }

    public void ShowNextPlayerShootingAtSatanLines()
    {
        ShowNextSatanLines(PlayerShootingAtSatanLinesQueue, playerShootingAtSatanOrderedLines);
    }

    private void ShowNextSatanLines(Queue<SatanLines> queueWithLines, List<SatanLines> baseListWithLines)
    {
        if(queueWithLines.Count > 0)
        {
            ShowLines(queueWithLines.Dequeue());
        }
        else
        {
            ShowLines(baseListWithLines.GetRandomElement());
        }
    }
}
