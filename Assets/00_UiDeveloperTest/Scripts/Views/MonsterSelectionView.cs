using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRuby.Tween;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using SocialPoint;

public class MonsterSelectionView : View {

    // On Awake
    protected override void Awake()
    {
        base.Awake();
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
