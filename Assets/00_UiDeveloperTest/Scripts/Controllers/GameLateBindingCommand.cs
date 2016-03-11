using System;
using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.impl;

using SocialPoint.Signals;

namespace SocialPoint.Commands
{
    public class GameLateBindingCommand : Command
    {
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject contextView { get; set; }

        [Inject(ContextKeys.CONTEXT)]
        public IContext context { get; set; }

        

        override public void Execute()
        {
            // If we are not first context something is wrong
            if (context != Context.firstContext)
            {

            }

            

        }

        private void onSuccess()
        { }

        private void onFailure(object payload)
        { }
    }
}

