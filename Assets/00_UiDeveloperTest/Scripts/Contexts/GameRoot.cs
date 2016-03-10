using UnityEngine;
using System.Collections;
using strange.extensions.context.impl;

namespace SocialPoint
{
    public class GameRoot : ContextView
    {
        void Awake()
        {
            context = new GameContext(this);
        }       
    }
}

