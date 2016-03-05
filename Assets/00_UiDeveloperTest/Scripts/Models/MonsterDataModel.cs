using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SocialPoint
{
    [Serializable]
    public class MonsterDataModel
    {
        [SerializeField]
        public string type;
        [SerializeField]
        public string name;
        [SerializeField]
        public string thumb_img;
        [SerializeField]
        public string full_img;
        [SerializeField]
        public string description;
        [SerializeField]
        public string[] elements;
        [SerializeField]
        public int level;
    }
}

