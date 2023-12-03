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

    public Queue<TLineRecord> CreateLinesQueue => new Queue<TLineRecord>(monologuesCurrentQueue.Dequeue());

    private Queue<List<TLineRecord>> monologuesCurrentQueue;

    public override void Init()
    {
        switch(initType)
        {
            case ThematicMonologuesInitType.Normal:
                monologuesCurrentQueue = new Queue<List<TLineRecord>>(lines);
                break;
            case ThematicMonologuesInitType.FirstMonologueNormalShuffleOthers:
                monologuesCurrentQueue = new Queue<List<TLineRecord>>();
                monologuesCurrentQueue.Enqueue(lines[0]);
                List<List<TLineRecord>> linesWithoutFirst = lines.TakeLast(lines.Count - 1).ToList();
                for(int i = 0; i < linesWithoutFirst.Count; i++)
                {
                    List<TLineRecord> monologueDrawn = linesWithoutFirst.GetRandomElement();
                    monologuesCurrentQueue.Enqueue(monologueDrawn);
                    linesWithoutFirst.Remove(monologueDrawn);
                }
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
    FirstMonologueNormalShuffleOthers,
}