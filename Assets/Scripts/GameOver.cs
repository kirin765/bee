using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private Xp xp;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
        float xpSum = xp != null ? xp.totalXP() : 0f;
        if (xpText != null) xpText.text = $"{xpSum:0.##}XP";
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
