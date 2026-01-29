using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Xp : MonoBehaviour
{
    [SerializeField]
    // Debug: set low XP per level for quick leveling during testing
    private int[] xpPerLevel = {1,1,1};

    [SerializeField]
    private Image xpBar;
    [SerializeField]
    private TMP_Text levelText;
    private int xpSum;
    private int level = 0;

    public int Level => level;
    public int XpSum => xpSum;


    [SerializeField] private SkillManager skillManager;
    [SerializeField] private SkillWindow skillWindow;

    void Start()
    {
        xpSum = 0;
    }

    public void AddXp(int xp)
    {
        xpSum += xp;
        if (xpPerLevel == null || xpPerLevel.Length == 0) return;

        bool leveled = false;
        while (level < xpPerLevel.Length && xpSum >= xpPerLevel[level])
        {
            xpSum -= xpPerLevel[level];
            level++;
            leveled = true;
            if (xpBar != null) xpBar.fillAmount = 0f;
            // On each level up, present skill choices
            if (skillManager != null && skillWindow != null)
            {
                var (a, b) = skillManager.PickTwoSkills();
                // Show options and apply when chosen
                skillWindow.ShowOptions(a, b, chosen => { skillManager.ApplySkill(chosen); });
            }
        }

        if (xpBar != null)
            xpBar.fillAmount = Mathf.Clamp01((float)xpSum / xpPerLevel[Mathf.Clamp(level, 0, xpPerLevel.Length - 1)]);
        if (levelText != null)
            levelText.text = level.ToString();
    }

    public int totalXP()
    {
        int sum = 0;
        for(int i=0; i<level; i++)
        {
            sum += xpPerLevel[i];
        }

        return sum+xpSum;
    }
}
