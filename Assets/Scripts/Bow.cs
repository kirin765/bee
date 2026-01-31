using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private Spawner arrowSpawner;
    [SerializeField]
    private float coolTime = 1.0f;
    [SerializeField]
    private Image gauge;
    [SerializeField]
    private float ultimateShotInterval = 0.03f;
    private float x_dir = 1.0f;
    private float passedTime = 0f;


    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (cam == null) cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        float y = -halfHeight + 0.75f;

        Vector3 pos = transform.position;
        pos.y = y;
        pos.x += x_dir * speed * Time.deltaTime;

        if (pos.x < -halfWidth) x_dir = 1f;
        if (pos.x >  halfWidth) x_dir = -1f;

        transform.position = pos;

        passedTime += Time.deltaTime;
        float effectiveCool = coolTime;
        if (SkillManager.Instance != null)
            effectiveCool = coolTime * SkillManager.Instance.GetCooldownMultiplier();
        if (gauge != null)
            gauge.fillAmount = Mathf.Clamp01(passedTime / effectiveCool);
    }


    public void Shoot()
    {
        if (gauge != null && gauge.fillAmount < 1.0f) return;

        int count = 1;
        int damage = 1;
        int pierce = 1;
        if (SkillManager.Instance != null)
        {
            count = SkillManager.Instance.GetArrowCount();
            damage = SkillManager.Instance.GetArrowDamage();
            pierce = SkillManager.Instance.GetArrowPierceCount();
        }

        // simple spread offsets
        float[] offsets;
        if (count == 1) offsets = new float[] { 0f };
        else if (count == 2) offsets = new float[] { -0.1f, 0.1f };
        else offsets = new float[] { -0.2f, 0f, 0.2f };

        foreach (var off in offsets)
        {
            arrowSpawner.MakeArrow(transform.position, damage, pierce, off);
        }

        passedTime = 0f;
            
    }

    public void UseUltimate()
    {
        if (SkillManager.Instance != null && !SkillManager.Instance.TryUseUltimate()) return;
        if (arrowSpawner == null) return;

        int damage = 1;
        int pierce = 1;
        if (SkillManager.Instance != null)
        {
            damage = SkillManager.Instance.GetArrowDamage();
            pierce = SkillManager.Instance.GetArrowPierceCount();
        }

        // Straight line spread: same height, straight up
        int count = 11;
        Camera cam = Camera.main;
        float spreadX = 1.2f;
        if (cam != null)
        {
            spreadX = cam.orthographicSize * cam.aspect;
        }

        float centerX = (cam != null) ? cam.transform.position.x : transform.position.x;
        Vector3 basePos = new Vector3(centerX, transform.position.y, transform.position.z);

        StartCoroutine(UltimateBurst(basePos, damage, pierce, spreadX, count));
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
}
