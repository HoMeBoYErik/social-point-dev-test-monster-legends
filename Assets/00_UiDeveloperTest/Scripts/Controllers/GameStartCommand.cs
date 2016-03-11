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

        [Inject]
        public ILocalizationService localizationService { get; set; }

        [Inject]
        public GameDataService gameDataService { get; set; }


        override public void Execute()
        {
            Retain();

            // If we are not first context something is wrong
            if (context != Context.firstContext)
            {
               
            }


            string lang = localizationService.LoadUserLanguage();
            gameDataService.LoadData(lang)
               .Subscribe(
                   x => OnDataReady(x),    // on success
                   ex => OnDataError(ex),  // on error
                   OnDataComplete          // on complete
               );


            // Instantiate game view through prefab
            GameObject GameVO = UnityEngine.Object.Instantiate(Resources.Load("GameContext/GameVO")) as GameObject;

            // Dispatch Late Binding Signal for GameObject that
            // need to wait first frame to init theirs values
            lateBindingSignal.Dispatch();

        }


        private void OnDataReady(string x)
        {
            // Pass data to persistency models
            RawGameDataModel data = JsonConvert.DeserializeObject<RawGameDataModel>(x);
            // Load strings for localization dictionary
            localizationService.LoadDictionary(data.strings);
            // Load dictionary of elements images url
            gameDataService.LoadElements(data.elements);
            // Load monsters data
            gameDataService.LoadMonsters(data.monsters);
            // nullify local reference
            data = null;
            //Debug.Log("[StartupSequenceController] : Parsing Game Data Completed");
        }

        void OnDataError(Exception ex)
        {
#if DEBUG
            Debug.LogException(ex);
#endif
        }

        void OnDataComplete()
        {
#if DEBUG
            //Debug.Log("[StartupSequenceController] : Load Game Data Completed");
            //Debug.Log("[StartupSequenceController] : Starting Download of image assets");
#endif
            // Start download images sequence...
            //var progressReport = new Progress<float>(gameDataService.progressReport);
            //gameDataService.DownloadImages(progressReport);

            Release();

        }


       
    }
}

