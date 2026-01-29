using System;
using UnityEngine;

public enum SkillType { ArrowCount, ArrowDamage, ArrowPiercing, ArrowCooldown, Ultimate }

public class SkillManager : MonoBehaviour
{
    [SerializeField] private int arrowCountLevel = 0; // 0 -> 1, 1 -> 2, 2 -> 3
    [SerializeField] private int arrowDamage = 1;
    [SerializeField] private int arrowPierceLevel = 0; // 0..2 meaning 0..2 extra pierces (max 3)
    [SerializeField] private float cooldownMultiplier = 1.0f; // multiplies base cooldown
    [SerializeField] private bool hasUltimate = false;

    public static SkillManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public int GetArrowCount()
    {
        return Mathf.Clamp(arrowCountLevel, 0, 2) + 1; // 1..3
    }

    public int GetArrowDamage() => Math.Max(1, arrowDamage);

    public int GetArrowPierceCount() => Mathf.Clamp(arrowPierceLevel, 0, 2) + 1; // 1..3

    public float GetCooldownMultiplier() => cooldownMultiplier;

    public bool HasUltimate() => hasUltimate;

    // Apply chosen skill (simple progression)
    public void ApplySkill(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.ArrowCount:
                arrowCountLevel = Mathf.Clamp(arrowCountLevel + 1, 0, 2);
                // penalty: increase cooldown by 15% per level
                cooldownMultiplier *= 1.15f;
                break;
            case SkillType.ArrowDamage:
                arrowDamage += 1;
                break;
            case SkillType.ArrowPiercing:
                arrowPierceLevel = Mathf.Clamp(arrowPierceLevel + 1, 0, 2);
                // penalty: reduce damage by 10% per pierce-level (rounded)
                arrowDamage = Math.Max(1, Mathf.RoundToInt(arrowDamage * 0.9f));
                break;
            case SkillType.ArrowCooldown:
                cooldownMultiplier = Mathf.Max(0.5f, cooldownMultiplier * 0.9f);
                break;
            case SkillType.Ultimate:
                hasUltimate = true;
                break;
        }
    }

    // Pick two random skills for level-up choices (skips Ultimate until a later level if needed)
    public (SkillType, SkillType) PickTwoSkills()
    {
        SkillType[] all = { SkillType.ArrowCount, SkillType.ArrowDamage, SkillType.ArrowPiercing, SkillType.ArrowCooldown, SkillType.Ultimate };
        int i = UnityEngine.Random.Range(0, all.Length);
        int j = UnityEngine.Random.Range(0, all.Length - 1);
        if (j >= i) j++; // ensure j != i
        return (all[i], all[j]);
    }
}
