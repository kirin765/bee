using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float speed = 0.2f;
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatFrequency = 0.4f;
    [SerializeField] private float wrapPadding = 1.0f;
    [SerializeField] private bool moveRight = true;

    private Camera cam;
    private float baseY;

    private void Start()
    {
        cam = Camera.main;
        baseY = transform.position.y;
    }

    private void Update()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        float dir = moveRight ? 1f : -1f;
        Vector3 pos = transform.position;
        pos.x += dir * speed * Time.deltaTime;
        pos.y = baseY + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = pos;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        float left = cam.transform.position.x - halfWidth - wrapPadding;
        float right = cam.transform.position.x + halfWidth + wrapPadding;

        if (moveRight && pos.x > right)
        {
            pos.x = left;
            transform.position = pos;
        }
        else if (!moveRight && pos.x < left)
        {
            pos.x = right;
            transform.position = pos;
        }
    }

    public void SetSpeed(float value) => speed = value;
    public void SetFloatAmplitude(float value) => floatAmplitude = value;
    public void SetFloatFrequency(float value) => floatFrequency = value;
    public void SetWrapPadding(float value) => wrapPadding = value;
    public void SetMoveRight(bool value) => moveRight = value;
}
