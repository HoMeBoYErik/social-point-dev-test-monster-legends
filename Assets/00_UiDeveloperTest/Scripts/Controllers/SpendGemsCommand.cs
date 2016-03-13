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
    public class SpendGemsCommand : Command
    {
        [Inject]
        public int gems { get; set; }
        [Inject]
        public BreedingSpeededUpSignal breedingSpeededUpSignal { get; set; }
       
        override public void Execute()
        {
            Retain();
            // We could operate on models here in a real app but...
            // maybe notify service and persistency layer that he spent gems
            // Just forward the signal to listeners
#if DEBUG
            Debug.Log("User spent " + gems + " gems");
#endif
            breedingSpeededUpSignal.Dispatch();
            Release();
        }
    }
}
