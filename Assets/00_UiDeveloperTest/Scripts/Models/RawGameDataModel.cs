using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace SocialPoint
{
    [System.Serializable]
    public class RawGameDataModel
    {
        [SerializeField]
        public Dictionary<string, string> strings;                // locales strings depending on 'lang' param
        [SerializeField]
        public Dictionary<string, ElementDataModel> elements;     // elements image thumb (fire, nature, air, etc...
        [SerializeField]
        //public MonsterDataModel[] monsters;                       // monsters list
        public ReactiveCollection<MonsterDataModel> monsters;

    }
}