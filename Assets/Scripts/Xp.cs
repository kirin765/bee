using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Xp : MonoBehaviour
{
    [SerializeField] private float baseXpRequirement = 10f;
    [SerializeField] private float xpIncreaseMultiplier = 1.3f;
    [SerializeField] private Image xpBar;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private SkillWindow skillWindow;

    private float xpSum;
    private int level;
    private float totalXpEarned = 0f;

    public int Level => level;
    public float XpSum => xpSum;

    private void Start()
    {
        xpSum = 0;
    }

    public void AddXp(float xp)
    {
        totalXpEarned += xp;
        xpSum += xp;

        while (xpSum >= GetRequiredXp(level))
        {
            xpSum -= GetRequiredXp(level);
            level++;
            if (xpBar != null) xpBar.fillAmount = 0f;

            if (skillManager != null && skillWindow != null)
            {
                var (a, b) = skillManager.PickTwoSkills();
                skillWindow.ShowOptions(a, b, chosen => { skillManager.ApplySkill(chosen); });
            }
        }

        if (xpBar != null)
        {
            float required = Mathf.Max(1f, GetRequiredXp(level));
            xpBar.fillAmount = Mathf.Clamp01(xpSum / required);
        }

        if (levelText != null)
        {
            levelText.text = level.ToString();
        }
    }

    public float totalXP()
    {
        return totalXpEarned;
    }

    private float GetRequiredXp(int levelIndex)
    {
        float mult = Mathf.Max(1f, xpIncreaseMultiplier);
        return Mathf.Max(1f, baseXpRequirement) * Mathf.Pow(mult, levelIndex);
    }
}
