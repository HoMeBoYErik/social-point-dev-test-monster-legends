using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;
using Newtonsoft.Json;
using UniRx;
using SocialPoint.Signals;

namespace SocialPoint.Commands
{
    public class SpeedUpBreedingRequestCommand : Command
    {
        [Inject]
        public BreedingStatusModel breedingStatus { get; set; }
        [Inject]
        public NewSpeedUpBreedingRequestSignal newSpeedUpBreedingRequestSignal { get; set; }

        override public void Execute()
        {
            Retain();
            // simply forward this request to subscribers
            newSpeedUpBreedingRequestSignal.Dispatch(breedingStatus);
            Release();
        }
        
    }
}
