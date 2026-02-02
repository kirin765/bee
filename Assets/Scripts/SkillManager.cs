using System.Collections.Generic;
using UnityEngine;

public enum SkillType { ArrowDamage, ArrowCooldown, Ultimate, Heart, BeeSlow, DelayedShot }

public class SkillManager : MonoBehaviour
{
    [SerializeField] private float arrowDamage = 1f;
    [SerializeField] private float cooldownMultiplier = 1.0f; // multiplies base cooldown
    [SerializeField] private int ultimateCount = 1;
    [SerializeField] private int maxUltimateCount = 2;
    [SerializeField] private float beeSpeedMultiplier = 1.0f;
    [SerializeField] private Hearts hearts;
    [SerializeField] private int maxHeartCount = 3;
    [SerializeField] private int delayedShotLevel = 0;
    [SerializeField] private int maxDelayedShotLevel = 5;
    [SerializeField] private float delayedShotInterval = 0.1f;

    public static SkillManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        ultimateCount = Mathf.Clamp(ultimateCount, 1, maxUltimateCount);
    }

    public float GetArrowDamage() => Mathf.Max(1f, arrowDamage);

    public float GetCooldownMultiplier() => cooldownMultiplier;

    public float GetBeeSpeedMultiplier() => beeSpeedMultiplier;

    public int GetDelayedShotCount()
    {
        return Mathf.Clamp(delayedShotLevel, 0, maxDelayedShotLevel);
    }

    public float GetDelayedShotInterval() => delayedShotInterval;

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
            case SkillType.ArrowDamage:
                arrowDamage *= 1.2f;
                break;
            case SkillType.ArrowCooldown:
                cooldownMultiplier = Mathf.Max(0.5f, cooldownMultiplier * 0.8f);
                break;
            case SkillType.Ultimate:
                if (ultimateCount < maxUltimateCount) ultimateCount++;
                break;
            case SkillType.Heart:
                if (hearts != null) hearts.AddHeart(maxHeartCount);
                break;
            case SkillType.BeeSlow:
                beeSpeedMultiplier = Mathf.Max(0.5f, beeSpeedMultiplier * 0.8f);
                break;
            case SkillType.DelayedShot:
                delayedShotLevel = Mathf.Clamp(delayedShotLevel + 1, 0, maxDelayedShotLevel);
                break;
        }
    }

    // Pick two random skills for level-up choices (skips Ultimate until a later level if needed)
    public (SkillType, SkillType) PickTwoSkills()
    {
        List<SkillType> pool = new List<SkillType>
        {
            SkillType.ArrowDamage,
            SkillType.ArrowCooldown,
            SkillType.Heart,
            SkillType.BeeSlow
        };

        if (ultimateCount < maxUltimateCount) pool.Add(SkillType.Ultimate);
        if (delayedShotLevel < maxDelayedShotLevel) pool.Add(SkillType.DelayedShot);

        int i = UnityEngine.Random.Range(0, pool.Count);
        int j = UnityEngine.Random.Range(0, pool.Count - 1);
        if (j >= i) j++; // ensure j != i
        return (pool[i], pool[j]);
    }
}
