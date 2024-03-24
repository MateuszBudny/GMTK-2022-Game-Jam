using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndingsCounter : MonoBehaviour
{
    [SerializeField]
    private List<EndingSO> allEndings;

    public int AllEndingsNum => allEndings.Count;

    public int DoneEndingsNum
    {
        get
        {
            return allEndings.Aggregate(0, (doneEndingsNum, ending) =>
            {
                if(ending.WasEndingAlreadyDone)
                {
                    doneEndingsNum++;
                }

                return doneEndingsNum;
            });
        }
    }
}
