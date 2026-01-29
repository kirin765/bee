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
        if (gauge != null)
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