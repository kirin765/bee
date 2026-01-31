using UnityEngine;

public class Powers : MonoBehaviour
{
    [SerializeField] private GameObject[] powers;
    [SerializeField] private SkillManager skillManager;

    private int lastCount = -1;

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (skillManager == null) skillManager = SkillManager.Instance;
        if (skillManager == null) return;

        int count = skillManager.GetUltimateCount();
        if (count != lastCount) Refresh();
    }

    private void Refresh()
    {
        int count = skillManager != null ? skillManager.GetUltimateCount() : 0;
        lastCount = count;

        if (powers == null) return;
        for (int i = 0; i < powers.Length; i++)
        {
            if (powers[i] != null) powers[i].SetActive(i < count);
        }
    }
}
