﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using SocialPoint;


public class GameViewMediator : Mediator
{
    // Injecting the view
    [Inject]
    public GameView view { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        // Init view
        view.init();
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }
}
