using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    TextMeshProUGUI levelText;
    Image healthSlider;
    Image expSlider;
    private void Awake()
    {
        levelText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        healthSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(2).GetChild(0).GetComponent<Image>();
    }
    void Start()
    {

    }
    void Update()
    {
        levelText.text = "LV." + mGame.Inst.playerStats.universalData.currentLevel.ToString("00");
        UpdateHealth();
        UpdateExp();
    }
    void UpdateHealth()
    {
        float sliderPercent = (float)mGame.Inst.playerStats.currentHP / mGame.Inst.playerStats.maxHP;
        healthSlider.fillAmount = sliderPercent;
    }

    void UpdateExp()
    {
        float sliderPercent = (float)mGame.Inst.playerStats.universalData.currentExp / mGame.Inst.playerStats.universalData.baseExp;
        expSlider.fillAmount = sliderPercent;
    }
}
