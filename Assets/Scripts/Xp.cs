using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Xp : MonoBehaviour
{
    [SerializeField]
    private int[] xpPerLevel = {10,10,10};

    [SerializeField]
    private Image xpBar;
    [SerializeField]
    private TMP_Text levelText;
    private int xpSum;
    private int level = 0;

    public int Level => level;
    public int XpSum => xpSum;

    void Start()
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
