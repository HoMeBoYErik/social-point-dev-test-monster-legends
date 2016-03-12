using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using UniRx;

namespace SocialPoint.Signals
{
    #region Game Context Signals
    public class GameStartSignal : Signal { }
    public class GameLateBindingSignal : Signal { }

    // FloatReactiveProperty is to notify load progress
    public class LoadGameDataSignal : Signal<FloatReactiveProperty> { }
    public class LoadCompleteSignal : Signal { }
    #endregion

}

