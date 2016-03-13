using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace SocialPoint
{
    using System.Collections;

    public class BreedingStatusModel
    {
        [SerializeField]
        public IntReactiveProperty timeLeft;
        [SerializeField]
        public IntReactiveProperty gemsRequired;
       
    }
}