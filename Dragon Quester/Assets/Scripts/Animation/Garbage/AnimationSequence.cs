using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationSequence : Sequence
{

    public class AnimationSequenceAction
    {
        public enum Action { ChangeUserAnimation, ChangeTargetAnimation, TerminateAnimation, GenerateEffect, TerminateEffect,
           Move, Rotate, Scale, Color, Vibrate, ChangeAnimationSpeed, ChangeAnimationState }

        public int frame;
        public Action action;
        public string param;
    }


    private bool active = false;
    private bool initialized = false;
    private int currentFrame = 0;
    private float directionX = 1;
    private float directionY = 1;

    public string sequenceName { get; private set; }

    private Entity user;
    private Entity target;
    private Vector3 userPosition;
    private Vector3 targetPosition;
    private Vector3 userRotation;
    private Vector3 targetRotation;
    private Vector3 userScale;
    private Vector3 targetScale;
    private Color userColor;
    private Color targetColor;
    private SpriteRenderer userSprite;
    private SpriteRenderer targetSprite;

    private List<Entity> effects = new List<Entity>();

    private List<AnimationSequenceAction> sequenceActions = new List<AnimationSequenceAction>();


    public AnimationSequence(AnimationSequenceObject obj, Entity u, Entity t)
    {
        user = u;
        target = t;

        userPosition = user.transform.position;
        userRotation = user.transform.eulerAngles;
        userScale = user.transform.localScale;
        userSprite = user.GetSpriteRenderer();
        userColor = userSprite.color;
        directionX = user.transform.localScale.x / Mathf.Abs(user.transform.localScale.x);
        directionY = user.transform.localScale.y / Mathf.Abs(user.transform.localScale.y);

        if (target!= null)
        {
            targetPosition = target.transform.position;
            targetRotation = target.transform.eulerAngles;
            targetScale = target.transform.localScale;
            targetSprite = target.GetSpriteRenderer();
            targetColor = targetSprite.color;
        }

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

        user.transform.position = userPosition;
        user.transform.eulerAngles = userRotation;
        user.transform.localScale = userScale;
        userSprite.color = userColor;

        if(target != null)
        {
            target.transform.position = targetPosition;
            target.transform.eulerAngles = targetRotation;
            target.transform.localScale = targetScale;
            targetSprite.color = targetColor;
        }
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
                string[] effectVals = param.Split(',');

                if (effectVals.Length != 5)
                    Debug.LogError("Invalid param count for Effect Generation!");

                GenerateEffect(effectVals[0], effectVals[1], 
                    float.Parse(effectVals[2].Trim()), float.Parse(effectVals[3].Trim()), float.Parse(effectVals[4].Trim()));
                break;

            case AnimationSequenceAction.Action.TerminateEffect:
                int id = int.Parse(param);
                TerminateEffect(id);
                break;

            case AnimationSequenceAction.Action.Move:
                string[] moveVals = param.Split(',');

                if(moveVals.Length > 6 && moveVals.Length < 5)
                    Debug.LogError("Invalid param count for Movement!");

                Transform tM;
                string sM = moveVals[0].Trim();

                if (sM.Equals("User"))
                    tM = user.transform;
                else if (sM.Equals("Target"))
                    tM = target.transform;
                else
                    tM = effects[int.Parse(moveVals[5].Trim())].transform;

                float durationM = ((float.Parse(moveVals[1].Trim())) / 60.0f);

                float xM = (float.Parse(moveVals[2].Trim()) * directionX) + tM.position.x;
                float yM = (float.Parse(moveVals[3].Trim()) * directionY) + tM.position.y;
                float zM = float.Parse(moveVals[4].Trim()) + tM.position.z;

                TweenPosition(tM, xM, yM, zM, durationM);
                break;

            case AnimationSequenceAction.Action.Rotate:
                string[] rotateVals = param.Split(',');

                if (rotateVals.Length > 6 && rotateVals.Length < 5)
                    Debug.LogError("Invalid param count for Movement!");

                Transform tR;
                string sR = rotateVals[0].Trim();

                if (sR.Equals("User"))
                    tR = user.transform;
                else if (sR.Equals("Target"))
                    tR = target.transform;
                else
                    tR = effects[int.Parse(rotateVals[5].Trim())].transform;

                float durationR = ((float.Parse(rotateVals[1].Trim())) / 60.0f); ;

                float xR = float.Parse(rotateVals[2].Trim()) + tR.position.x;
                float yR = float.Parse(rotateVals[3].Trim()) + tR.position.y;
                float zR = float.Parse(rotateVals[4].Trim()) + tR.position.z;

                TweenRotation(tR, xR, yR, zR, durationR);
                break;

            case AnimationSequenceAction.Action.Scale:
                string[] scaleVals = param.Split(',');

                if (scaleVals.Length > 6 && scaleVals.Length < 5)
                    Debug.LogError("Invalid param count for Movement!");

                Transform tS;
                string sS = scaleVals[0].Trim();

                if (sS.Equals("User"))
                    tS = user.transform;
                else if (sS.Equals("Target"))
                    tS = target.transform;
                else
                    tS = effects[int.Parse(scaleVals[5].Trim())].transform;

                float durationS = ((float.Parse(scaleVals[1].Trim())) / 60.0f); ;

                float xS = (float.Parse(scaleVals[2].Trim()) * directionX) * Mathf.Abs(tS.localScale.x);
                float yS = (float.Parse(scaleVals[3].Trim()) * directionY) * Mathf.Abs(tS.localScale.y);
                float zS = float.Parse(scaleVals[4].Trim()) + tS.position.z;

                TweenScale(tS, xS, yS, zS, durationS);
                break;

            case AnimationSequenceAction.Action.Color:
                string[] colorVals = param.Split(',');

                if (colorVals.Length > 7 && colorVals.Length < 6)
                    Debug.LogError("Invalid param count for Color!");

                Entity eC;
                string sC = colorVals[0].Trim();

                if (sC.Equals("User"))
                    eC = user;
                else if (sC.Equals("Target"))
                    eC = target;
                else
                    eC = effects[int.Parse(colorVals[6].Trim())];

                float durationC = ((float.Parse(colorVals[1].Trim())) / 60.0f);

                float xC = float.Parse(colorVals[2].Trim());
                float yC = float.Parse(colorVals[3].Trim());
                float zC = float.Parse(colorVals[4].Trim());
                float wC = float.Parse(colorVals[5].Trim());

                TweenColor(eC, new Color(xC, yC, zC, wC), durationC);
                break;

            case AnimationSequenceAction.Action.Vibrate:
                string[] vibrateVals = param.Split(',');

                if (vibrateVals.Length > 6 && vibrateVals.Length < 5)
                    Debug.LogError("Invalid param count for Vibration!");

                Transform tV;
                string sV = vibrateVals[0].Trim();

                if (sV.Equals("User"))
                    tV = user.transform;
                else if (sV.Equals("Target"))
                    tV = target.transform;
                else
                    tV= effects[int.Parse(vibrateVals[5].Trim())].transform;

                float durationV = ((float.Parse(vibrateVals[1].Trim())) / 60.0f);
                Debug.Log(durationV);
                Vector3 strengthV = new Vector3(float.Parse(vibrateVals[2]), float.Parse(vibrateVals[3]), 0.0f);
                int vibratoV = int.Parse(vibrateVals[4]);

                Vibrate(tV, durationV, strengthV, vibratoV);
                break;

            case AnimationSequenceAction.Action.ChangeAnimationSpeed:
                string[] speedVals = param.Split(',');

                if (speedVals.Length > 3 && speedVals.Length < 2)
                    Debug.LogError("Invalid param count for Speed!");
                
                string sSp = speedVals[0].Trim();
                float sSpeed = float.Parse(speedVals[1].Trim());

                if (sSp.Equals("User"))
                    user.FrameSpeedModify(sSpeed);
                else if (sSp.Equals("Target"))
                    target.FrameSpeedModify(sSpeed);
                else
                    effects[int.Parse(speedVals[2].Trim())].FrameSpeedModify(sSpeed);
                break;

            case AnimationSequenceAction.Action.ChangeAnimationState:
                string[] stateVals = param.Split(',');

                if (stateVals.Length > 4 && stateVals.Length < 3)
                    Debug.LogError("Invalid param count for State Change!");

                Entity eAS;
                string sAS = stateVals[0].Trim();

                if (sAS.Equals("User"))
                    eAS = user;
                else if (sAS.Equals("Target"))
                    eAS = target;
                else
                    eAS = effects[int.Parse(stateVals[3].Trim())];

                eAS.SetAnimationState(stateVals[1].Trim(), bool.Parse(stateVals[2].Trim()));

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

            x = (rel.x) + (x * directionX);
            y = (rel.y) + (y * directionY);
            z += rel.z;
        }
        else if(relative == "Target")
        {
            Vector3 rel = target.transform.position;

            x = (rel.x) + (x * directionX);
            y = (rel.y) + (y * directionY);
            z += rel.z;
        }

        GameObject effect = GameObject.Instantiate(Resources.Load(path, typeof(GameObject))) as GameObject;
        effect.transform.position = new Vector3(x, y, z);
        effects.Add(effect.GetComponent<Entity>());
    }


    private void TerminateEffect(int id)
    {
        effects[id].gameObject.SetActive(false);
    }


    private void TweenPosition(Transform t, float x, float y, float z, float duration)
    {
        t.DOMove(new Vector3(x, y, z), duration);
    }


    private void TweenRotation(Transform t, float x, float y, float z, float duration)
    {
        t.DORotate(new Vector3(x, y, z), duration);
    }


    private void TweenScale(Transform t, float x, float y, float z, float duration)
    {
        t.DOScale(new Vector3(x, y, z), duration);
    }


    private void TweenColor(Entity e, Color c, float duration)
    {
        e.SetColorTween(c, duration);
    }


    private void Vibrate(Transform t, float duration, Vector3 strength, int vibrato)
    {
        t.transform.DOShakePosition(duration, strength, vibrato);
    }

    #endregion
}
