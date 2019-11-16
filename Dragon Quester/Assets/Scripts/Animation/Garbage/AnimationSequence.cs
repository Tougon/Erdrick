using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSequence : Sequence
{

    public class AnimationSequenceAction
    {
        public enum Action { ChangeUserAnimation, ChangeTargetAnimation, TerminateAnimation, GenerateEffect, TerminateEffect }

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

        string[] sequence = obj.animationSequence.text.Split('\n');

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
            while (sequenceActions.Count > 0 && !(sequenceActions[0].frame > currentFrame))
            {
                CallSequenceFunction(sequenceActions[0].action, sequenceActions[0].param);
                sequenceActions.RemoveAt(0);
            }

            yield return null;
        }

        SequenceEnd();
    }


    public override void SequenceEnd()
    {
        Debug.Log("End of Animation");

        while(effects.Count > 0)
            effects.RemoveAt(0);
    }



    #region Animation Events

    private void CallSequenceFunction(AnimationSequenceAction.Action a, string param)
    {
        switch (a)
        {
            case AnimationSequenceAction.Action.ChangeUserAnimation:
                ChangeUserAnimation(param);
                break;
            case AnimationSequenceAction.Action.GenerateEffect:
                string[] vals = param.Split(',');

                if (vals.Length != 5)
                    Debug.LogError("Invalid param count for Effect Generation!");

                GenerateEffect(vals[0], vals[1], float.Parse(vals[2].Trim()), float.Parse(vals[3].Trim()), float.Parse(vals[4].Trim()));
                break;
            case AnimationSequenceAction.Action.TerminateEffect:
                int id = int.Parse(param);
                TerminateEffect(id);
                break;
            case AnimationSequenceAction.Action.TerminateAnimation:
                active = false;
                break;
        }
    }

    private void ChangeUserAnimation(string t) { user.SetAnimation(t.Trim()); }
    private void ChangeTargetAnimation(string t) { target.SetAnimation(t.Trim()); }
    
    private void GenerateEffect(string path, string relative, float x, float y, float z)
    {
        path = path.Trim();
        relative = relative.Trim();

        if(relative == "User")
        {
            Vector3 rel = user.transform.position;

            x += rel.x;
            y += rel.y;
            z += rel.z;
        }
        else if(relative == "Target")
        {
            Vector3 rel = target.transform.position;

            x += rel.x;
            y += rel.y;
            z += rel.z;
        }
        Debug.Log(path);
        GameObject effect = GameObject.Instantiate(Resources.Load(path, typeof(GameObject))) as GameObject;
        effect.transform.position = new Vector3(x, y, z);
        effects.Add(effect);
    }


    private void TerminateEffect(int id)
    {
        effects[id].SetActive(false);
    }

    #endregion
}
