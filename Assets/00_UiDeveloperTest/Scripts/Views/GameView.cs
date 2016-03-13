using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using SocialPoint;


public class GameView : View {

    public Canvas front_ui_effects_canvas;
    public Canvas game_canvas;
    public Canvas background_canvas;


    // On Awake
    protected override void Awake()
    {
        base.Awake();
        front_ui_effects_canvas = this.transform.FindChild("FrontUIEffectsCanvas").GetComponent<Canvas>();
        front_ui_effects_canvas.worldCamera = Camera.main;
        game_canvas = this.transform.FindChild("GameCanvas").GetComponent<Canvas>();
        game_canvas.worldCamera = Camera.main;
        background_canvas = this.transform.FindChild("BackgroundCanvas").GetComponent<Canvas>();
        background_canvas.worldCamera = Camera.main;

    }

    // Reset view to default state
    internal void reset()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.mapEventListeners();
    }

    // OnDisable detact listeners and unmap delegates
    protected override void OnDisable()
    {
    }

    // Here map delegates between others monobehaviours and the view
    internal void mapEventListeners()
    { 
    }

    internal void init()
    {    
    }


}
