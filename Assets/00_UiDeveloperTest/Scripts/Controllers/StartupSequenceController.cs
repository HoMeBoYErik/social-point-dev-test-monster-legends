using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UniRx;


namespace SocialPoint
{
    // Candidate to be start command in IOC arch
    public class StartupSequenceController : MonoBehaviour
    {
        public GameDataService gameDataService;
        public LocalizationService localizationService;
        [SerializeField]
        private RawGameDataModel data;

        // Startup command
        void Start()
        {
            gameDataService.LoadData(localizationService.Language)
                .Subscribe(
                    x => OnDataReady(x),    // on success
                    ex => OnDataError(ex),  // on error
                    OnDataComplete          // on complete
                );
        }

        void OnDataReady(string x)
        { 
            // Pass data to persistency models
            data = JsonConvert.DeserializeObject<RawGameDataModel>(x);
            // Load strings for localization dictionary
            localizationService.LoadDictionary(data.strings);
            // Load dictionary of elements images url
            gameDataService.LoadElements(data.elements);
            // Load monsters data
            gameDataService.LoadMonsters(data.monsters);
            // nullify local reference
            data = null;
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
            Debug.Log("[StartupSequenceController] : Load Game Data Completed");
#endif
            // Start download images sequence...
        }
    }
}

