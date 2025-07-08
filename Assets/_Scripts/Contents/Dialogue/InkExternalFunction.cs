using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkExternalFunction
{
    public void Bind(Story story, Animator anim)
    {
        story.BindExternalFunction("playAnim", (string animName) =>
            PlayAnim(animName, anim));
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("playAnim");
    }

    public void PlayAnim(string animName, Animator anim)
    {
        Debug.Assert(anim != null, "anim is null!");

        if (anim != null)
        {
            anim.Play(animName);
        }

    }

}
