using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SocialPoint
{
    [Serializable]
    public class BreedingCoupleIdModel
    {
        [SerializeField]
        public int leftMonsterId;
        [SerializeField]
        public int rightMonsterId;

        public BreedingCoupleIdModel(int left, int right)
        {
            this.leftMonsterId = left;
            this.rightMonsterId = right;
        }
    }
}
