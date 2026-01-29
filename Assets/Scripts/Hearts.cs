using UnityEngine;

public class Hearts : MonoBehaviour
{
    [SerializeField] private GameObject[] hearts;
    [SerializeField] private bool debugInvincible = false;

    public bool IsInvincible => debugInvincible;

    private void Start()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(true);
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

    [ContextMenu("Debug/Toggle Invincible")]
    private void ToggleInvincible()
    {
        debugInvincible = !debugInvincible;
    }
}
