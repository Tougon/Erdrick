using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEE : MonoBehaviour
{
    public AnimationSequenceObject aso;

    // Start is called before the first frame update
    void Start()
    {
        AnimationSequence seq = new AnimationSequence(aso, GetComponent<Entity>(), null);
        seq.SequenceStart();
        StartCoroutine(seq.SequenceLoop());
    }
}
