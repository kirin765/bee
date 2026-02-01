using UnityEngine;

public class Hearts : MonoBehaviour
{
    [SerializeField] private GameObject[] hearts;
    [SerializeField] private bool debugInvincible = false;
    [SerializeField] private int maxHearts = 3;

    public bool IsInvincible => debugInvincible;

    private void Start()
    {
        if (hearts == null) return;
        int limit = Mathf.Min(maxHearts, hearts.Length);
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i >= limit) hearts[i].SetActive(false);
        }
    }

    public int LossHeart()
    {
        if (hearts == null || hearts.Length == 0) return 0;

        for (int i = hearts.Length - 1; i >= 0; i--)
        {
            if (hearts[i].activeSelf)
            {
                hearts[i].SetActive(false);
                return i;
            }
        }

        return 0;
    }

    public bool AddHeart(int maxAllowed)
    {
        if (hearts == null || hearts.Length == 0) return false;
        int limit = Mathf.Min(maxAllowed, maxHearts, hearts.Length);
        for (int i = 0; i < limit; i++)
        {
            if (!hearts[i].activeSelf)
            {
                hearts[i].SetActive(true);
                return true;
            }
        }

        return false;
    }

    [ContextMenu("Debug/Toggle Invincible")]
    private void ToggleInvincible()
    {
        debugInvincible = !debugInvincible;
    }
}
