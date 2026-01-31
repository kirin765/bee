using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UltimateButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Bow bow;
    [SerializeField] private SkillManager skillManager;

    private int lastCount = -1;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (button != null) button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
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

    private void OnClick()
    {
        if (bow != null) bow.UseUltimate();
        Refresh();
    }

    private void Refresh()
    {
        int count = skillManager != null ? skillManager.GetUltimateCount() : 0;
        lastCount = count;

        if (countText != null) countText.text = count.ToString();
        if (button != null) button.interactable = count > 0;
    }
}
