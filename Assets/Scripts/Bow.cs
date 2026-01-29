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
        else if (count == 3) offsets = new float[] { -0.2f, 0f, 0.2f };
        else offsets = new float[] { -0.4f, -0.2f, 0f, 0.2f, 0.4f };

        foreach (var off in offsets)
        {
            arrowSpawner.MakeArrow(transform.position, damage, pierce, off);
        }

        passedTime = 0f;
            
    }
}