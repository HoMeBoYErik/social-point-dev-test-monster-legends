using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using UniRx;

namespace SocialPoint.Signals
{
    #region Game Context Signals
    public class GameStartSignal : Signal { }
    public class GameLateBindingSignal : Signal { }

    // FloatReactiveProperty is to notify load progress
    public class LoadGameDataSignal : Signal<FloatReactiveProperty> { }
    // Payload will be monsters list, elements list and images collection
    public class LoadCompleteSignal : Signal<ReactiveCollection<MonsterDataModel>,
                                              ReactiveDictionary<string, ElementDataModel>,
                                              Dictionary<string, Texture2D>> { }

    
    public class StartBreedingSignal : Signal<BreedingCoupleIdModel> { }
    public class NewBreedingCoupleSignal : Signal<BreedingCoupleDataModel> { }
    public class SpeedUpBreedingRequestSignal : Signal<BreedingStatusModel> { }
    public class NewSpeedUpBreedingRequestSignal : Signal<BreedingStatusModel> { }
    public class BreedingEndedSignal : Signal { }
    #endregion

}

