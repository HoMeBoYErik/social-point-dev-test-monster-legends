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
    public class BreedingEndedCommand : Command
    {
        [Inject]
        public BreedingEndedReceivedSignal breedingEndedSignal { get; set; }

        override public void Execute()
        {
            Retain();
            // We could operate on models here in a real app but...
            // maybe execute a sound
            // Just forward the signal to listeners
            breedingEndedSignal.Dispatch();
            Release();
        }

    }
}
