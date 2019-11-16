using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence
{
    public virtual void SequenceStart()
    {

    }


    public virtual IEnumerator SequenceLoop()
    {
        yield return null;
    }


    public virtual void SequenceEnd()
    {

    }
}
