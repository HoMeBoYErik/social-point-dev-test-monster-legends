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
    public class GameStartCommand : Command
    {
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject contextView { get; set; }

        [Inject(ContextKeys.CONTEXT)]
        public IContext context { get; set; }

        [Inject]
        public GameLateBindingSignal lateBindingSignal { get; set; }     

        override public void Execute()
        {
            Retain();

            // If we are not first context something is wrong
            if (context != Context.firstContext)
            {
               
            }      

            // Instantiate game view through prefab
            GameObject GameVO = UnityEngine.Object.Instantiate(Resources.Load("GameContext/GameVO")) as GameObject;

            // Dispatch Late Binding Signal for GameObject that
            // need to wait first frame to init theirs values
            lateBindingSignal.Dispatch();

            Release();

        }   
    }
}

