using UnityEngine;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    [SerializeField]
    Camera cam;

    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    Spawner arrowSpawner;
    [SerializeField]
    public float coolTime = 1.0f;
    [SerializeField]
    public Image gauge;
    private float x_dir = 1.0f;
    private float passedTime = 0f;


    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
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
        gauge.fillAmount = Mathf.Clamp01(passedTime/coolTime);
    }


    public void Shoot()
    {
        if(gauge.fillAmount >= 1.0f)
        {
            arrowSpawner.MakeArrow(transform.position);
            passedTime=0f;
        }
            
    }
}