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
    }

    string Describe(SkillType s)
    {
        switch (s)
        {
            case SkillType.ArrowCount: return "Arrow Count: 1 -> 3 -> 5 (increases cooldown)";
            case SkillType.ArrowDamage: return "Arrow Damage: +1";
            case SkillType.ArrowPiercing: return "Arrow Piercing: penetrate more enemies (damage penalty)";
            case SkillType.ArrowCooldown: return "Arrow Cooldown: reduce cooldown";
            case SkillType.Ultimate: return "Gain Ultimate Skill";
            default: return s.ToString();
        }
    }

    void ChooseA()
    {
        if (windowRoot != null) windowRoot.SetActive(false);
        onChosen?.Invoke(optionAType);
    }

    void ChooseB()
    {
        if (windowRoot != null) windowRoot.SetActive(false);
        onChosen?.Invoke(optionBType);
    }
}
