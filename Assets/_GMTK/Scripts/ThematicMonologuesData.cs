using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThematicMonologuesData<TLineRecord> : AbstractThematicMonologuesData where TLineRecord : MonologueLineRecord
{
    [SerializeField]
    private ThematicMonologuesInitType initType = ThematicMonologuesInitType.Normal;
    [SerializeField]
    private List<List<TLineRecord>> lines;

    public Queue<TLineRecord> CreateLinesQueue
    {
        get
        {
            if (monologuesCurrentQueue.Count > 0)
            {
                return new Queue<TLineRecord>(monologuesCurrentQueue.Dequeue());
            }
            else
            {
                return new Queue<TLineRecord>(lines.GetRandomElement());
            }
        }
    }

    private Queue<List<TLineRecord>> monologuesCurrentQueue;

    public override void Init()
    {
        switch(initType)
        {
            case ThematicMonologuesInitType.Normal:
                monologuesCurrentQueue = new Queue<List<TLineRecord>>(lines);
                break;
            case ThematicMonologuesInitType.Shuffle:
                Debug.LogError("init");
                monologuesCurrentQueue = new Queue<List<TLineRecord>>(lines.Shuffle());
                break;
            case ThematicMonologuesInitType.FirstMonologueNormalShuffleOthers:
                List<List<TLineRecord>> shuffledLines = lines.TakeLast(lines.Count - 1).ToList().Shuffle();
                shuffledLines.Insert(0, lines[0]);
                monologuesCurrentQueue = new Queue<List<TLineRecord>>(shuffledLines);
                break;

        }
    }

    public override void PlayNextMonologue(MonologueTextLabel textLabel)
    {
        textLabel.ShowLines(this);
    }
}

public abstract class AbstractThematicMonologuesData : SerializedScriptableObject
{
    public abstract void Init();
    public abstract void PlayNextMonologue(MonologueTextLabel textLabel);
}

public enum ThematicMonologuesInitType
{
    Normal,
    Shuffle,
    FirstMonologueNormalShuffleOthers,
}