using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class AnimationSequence : Sequence
{

    public class AnimationSequenceAction
    {
        public enum Action { ChangeUserAnimation, ChangeTargetAnimation, TerminateAnimation }

        public int frame;
        public Action action;
        public string param;
    }


    private bool active = false;
    private bool initialized = false;
    private int currentFrame = 0;

    public string sequenceName { get; private set; }

    private Player user;
    private Player target;
    private List<GameObject> effects = new List<GameObject>();

    private List<AnimationSequenceAction> sequenceActions = new List<AnimationSequenceAction>();


    public AnimationSequence(AnimationSequenceObject obj, Player u, Player t)
    {
        user = u;
        target = t;

        string[] sequence = Regex.Split(obj.animationSequence.text, "\n");

        for(int i=0; i<sequence.Length; i++)
        {
            string[] line = sequence[i].Split('|');

            if(line.Length > 3 || line.Length < 2)
            {
                Debug.LogError("Invalid format on line " + i + "!");
                return;
            }

            AnimationSequenceAction seq = new AnimationSequenceAction();

            seq.frame = int.Parse(line[0]);
            seq.action = (AnimationSequenceAction.Action)Enum.Parse(typeof(AnimationSequenceAction.Action), line[1]);
            
            if (line.Length > 2)
                seq.param = line[2];

            sequenceActions.Add(seq);
        }

        initialized = true;
    }


    public override void SequenceStart()
    {
        if (!initialized)
            Debug.LogError("Sequence has not been initialized!");

        active = true;
    }


    public override IEnumerator SequenceLoop()
    {
        while (active)
        {
            currentFrame++;

            // Process animation events
            foreach (AnimationSequenceAction action in sequenceActions)
            {
                if (action.frame > currentFrame)
                    break;
                else
                    CallSequenceFunction(action.action, action.param);
            }

            yield return null;
        }

        SequenceEnd();
    }


    public override void SequenceEnd()
    {
        Debug.Log("End of Animation");
    }



    #region Animation Events

    private void CallSequenceFunction(AnimationSequenceAction.Action a, string param)
    {
        switch (a)
        {
            case AnimationSequenceAction.Action.ChangeUserAnimation:
                ChangeUserAnimation(param);
                break;
            case AnimationSequenceAction.Action.TerminateAnimation:
                active = false;
                break;
        }
    }


    private void ChangeAnimation(string t) { Debug.Log(t); }


    private void ChangeUserAnimation(string t) { ChangeAnimation(t); }

    #endregion
}
