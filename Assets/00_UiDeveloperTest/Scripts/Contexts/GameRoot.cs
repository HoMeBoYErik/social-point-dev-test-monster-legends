using UnityEngine;
using System.Collections;
using strange.extensions.context.impl;

namespace SocialPoint
{
    public class GameRoot : ContextView
    {

        // Config section for this context
        public GameObject monster_row_prefab;
        public GameObject monster_row_wide_prefab;
        public GameObject monster_row_squared_prefab;


        void Awake()
        {
            context = new GameContext(this);
        }       
    }
}

