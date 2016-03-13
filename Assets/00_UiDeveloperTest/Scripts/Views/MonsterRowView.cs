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

public class MonsterRowView : View
{
    public enum TableSide { LEFT, RIGHT }

    public int id;
    public TableSide tableSide;
    public Image monster_thumb;
    public RawImage monster_thumb_image;
    public Text monster_level_text;
    public Text monster_name_text;
    public Text monster_type_text;
    public Image monster_box;

    public RawImage[] monster_elements = new RawImage[3];   

    public Transform monster_selector;
    public Button monster_selector_button;
    public AudioSource monster_click_sound;

    public delegate void MonsterClick(int id, TableSide tableSide);
    public MonsterClick OnMonsterClick;

    //Extra config for state change
    // Font face for Monster Name
    public Font MonsterName_FontFace_Normal;
    public int MonsterName_FontSize_Normal = 28;
    public Font MonsterName_FontFace_Selected;
    public int MonsterName_FontSize_Selected = 30;
    public Material MonsterName_Material_Normal;
    public Material MonsterName_Material_Selected;

    // Font color for Monster Name
    public Color MonsterName_FontColor_Normal;
    public Color MonsterName_FontColor_Selected;
    // Font color for Monster Type
    public Color MonsterType_FontColor_Normal;
    public Color MonsterType_FontColor_Selected;
    // Box Background
    public Color Box_Color_Normal;
    public Color Box_Color_Selected;


	// On Awake map view elements
    protected override void Awake()
    {
        id = -1; // not initialized
        monster_thumb = transform.FindChild("monster_thumb").GetComponent<Image>();
        monster_thumb_image = transform.FindChild("monster_thumb/monster_thumb_image").GetComponent<RawImage>();
        monster_level_text = transform.FindChild("monster_box/monster_star/monster_level_text").GetComponent<Text>();
        monster_name_text = transform.FindChild("monster_box/monster_name_text").GetComponent<Text>();
        monster_type_text = transform.FindChild("monster_box/monster_type_text").GetComponent<Text>();
        monster_box = transform.FindChild("monster_box").GetComponent<Image>();
        monster_elements[0] = transform.FindChild("monster_box/monster_elements/monster_element_0").GetComponent<RawImage>();
        monster_elements[0].enabled = false;
        monster_elements[1] = transform.FindChild("monster_box/monster_elements/monster_element_1").GetComponent<RawImage>();
        monster_elements[1].enabled = false;
        monster_elements[2] = transform.FindChild("monster_box/monster_elements/monster_element_2").GetComponent<RawImage>();
        monster_elements[2].enabled = false;
        
        monster_selector = transform.FindChild("monster_box/monster_selector").transform;
        monster_selector_button = transform.FindChild("monster_box").GetComponent<Button>();
        monster_click_sound = monster_selector_button.gameObject.GetComponent<AudioSource>();

       
    
    }

    // Reset view to default state
    internal void Reset()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.mapEventListeners();
    }

    // OnDisable detach listeners and unmap delegates
    protected override void OnDisable()
    {
    }

    // Here map delegates between others monobehaviours and the view
    internal void mapEventListeners()
    {
        monster_selector_button.onClick.AddListener(() =>
        {
            OnMonsterClick(this.id, this.tableSide);
        });

        monster_selector_button.onClick.AddListener(() =>
            {
                monster_click_sound.Play();
            });
    }



    internal void init(int id, TableSide tableSide)
    {
        this.id = id;
        this.tableSide = tableSide;
    }

    public void SelectView()
    {
        monster_selector.GetComponent<Image>().enabled = true;
        monster_box.color = Box_Color_Selected;
        monster_name_text.font = MonsterName_FontFace_Selected;
        monster_name_text.color = MonsterName_FontColor_Selected;
        monster_type_text.color = MonsterType_FontColor_Selected;
        monster_name_text.fontSize = MonsterName_FontSize_Selected;
    }

    public void DeselectView()
    {
        monster_selector.GetComponent<Image>().enabled = false;
        monster_box.color = Box_Color_Normal;
        monster_name_text.font = MonsterName_FontFace_Normal;
        monster_name_text.color = MonsterName_FontColor_Normal;
        monster_type_text.color = MonsterType_FontColor_Normal;
        monster_name_text.fontSize = MonsterName_FontSize_Normal;
    }

    public void DeactivateView()
    {
        monster_thumb.color = Color.gray;
        monster_box.color = Color.gray;
        monster_thumb_image.color = Color.gray;
        monster_selector_button.interactable = false;
    }

    public void ActivateView()
    {
        monster_thumb.color = Color.white;
        monster_box.color = Color.white;
        monster_thumb_image.color = Color.white;
        monster_selector_button.interactable = true;
    }


}
