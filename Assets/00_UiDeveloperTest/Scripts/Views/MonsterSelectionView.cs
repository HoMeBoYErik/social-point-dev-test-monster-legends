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

    //Reference to view elements
    public RectTransform right_selection_panel;
    public RectTransform left_selection_panel;

    public Text header_title_text;
    public Button breeding_button;
    public Button language_button;
    public AudioSource start_breeding_sound;
    public Text breeding_button_text;
    public CanvasGroup canvas;
    public Transform right_list_root;
    public Transform left_list_root;
    public Tweener hearth_decoration_tweener;
    public Tweener hearth_decoration_button_tweener;

    public delegate void StartBreedingClick();
    public StartBreedingClick OnStartBreedingClick;

    public delegate void OpenLanguagePanelClick();
    public OpenLanguagePanelClick OnOpenLanguagePanelClick;


    // On Awake
    protected override void Awake()
    {
        base.Awake();

        right_selection_panel = this.transform.FindChild("horizontal_fit/right_monster_selection_panel").GetComponent<RectTransform>();
        left_selection_panel = this.transform.FindChild("horizontal_fit/left_monster_selection_panel").GetComponent<RectTransform>();

        header_title_text = this.transform
            .Find("vertical_fit/header/header_title_text")
            .GetComponent<Text>();

        breeding_button = this.transform
            .Find("vertical_fit/breeding_button").GetComponent<Button>();

        start_breeding_sound = breeding_button.gameObject.GetComponent<AudioSource>();

        breeding_button_text = breeding_button.GetComponentInChildren<Text>();

        language_button = this.transform.FindChild("vertical_fit/language_button").GetComponent<Button>();

        canvas = this.GetComponent<CanvasGroup>();

        // We use the tag here because the child is very deep down the hierarchy
        right_list_root = GameObject.FindGameObjectWithTag("RightListRoot").transform;
        left_list_root = GameObject.FindGameObjectWithTag("LeftListRoot").transform;

        hearth_decoration_tweener = this.transform.FindChild("horizontal_fit/hearth_decoration").GetComponent<Tweener>();
        hearth_decoration_button_tweener = this.transform.FindChild("vertical_fit/breeding_button_decoration_root").GetComponent<Tweener>();

        // Determine screen aspect ratio to select row element prefab
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        Debug.Log("Aspect ratio " + aspectRatio);

        
        // Changed Grid Layout cell based on aspect ratio
        // 4:3 or squared ( like iPad wide )
        if (aspectRatio <= 1.34f)
        {
            right_list_root.GetComponent<GridLayoutGroup>().cellSize = new Vector2(380f, 90f);
            left_list_root.GetComponent<GridLayoutGroup>().cellSize = new Vector2(380f, 90f);
            right_selection_panel.sizeDelta = new Vector2(right_selection_panel.sizeDelta.x, 400f);
            left_selection_panel.sizeDelta = new Vector2(left_selection_panel.sizeDelta.x, 400f);

            
        }
        // iPhone 4 or normal wide
        else if (aspectRatio <= 1.52f)
        {
            right_list_root.GetComponent<GridLayoutGroup>().cellSize = new Vector2(400f, 90f);
            left_list_root.GetComponent<GridLayoutGroup>().cellSize = new Vector2(400f, 90f);
        }
        else
        {
            right_selection_panel.offsetMax = new Vector2(-90f, right_selection_panel.offsetMax.y);
            left_selection_panel.offsetMin = new Vector2(90f, left_selection_panel.offsetMin.y);
            //right_list_root.GetComponent<GridLayoutGroup>().cellSize = new Vector2(450f, 90f);
            //left_list_root.GetComponent<GridLayoutGroup>().cellSize = new Vector2(450f, 90f);
        }
    
    
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
        breeding_button.onClick.AddListener(() =>
        {
            OnStartBreedingClick();
        });

        breeding_button.onClick.AddListener(() =>
            {
                start_breeding_sound.Play();
            });

        language_button.onClick.AddListener(() =>
            {
                OnOpenLanguagePanelClick();
            });
    }

    internal void init()
    {
        header_title_text.text = "BREEDING";
        breeding_button.interactable = false;
        breeding_button_text.text = "___";
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
