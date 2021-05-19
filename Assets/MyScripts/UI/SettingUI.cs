﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    private Slider m_BGMSlider;
    private Button m_BGMButton;
    //private Slider m_EffectSlider;
    //private Button m_EffectButton;

    private SoundManager m_SoundManager;

    // Start is called before the first frame update
    void Start()
    {
        m_BGMSlider = transform.GetChild(0).Find("BGMSlider").GetComponent<Slider>();
        m_BGMButton = transform.GetChild(0).Find("Button").GetComponent<Button>();

        //m_EffectSlider = transform.GetChild(1).Find("EffectSlider").GetComponent<Slider>();
        //m_EffectButton = transform.GetChild(1).Find("Button").GetComponent<Button>();

        m_SoundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        m_SoundManager.SetBGMVolume(m_BGMSlider.value);
        //m_SoundManager.SetEffectVolume(m_EffectSlider.value);
    }

    public void ClickBGMButton()
    {
        m_SoundManager.SetBGMMute(m_BGMButton);
    }

    //public void ClickEffectButton()
    //{
    //    m_SoundManager.SetEffectMute(m_EffectButton);
    //}

    public void GoToTitleScene()
    {
        LoadingSceneManager.LoadScene("TitleScene");
    }
}