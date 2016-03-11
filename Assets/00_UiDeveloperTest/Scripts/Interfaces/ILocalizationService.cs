using System;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UniRx;

namespace SocialPoint
{
    public interface ILocalizationService
    {
        string Language { get; set; }

        void LoadDictionary(Dictionary<string, string> dict);
        string GetString(string key);
        string LoadUserLanguage();
        void SetUserLanguage(string lang);
        bool isValidLanguage(string lang);
        string UnescapeString(string escapedString);
        string DecodeUnicodeString(string inputString);
    }
}