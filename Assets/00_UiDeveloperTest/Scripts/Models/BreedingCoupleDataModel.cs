using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SocialPoint
{
    [Serializable]
    public class BreedingCoupleDataModel
    {
        [SerializeField]
        public int leftMonsterId;
        [SerializeField]
        public string leftMonsterName;
        [SerializeField]
        public Texture2D leftMonsterTex;
        [SerializeField]
        public int leftMonsterLevel;
        [SerializeField]
        public string leftMonsterDescription;
        [SerializeField]
        public string leftMonsterType;

        [SerializeField]
        public int rightMonsterId;
        [SerializeField]
        public string rightMonsterName;
        [SerializeField]
        public Texture2D rightMonsterTex;
        [SerializeField]
        public int rightMonsterLevel;
        [SerializeField]
        public string rightMonsterDescription;
        [SerializeField]
        public string rightMonsterType;


        public BreedingCoupleDataModel()
        {
            
        }
    }
}
