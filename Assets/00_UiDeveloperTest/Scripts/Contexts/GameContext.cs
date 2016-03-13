using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.dispatcher.eventdispatcher.impl;
using strange.extensions.command.api;
using strange.extensions.command.impl;
using UniRx;
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

            // Inject here prefabs to instances
        }

        // The First Context for the game starts here with the start signal 
        override public IContext Start()
        {
            base.Start();

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

            // Binds signals with commands (business logic)
            commandBinder.Bind<LoadGameDataSignal>().To<LoadGameDataCommand>();
            commandBinder.Bind<StartBreedingSignal>().To<StartBreedingCommand>();
            commandBinder.Bind<SpeedUpBreedingRequestSignal>().To<SpeedUpBreedingRequestCommand>();
            commandBinder.Bind<BreedingEndedSignal>().To<BreedingEndedCommand>();
            commandBinder.Bind<SpendGemsSignal>().To<SpendGemsCommand>();
            // Class bindings to values
            // Instantiate any prefab (Monobehaviours) that we want to inject into other objects
            // And save reference
            // TODO switch to singleton on final
            GameDataService gameDataService = (this.contextView as GameObject).GetComponent<GameDataService>();
            injectionBinder.Bind<GameDataService>().ToValue(gameDataService);

            LocalizationService localizationService = (this.contextView as GameObject).GetComponent<LocalizationService>();
            injectionBinder.Bind<ILocalizationService>().ToValue(localizationService);


            // Map gameobjects factories
            GameRoot root = (this.contextView as GameObject).GetComponent<GameRoot>();
            // TODO switch screen resolution to determine which prefab
            injectionBinder.Bind<GameObject>().ToValue(root.monster_row_prefab).ToName("MonsterRowPrefab");
            injectionBinder.Bind<MonsterRowFactory>().ToSingleton();

            // Map views with their mediators/presenters
            mediationBinder.Bind<GameView>().To<GameViewMediator>();
            mediationBinder.Bind<BreedingView>().To<BreedingViewMediator>();
            mediationBinder.Bind<LanguageView>().To<LanguageViewMediator>();
            mediationBinder.Bind<LoadingView>().To<LoadingViewMediator>();
            mediationBinder.Bind<MonsterPopupView>().To<MonsterPopupViewMediator>();
            mediationBinder.Bind<MonsterSelectionView>().To<MonsterSelectionViewMediator>();
            mediationBinder.Bind<SpeedUpView>().To<SpeedUpViewMediator>();

            // Bind Signals to context
            injectionBinder.Bind<LoadCompleteSignal>().ToSingleton(); // when load sequence end
            // when breeding view receive a new couple to show/process
            injectionBinder.Bind<NewBreedingCoupleSignal>().ToSingleton(); 
            // when we want to notify a new breeding request to subscribers
            injectionBinder.Bind<NewSpeedUpBreedingRequestSignal>().ToSingleton();
            // when a breeding operation ended we notify to others view from the command
            injectionBinder.Bind<BreedingEndedReceivedSignal>().ToSingleton();
            // when a user speed up the breeding process
            injectionBinder.Bind<BreedingSpeededUpSignal>().ToSingleton();

            // Bind prefabs based on configuration


        }
    }
}


