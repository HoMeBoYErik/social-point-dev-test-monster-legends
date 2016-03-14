using System;
using System.Collections;
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
    public class LoadGameDataCommand : Command
    {
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject contextView { get; set; }

        [Inject(ContextKeys.CONTEXT)]
        public IContext context { get; set; }

        [Inject]
        public ILocalizationService localizationService { get; set; }

        [Inject]
        public GameDataService gameDataService { get; set; }

        [Inject]
        public FloatReactiveProperty progressReport {  get;  set; }

        [Inject]
        public LoadCompleteSignal loadCompleteSignal { get; set; }

        override public void Execute()
        {
            Retain();

            // Move this to a service method
            string lang = localizationService.LoadUserLanguage();
            //lang = "ru";
            //lang = "en";
            //lang = "es";
            //lang = "fr";
            gameDataService.LoadData(lang)
               .Subscribe(
                   x => OnDataReady(x),    // on success
                   ex => OnDataError(ex),  // on error
                   OnDataComplete          // on complete
               );
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
            // Start download images sequence...
            // We can subscribe to Observable variable of gameDataService
            gameDataService.CurrentLoadProgress
                .TakeWhile(p => p < 1.0f) //stop when global progress reach 1
                .Subscribe(x =>
                {
                    // Update progress
                    progressReport.Value = x;                    
                    
                },
                ex => { Debug.LogException(ex); },//Fail();// Release(); },
                () =>
                {
                    progressReport.Value = 1.0f;
#if DEBUG
                    //Debug.Log("Image Download Completed");
#endif

                    // Wait three more seconds to appreciate the view
                    //contextView.GetComponent<GameRoot>().StartCoroutine(WaitBeforeLoadComplete());

                    // Notify that all data are loaded and ready
                   loadCompleteSignal.Dispatch(gameDataService.monsters,
                                                gameDataService.elements,
                                                gameDataService.imageCache);
                    // Release the command
                    Release();
                }
               );

            // While downloading images the Current Load Progress is 
            // automatically updated
            // Download Images will trigger updates on current load progress
            gameDataService.DownloadImages();
            
        }

        //...this is used only if we use the IProgress interface as argument
        public void progressCallBack(float progress)
        {
            progressReport.Value = progress;
        }

        IEnumerator WaitBeforeLoadComplete()
        {
            yield return new WaitForSeconds(5.0f);
            // Notify that all data are loaded and ready
            loadCompleteSignal.Dispatch(gameDataService.monsters,
                                        gameDataService.elements,
                                        gameDataService.imageCache);
            // Release the command
            Release();
        }

        
    }
}
