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

public class SpeedUpView : View{

    // References to view elements
    public Text popup_title_text;
    public Text heart_root_text;
    public Text popup_text;
    public Button speedup_button;
    public Text speedup_button_text;
    public Button popup_close_button;

    public CanvasGroup canvas;

    //Buttons delegates
    public delegate void SpendGemsClick();
    public SpendGemsClick OnSpendGemsClick;
    public delegate void ClosePopupClick();
    public ClosePopupClick OnClosePopupClick;



    // On Awake
    protected override void Awake()
    {
        base.Awake();

        popup_title_text = transform.FindChild("popup_container/popup_title/popup_title_text").GetComponent<Text>();
        heart_root_text = transform.FindChild("popup_container/hearth_root/hearth_root_text").GetComponent<Text>();
        popup_text = transform.FindChild("popup_container/popup_text").GetComponent<Text>();
        speedup_button = transform.FindChild("popup_container/popup_speedup_button").GetComponent<Button>();
        speedup_button_text = transform.FindChild("popup_container/popup_speedup_button/speedup_button_text").GetComponent<Text>();
        popup_close_button = transform.FindChild("popup_container/popup_button_close").GetComponent<Button>();
        canvas = this.transform.GetComponent<CanvasGroup>();
    }

    // Reset view to default state
    internal void Reset()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MapEventListeners();
    }

    // OnDisable detact listeners and unmap delegates
    protected override void OnDisable()
    {
    }

    // Here map delegates between others monobehaviours and the view
    internal void MapEventListeners()
    {
        speedup_button.onClick.AddListener(() =>
            {
                OnSpendGemsClick();
            });

        popup_close_button.onClick.AddListener(() =>
            {
                OnClosePopupClick();
            });
    }

    internal void init()
    {
    }

    #region VIEW ANIMATIONS CONTROLS
    public void HideView()
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        TweenFactory.Tween("FadeOutCanvas", 1.0f, 0.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    public void ShowView()
    {
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        TweenFactory.Tween("FadeInCanvas", 0.0f, 1.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    #endregion
}
