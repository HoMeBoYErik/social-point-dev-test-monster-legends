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
    public AudioSource popup_close_sound;
    public AudioSource popup_open_sound;

    public CanvasGroup canvas;

    //Buttons delegates
    public delegate void SpendGemsClick();
    public SpendGemsClick OnSpendGemsClick;
    public delegate void ClosePopupClick();
    public ClosePopupClick OnClosePopupClick;


    // FX and Particles
    public GameObject SpeedUpPopupViewFX;
    public GameObject gem_aura_fx;
    public GameObject hearth_spark;



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
        popup_close_sound = popup_close_button.gameObject.GetComponent<AudioSource>();
        canvas = this.transform.GetComponent<CanvasGroup>();
        popup_open_sound = this.GetComponent<AudioSource>();

        SpeedUpPopupViewFX = GameObject.Find("SpeedUpPopupViewFX");
        gem_aura_fx = SpeedUpPopupViewFX.transform.FindChild("gem_aura").gameObject;
        hearth_spark = SpeedUpPopupViewFX.transform.FindChild("hearth_spark").gameObject;
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
        popup_close_button.onClick.AddListener(() =>
            {
                popup_close_sound.Play();
            });
    }

    internal void init()
    {
    }

    #region VIEW ANIMATIONS CONTROLS
    public void HideView()
    {
        SpeedUpPopupViewFX.SetActive(false);
        gem_aura_fx.SetActive(false);
        hearth_spark.SetActive(false);

        if( this.canvas.alpha > 0)
        {
            canvas.interactable = false;
            canvas.blocksRaycasts = false;

            TweenFactory.Tween("FadeOutSpeedUpView", 1.0f, 0.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
                (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
        }
       
    }
    public void ShowView()
    {
        SpeedUpPopupViewFX.SetActive(true);
        gem_aura_fx.SetActive(true);
        hearth_spark.SetActive(true);

        if (this.canvas.alpha < 1f)
        {
            popup_open_sound.Play();

            canvas.interactable = true;
            canvas.blocksRaycasts = true;

            TweenFactory.Tween("FadeInSpeedUpView", 0.0f, 1.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
                (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
        }
    }
    #endregion
}
