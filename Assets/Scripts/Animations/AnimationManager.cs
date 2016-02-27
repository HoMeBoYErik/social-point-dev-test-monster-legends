using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour 
{
    //opacity
    public static void FadeInOut(GameObject go, float animationTime, float opacity, float delay)
    {
        TweenAlpha.Begin(go.gameObject, animationTime, opacity).delay = delay;
    }
}