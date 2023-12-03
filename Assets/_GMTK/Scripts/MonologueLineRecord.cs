using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using JetBrains.Annotations;

[Serializable]
public class SatanLines
{
    [SerializeField]
    private List<SatanLineRecord> lines;

    public Queue<SatanLineRecord> LinesQueue { get; private set; }

    public void Prepare()
    {
        LinesQueue = new Queue<SatanLineRecord>(lines);
    }
}

public abstract class MonologueLineRecord
{
    [TextArea]
    public string text;

    public virtual void Play(MonologueTextLabel monologueTextLabel)
    {
        monologueTextLabel.textLabel.text = text;
    }
}

[Serializable]
public class PlayerLineRecord : MonologueLineRecord
{
}

[Serializable]
public class SatanLineRecord : MonologueLineRecord
{
    public SatanFaceType satanFaceType;

    public override void Play(MonologueTextLabel monologueTextLabel)
    {
        base.Play(monologueTextLabel);
        GameplayManager.Instance.satan.SetFace(satanFaceType);
        SoundManager.Instance.PlaySatanTalking();
    }
}