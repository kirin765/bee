using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { ArrowCount, ArrowDamage, ArrowPiercing, ArrowCooldown, Ultimate }

public class SkillManager : MonoBehaviour
{
    [SerializeField] private int arrowCountLevel = 0; // 0 -> 1, 1 -> 2, 2 -> 3
    [SerializeField] private int arrowDamage = 1;
    [SerializeField] private int arrowPierceLevel = 0; // 0..2 meaning 0..2 extra pierces (max 3)
    [SerializeField] private float cooldownMultiplier = 1.0f; // multiplies base cooldown
    [SerializeField] private int ultimateCount = 1;
    [SerializeField] private int maxUltimateCount = 2;

    public static SkillManager Instance { get; private set; }

    private const int MaxArrowLevel = 2;
    private const int MaxPierceLevel = 2;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        ultimateCount = Mathf.Clamp(ultimateCount, 1, maxUltimateCount);
    }

    public int GetArrowCount()
    {
        return Mathf.Clamp(arrowCountLevel, 0, MaxArrowLevel) + 1; // 1..3
    }

    public int GetArrowDamage() => Math.Max(1, arrowDamage);

    public int GetArrowPierceCount() => Mathf.Clamp(arrowPierceLevel, 0, MaxPierceLevel) + 1; // 1..3

    public float GetCooldownMultiplier() => cooldownMultiplier;

    public bool HasUltimate() => GetUltimateCount() > 0;

    public int GetUltimateCount()
    {
        return Mathf.Clamp(ultimateCount, 0, maxUltimateCount);
    }

    public bool TryUseUltimate()
    {
        if (ultimateCount <= 0) return false;
        ultimateCount--;
        return true;
    }

    // Apply chosen skill (simple progression)
    public void ApplySkill(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.ArrowCount:
                arrowCountLevel = Mathf.Clamp(arrowCountLevel + 1, 0, MaxArrowLevel);
                // penalty: increase cooldown by 15% per level
                cooldownMultiplier *= 1.15f;
                break;
            case SkillType.ArrowDamage:
                arrowDamage += 1;
                break;
            case SkillType.ArrowPiercing:
                arrowPierceLevel = Mathf.Clamp(arrowPierceLevel + 1, 0, MaxPierceLevel);
                // penalty: reduce damage by 10% per pierce-level (rounded)
                arrowDamage = Math.Max(1, Mathf.RoundToInt(arrowDamage * 0.9f));
                break;
            case SkillType.ArrowCooldown:
                cooldownMultiplier = Mathf.Max(0.5f, cooldownMultiplier * 0.9f);
                break;
            case SkillType.Ultimate:
                if (ultimateCount < maxUltimateCount) ultimateCount++;
                break;
        }
    }

    // Pick two random skills for level-up choices (skips Ultimate until a later level if needed)
    public (SkillType, SkillType) PickTwoSkills()
    {
        List<SkillType> pool = new List<SkillType>
        {
            SkillType.ArrowCount,
            SkillType.ArrowDamage,
            SkillType.ArrowPiercing,
            SkillType.ArrowCooldown
        };

        if (ultimateCount < maxUltimateCount) pool.Add(SkillType.Ultimate);

        int i = UnityEngine.Random.Range(0, pool.Count);
        int j = UnityEngine.Random.Range(0, pool.Count - 1);
        if (j >= i) j++; // ensure j != i
        return (pool[i], pool[j]);
    }
}
