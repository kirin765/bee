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
    [SerializeField] private Sprite arrowCountSprite;
    [SerializeField] private Sprite arrowDamageSprite;
    [SerializeField] private Sprite arrowPiercingSprite;
    [SerializeField] private Sprite arrowCooldownSprite;
    [SerializeField] private Sprite ultimateSprite;

    private Action<SkillType> onChosen;
    private SkillType optionAType;
    private SkillType optionBType;

    void Awake()
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
        if (optionAText != null) optionAText.text = Describe(a);
        if (optionBText != null) optionBText.text = Describe(b);
        if (optionAImage != null) optionAImage.sprite = GetSprite(a);
        if (optionBImage != null) optionBImage.sprite = GetSprite(b);
        // Pause game while player chooses a skill
        Time.timeScale = 0f;
    }

    string Describe(SkillType s)
    {
        switch (s)
        {
            case SkillType.ArrowCount: return "Arrow Count: 1 -> 2 -> 3 (increases cooldown)";
            case SkillType.ArrowDamage: return "Arrow Damage: +1";
            case SkillType.ArrowPiercing: return "Arrow Piercing: penetrate more enemies (damage penalty)";
            case SkillType.ArrowCooldown: return "Arrow Cooldown: reduce cooldown";
            case SkillType.Ultimate: return "Ultimate: +1 (max 2)";
            default: return s.ToString();
        }
    }

    Sprite GetSprite(SkillType s)
    {
        switch (s)
        {
            case SkillType.ArrowCount: return arrowCountSprite;
            case SkillType.ArrowDamage: return arrowDamageSprite;
            case SkillType.ArrowPiercing: return arrowPiercingSprite;
            case SkillType.ArrowCooldown: return arrowCooldownSprite;
            case SkillType.Ultimate: return ultimateSprite;
            default: return null;
        }
    }

    void ChooseA()
    {
        if (windowRoot != null) windowRoot.SetActive(false);
        Time.timeScale = 1f;
        onChosen?.Invoke(optionAType);
    }

    void ChooseB()
    {
        if (windowRoot != null) windowRoot.SetActive(false);
        Time.timeScale = 1f;
        onChosen?.Invoke(optionBType);
    }
}
