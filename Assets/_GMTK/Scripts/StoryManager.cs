using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoryManager : SingleBehaviour<StoryManager>
{
    [SerializeField]
    private List<AbstractThematicMonologuesData> thematicMonologuesDataList;

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
    [SerializeField]
    private List<SatanLines> satanReaccToPlayerDiceNotFallingOnTheCrate;

    [Header("Other")]
    [SerializeField]
    private MonologueTextLabel textLabel;

    private Queue<SatanLines> SatanWinLinesQueue { get; set; }
    private Queue<SatanLines> SatanLoseLinesQueue { get; set; }
    private Queue<SatanLines> SatanDrawLinesQueue { get; set; } 
    private Queue<SatanLines> SatanSuggestionsQueue { get; set; }
    private Queue<SatanLines> SatanReaccToPlayerDiceNotFallingOnTheCrate { get; set; }
    private Queue<SatanLines> PlayerShootingAtSatanLinesQueue { get; set; } 

    protected override void Awake()
    {
        base.Awake();

        thematicMonologuesDataList.ForEach(monologues => monologues.Init());

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
        SatanReaccToPlayerDiceNotFallingOnTheCrate = new Queue<SatanLines>(satanReaccToPlayerDiceNotFallingOnTheCrate);
    }

    public void PlayNextMonologue(AbstractThematicMonologuesData monologuesData)
    {
        monologuesData.PlayNextMonologue(textLabel);
    }

    public void ShowLines(SatanLines lines)
    {
        //textLabel.ShowLines(lines);
        
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

    public void ShowNextSatanReaccToPlayerDiceNotFallingOnTheCrate()
    {
        ShowNextSatanLines(SatanReaccToPlayerDiceNotFallingOnTheCrate, satanReaccToPlayerDiceNotFallingOnTheCrate);
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
