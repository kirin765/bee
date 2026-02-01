using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour
{
    [SerializeField] private GameObject windowRoot;
    [SerializeField] private Button optionAButton;
    [SerializeField] private Button optionBButton;
    [SerializeField] private TMP_Text optionAText;
    [SerializeField] private TMP_Text optionBText;
    [SerializeField] private Image optionAImage;
    [SerializeField] private Image optionBImage;

    [Header("Skill Sprites")]
    [SerializeField] private Sprite arrowDamageSprite;
    [SerializeField] private Sprite arrowCooldownSprite;
    [SerializeField] private Sprite ultimateSprite;
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite beeSlowSprite;
    [SerializeField] private Sprite delayedShotSprite;

    private Action<SkillType> onChosen;
    private SkillType optionAType;
    private SkillType optionBType;
    private float beforeTimeScale = 1f;

    private void Awake()
    {
        if (windowRoot != null) windowRoot.SetActive(false);
        if (optionAButton != null) optionAButton.onClick.AddListener(() => ChooseA());
        if (optionBButton != null) optionBButton.onClick.AddListener(() => ChooseB());
    }

    public void ShowOptions(SkillType a, SkillType b, Action<SkillType> onChosenCallback)
    {
        if (windowRoot == null) return;
        this.onChosen = onChosenCallback;
        optionAType = a;
        optionBType = b;
        windowRoot.SetActive(true);
        SetOption(optionAText, optionAImage, a);
        SetOption(optionBText, optionBImage, b);
        // Pause game while player chooses a skill
        beforeTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void SetOption(TMP_Text text, Image image, SkillType type)
    {
        if (text != null) text.text = Describe(type);
        if (image != null) image.sprite = GetSprite(type);
    }

    private string Describe(SkillType s)
    {
        switch (s)
        {
            case SkillType.ArrowDamage: return "Arrow Damage: +1";
            case SkillType.ArrowCooldown: return "Arrow Cooldown: reduce cooldown";
            case SkillType.Ultimate: return "Ultimate: +1 (max 2)";
            case SkillType.Heart: return "Heart: +1 (max 3)";
            case SkillType.BeeSlow: return "Bee Slow: reduce bee speed";
            case SkillType.DelayedShot: return "Delayed Shot: extra arrows with 0.1s delay (max 5)";
            default: return s.ToString();
        }
    }

    private Sprite GetSprite(SkillType s)
    {
        switch (s)
        {
            case SkillType.ArrowDamage: return arrowDamageSprite;
            case SkillType.ArrowCooldown: return arrowCooldownSprite;
            case SkillType.Ultimate: return ultimateSprite;
            case SkillType.Heart: return heartSprite;
            case SkillType.BeeSlow: return beeSlowSprite;
            case SkillType.DelayedShot: return delayedShotSprite;
            default: return null;
        }
    }

    private void ChooseA()
    {
        if (windowRoot != null) windowRoot.SetActive(false);
        Time.timeScale = Mathf.Max(1f, beforeTimeScale);
        onChosen?.Invoke(optionAType);
    }

    private void ChooseB()
    {
        if (windowRoot != null) windowRoot.SetActive(false);
        Time.timeScale = Mathf.Max(1f, beforeTimeScale);
        onChosen?.Invoke(optionBType);
    }
}
