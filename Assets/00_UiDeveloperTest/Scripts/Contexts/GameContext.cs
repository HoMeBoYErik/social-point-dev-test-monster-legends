using UnityEngine;
using System.Collections;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using SocialPoint.Commands;

using SocialPoint.Signals;

namespace SocialPoint
{
    public class GameContext : MVCSContext
    {
        public GameContext (MonoBehaviour view) : base (view)
        {
        }

        public GameContext(MonoBehaviour view, ContextStartupFlags flags)
            : base(view, flags)
		{
		}

        // Unbind the default EventCommandBinder and rebind the SignalCommandBinder
        protected override void addCoreComponents()
        {
            base.addCoreComponents();
            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        }

        // The First Context for the game starts here with the start signal 
        override public IContext Start()
        {
            base.Start();

            // Instantiate any prefab (Monobehaviours) that we want to inject into other objects
            // And save reference


            // Fire here any startup signal to bootstrap the context
            GameStartSignal startSignal = (GameStartSignal)injectionBinder.GetInstance<GameStartSignal>();
            startSignal.Dispatch();
            return this;
        }

        // here we map all the bindings for DI
        protected override void mapBindings()
        {
            // Startup sequence for this context
            commandBinder.Bind<GameStartSignal>()
                .To<GameStartCommand>()
                .Once();

            // The late binding signal is dispatched from GameStartCommand 
            // as soon as he finish the startup sequence
            commandBinder.Bind<GameLateBindingSignal>()
                .To<GameLateBindingCommand>()
                .Once();


            // Map views with their mediators
            mediationBinder.Bind<GameView>().To<GameViewMediator>();
        }
    }
}


