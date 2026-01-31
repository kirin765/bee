using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Spawner arrowSpawner;
    [SerializeField] private float coolTime = 1.0f;
    [SerializeField] private Image gauge;
    [SerializeField] private float ultimateShotInterval = 0.03f;

    private float xDir = 1.0f;
    private float passedTime = 0f;

    private const float BowYOffset = 0.75f;
    private const int UltimateArrowCount = 11;
    private static readonly float[] OneOffset = { 0f };
    private static readonly float[] TwoOffsets = { -0.1f, 0.1f };
    private static readonly float[] ThreeOffsets = { -0.2f, 0f, 0.2f };

    private void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (cam == null) cam = Camera.main;
        UpdateMovement();
        UpdateGauge();
    }


    public void Shoot()
    {
        if (gauge != null && gauge.fillAmount < 1.0f) return;

        int count = 1;
        int damage = 1;
        int pierce = 1;
        SkillManager skillManager = SkillManager.Instance;
        if (skillManager != null)
        {
            count = skillManager.GetArrowCount();
            damage = skillManager.GetArrowDamage();
            pierce = skillManager.GetArrowPierceCount();
        }

        float[] offsets = GetOffsets(count);
        foreach (float off in offsets)
        {
            arrowSpawner.MakeArrow(transform.position, damage, pierce, off);
        }

        passedTime = 0f;
    }

    public void UseUltimate()
    {
        SkillManager skillManager = SkillManager.Instance;
        if (skillManager != null && !skillManager.TryUseUltimate()) return;
        if (arrowSpawner == null) return;

        int damage = 1;
        int pierce = 1;
        if (skillManager != null)
        {
            damage = skillManager.GetArrowDamage();
            pierce = skillManager.GetArrowPierceCount();
        }

        Camera currentCam = cam != null ? cam : Camera.main;
        float spreadX = 1.2f;
        if (currentCam != null) spreadX = currentCam.orthographicSize * currentCam.aspect;

        float centerX = (currentCam != null) ? currentCam.transform.position.x : transform.position.x;
        Vector3 basePos = new Vector3(centerX, transform.position.y, transform.position.z);

        StartCoroutine(UltimateBurst(basePos, damage, pierce, spreadX, UltimateArrowCount));
    }

    private IEnumerator UltimateBurst(Vector3 basePos, int damage, int pierce, float spreadX, int count)
    {
        if (count <= 0) yield break;

        Vector2[] offsets = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0f : (float)i / (count - 1);
            float x = Mathf.Lerp(-spreadX, spreadX, t);
            offsets[i] = new Vector2(x, 0f);
        }

        int center = count / 2;
        int left = center - 1;
        int right = center + 1;

        arrowSpawner.MakeArrow(basePos, damage, pierce, offsets[center], 0f);

        while (left >= 0 || right < count)
        {
            if (ultimateShotInterval > 0f) yield return new WaitForSeconds(ultimateShotInterval);
            if (left >= 0)
            {
                arrowSpawner.MakeArrow(basePos, damage, pierce, offsets[left], 0f);
                left--;
            }

            if (right < count)
            {
                arrowSpawner.MakeArrow(basePos, damage, pierce, offsets[right], 0f);
                right++;
            }
        }
    }

    private void UpdateMovement()
    {
        if (cam == null) return;
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float y = -halfHeight + BowYOffset;

        Vector3 pos = transform.position;
        pos.y = y;
        pos.x += xDir * speed * Time.deltaTime;

        if (pos.x < -halfWidth) xDir = 1f;
        if (pos.x > halfWidth) xDir = -1f;

        transform.position = pos;
    }

    private void UpdateGauge()
    {
        passedTime += Time.deltaTime;
        float effectiveCool = coolTime;
        SkillManager skillManager = SkillManager.Instance;
        if (skillManager != null) effectiveCool = coolTime * skillManager.GetCooldownMultiplier();

        if (gauge != null)
        {
            gauge.fillAmount = Mathf.Clamp01(passedTime / effectiveCool);
        }
    }

    private static float[] GetOffsets(int count)
    {
        if (count <= 1) return OneOffset;
        if (count == 2) return TwoOffsets;
        return ThreeOffsets;
    }
}
