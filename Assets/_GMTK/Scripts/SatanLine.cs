using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

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

[Serializable]
public class SatanLineRecord
{
    [TextArea]
    public string text;
    public SatanFaceType satanFaceType;
}