using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimationSequence", menuName = "Animation/Animation Sequence Object", order = 1)]
public class AnimationSequenceObject : ScriptableObject
{
    public string animationName = "";
    public TextAsset animationSequence;
}
