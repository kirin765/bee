using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Xp : MonoBehaviour
{
    [SerializeField] private int[] xpPerLevel = { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15 };
    [SerializeField] private Image xpBar;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private SkillWindow skillWindow;

    private int xpSum;
    private int level;

    public int Level => level;
    public int XpSum => xpSum;

    private void Start()
    {
        xpSum = 0;
    }

    public void AddXp(int xp)
    {
        xpSum += xp;
        if (xpPerLevel == null || xpPerLevel.Length == 0) return;

        while (level < xpPerLevel.Length && xpSum >= xpPerLevel[level])
        {
            xpSum -= xpPerLevel[level];
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
            int idx = Mathf.Clamp(level, 0, xpPerLevel.Length - 1);
            xpBar.fillAmount = Mathf.Clamp01((float)xpSum / xpPerLevel[idx]);
        }

        if (levelText != null)
        {
            levelText.text = level.ToString();
        }
    }

    public int totalXP()
    {
        int sum = 0;
        for (int i = 0; i < level; i++)
        {
            sum += xpPerLevel[i];
        }

        return sum + xpSum;
    }
}
