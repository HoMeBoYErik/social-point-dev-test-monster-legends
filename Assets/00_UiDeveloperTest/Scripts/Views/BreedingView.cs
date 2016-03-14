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

public class BreedingView : View
{

    //Reference to view elements
    public RectTransform right_panel;
    public RectTransform left_panel;
    public Text header_title_text;
    public Button speedup_button;
    public Text speedup_button_text;
    public Text breeding_time_remaining_text;
    public RawImage right_monster_image;
    public Text right_monster_name_text;
    public RawImage left_monster_image;
    public Text left_monster_name_text;
    public CanvasGroup canvas;
    public AudioSource breeding_complete_sound;

    public Text left_monster_description;
    public Text right_monster_description;
    public Tweener right_monster_tweener;
    public Tweener left_monster_tweener;
    public Button right_info_button;
    public Button left_info_button;


    // FX and Particles
    public GameObject BreedingViewFX;
    public GameObject gem_aura_fx;
    public GameObject electro_fx;
    public GameObject fire_lion_fx_right;
    public GameObject fire_lion_fx_left;
    public GameObject loving_fx_right;
    public GameObject loving_fx_left;
    public GameObject pandalf_fx_right;
    public GameObject pandalf_fx_left;
    public GameObject atlantis_fx_right;
    public GameObject atlantis_fx_left;

    //Buttons delegates
    public delegate void SpeedUpClick();
    public SpeedUpClick OnSpeedUpClick;

    public delegate void LeftMonsterDescriptionClick();
    public LeftMonsterDescriptionClick OnLeftMonsterDescriptionClick;

    public delegate void RightMonsterDescriptionClick();
    public RightMonsterDescriptionClick OnRightMonsterDescriptionClick;
  

 
    // On Awake
    protected override void Awake()
    {
        base.Awake();

        right_panel = this.transform.FindChild("horizontal_fit/right_breeding_panel").GetComponent<RectTransform>();
        left_panel = this.transform.FindChild("horizontal_fit/left_breeding_panel").GetComponent<RectTransform>();
        header_title_text = transform.FindChild("vertical_fit/header/header_title_text").GetComponent<Text>();
        speedup_button = transform.FindChild("vertical_fit/speedup_button").GetComponent<Button>();
        speedup_button_text = transform.FindChild("vertical_fit/speedup_button/speedup_button_text").GetComponent<Text>();
        breeding_time_remaining_text = transform.FindChild("vertical_fit/breeding_time_remaining/breeding_time_remaining_text").GetComponent<Text>();
        right_monster_image = transform.FindChild("horizontal_fit/right_breeding_panel/right_image_frame/right_monster_image").GetComponent<RawImage>();
        right_monster_name_text = transform.FindChild("horizontal_fit/right_breeding_panel/right_image_frame/right_monster_name_text").GetComponent<Text>();
        left_monster_image = transform.FindChild("horizontal_fit/left_breeding_panel/left_image_frame/left_monster_image").GetComponent<RawImage>();
        left_monster_name_text = transform.FindChild("horizontal_fit/left_breeding_panel/left_image_frame/left_monster_name_text").GetComponent<Text>();
        canvas = this.transform.GetComponent<CanvasGroup>();
        breeding_complete_sound = this.GetComponent<AudioSource>();


        // Get reference of monster description field, animations and tweener
        left_monster_tweener = left_panel.transform.FindChild("left_image_frame/left_monster_description").GetComponent<Tweener>();
        right_monster_tweener = right_panel.transform.FindChild("right_image_frame/right_monster_description").GetComponent<Tweener>();
        left_monster_description = left_monster_tweener.transform.FindChild("left_monster_description_text").GetComponent<Text>();
        right_monster_description = right_monster_tweener.transform.FindChild("right_monster_description_text").GetComponent<Text>();

        right_info_button = right_panel.transform.FindChild("right_image_frame/right_info_button").GetComponent<Button>();
        left_info_button = left_panel.transform.FindChild("left_image_frame/left_info_button").GetComponent<Button>();


        // Get reference of particles effects
        BreedingViewFX = GameObject.Find("BreedingViewFX");
        gem_aura_fx = BreedingViewFX.transform.FindChild("gem_aura_fx").gameObject;
        electro_fx = BreedingViewFX.transform.FindChild("electro_fx").gameObject;
        fire_lion_fx_right = BreedingViewFX.transform.FindChild("fire_lion_fx_right").gameObject;
        fire_lion_fx_left = BreedingViewFX.transform.FindChild("fire_lion_fx_left").gameObject;
        loving_fx_right = BreedingViewFX.transform.FindChild("loving_fx_right").gameObject;
        loving_fx_left = BreedingViewFX.transform.FindChild("loving_fx_left").gameObject;
        pandalf_fx_right = BreedingViewFX.transform.FindChild("pandalf_fx_right").gameObject;
        pandalf_fx_left = BreedingViewFX.transform.FindChild("pandalf_fx_left").gameObject;
        atlantis_fx_right = BreedingViewFX.transform.FindChild("atlantis_fx_right").gameObject;
        atlantis_fx_left = BreedingViewFX.transform.FindChild("atlantis_fx_left").gameObject;


        // Determine screen aspect ratio to select row element prefab
        float aspectRatio = (float)Screen.width / (float)Screen.height;


        // Changed Grid Layout cell based on aspect ratio
        // 4:3 or squared ( like iPad wide )
        if (aspectRatio <= 1.34f)
        {
            right_panel.sizeDelta = new Vector2(right_panel.sizeDelta.x, 400f);
            left_panel.sizeDelta = new Vector2(left_panel.sizeDelta.x, 400f);
        }
        // iPhone 4 or normal wide
        else if (aspectRatio <= 1.52f)
        {
            
        }
        else
        {
            right_panel.offsetMax = new Vector2(-90f, right_panel.offsetMax.y);
            left_panel.offsetMin = new Vector2(90f, left_panel.offsetMin.y);
            //right_list_root.GetComponent<GridLayoutGroup>().cellSize = new Vector2(450f, 90f);
            //left_list_root.GetComponent<GridLayoutGroup>().cellSize = new Vector2(450f, 90f);
        }



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
        speedup_button.onClick.AddListener( () =>
        {
            OnSpeedUpClick();
        });

        left_info_button.onClick.AddListener(() =>
            {
                OnLeftMonsterDescriptionClick();
            });
        right_info_button.onClick.AddListener(() =>
        {
            OnRightMonsterDescriptionClick();
        });
    }

    internal void init()
    {
        header_title_text.text = "BREEDING";
    }

    #region VIEW ANIMATIONS CONTROLS
    public void HideView()
    {
        BreedingViewFX.SetActive(false);
        breeding_complete_sound.Play();

        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        TweenFactory.Tween("FadeOutBreedingView", 1.0f, 0.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    public void ShowView()
    {
        BreedingViewFX.SetActive(true);
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        TweenFactory.Tween("FadeInBreedingView", 0.0f, 1.0f, 0.5f, TweenScaleFunctions.CubicEaseOut,
            (t) => { this.canvas.alpha = t.CurrentValue; }, (t) => { this.canvas.alpha = t.CurrentValue; });
    }
    #endregion
}
