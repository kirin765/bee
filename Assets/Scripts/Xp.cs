using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Xp : MonoBehaviour
{
    [SerializeField]
    int[] xpPerLevel = {10,10,10};

    [SerializeField]
    Image xpBar;
    [SerializeField]
    TMP_Text levelText;
    public int xpSum;
    public int level = 0;

    void Start()
    {
        xpSum = 0;
    }

    public void AddXp(int xp)
    {
        xpSum+=xp;
        if(xpPerLevel.Length-1>level && xpSum > xpPerLevel[level])
        {
            xpSum-=xpPerLevel[level];
            level++;
            xpBar.fillAmount = 0f;
        }
        
        xpBar.fillAmount += Mathf.Clamp01((float)xpSum/xpPerLevel[level]);
        levelText.text = $"{level}";
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
