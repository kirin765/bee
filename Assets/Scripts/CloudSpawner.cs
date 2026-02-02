using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private Sprite[] cloudSprites;
    [SerializeField] private int cloudCount = 4;
    [SerializeField] private float minY = -2.5f;
    [SerializeField] private float maxY = 2.5f;
    [SerializeField] private float minSpeed = 0.1f;
    [SerializeField] private float maxSpeed = 0.35f;
    [SerializeField] private float minFloatAmplitude = 0.05f;
    [SerializeField] private float maxFloatAmplitude = 0.2f;
    [SerializeField] private float minFloatFrequency = 0.2f;
    [SerializeField] private float maxFloatFrequency = 0.6f;
    [SerializeField] private float wrapPadding = 1.0f;
    [SerializeField] private bool randomDirection = true;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        SpawnClouds();
    }

    private void SpawnClouds()
    {
        if (cloudSprites == null || cloudSprites.Length == 0) return;
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        float left = cam.transform.position.x - halfWidth - wrapPadding;
        float right = cam.transform.position.x + halfWidth + wrapPadding;

        for (int i = 0; i < Mathf.Max(1, cloudCount); i++)
        {
            Sprite sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
            GameObject go = new GameObject($"Cloud_{i}");
            go.transform.SetParent(transform, false);

            float x = Random.Range(left, right);
            float y = Random.Range(minY, maxY);
            go.transform.position = new Vector3(x, y, 0f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = -10;

            Cloud cloud = go.AddComponent<Cloud>();
            SetCloudParams(cloud);
        }
    }

    private void SetCloudParams(Cloud cloud)
    {
        if (cloud == null) return;
        cloud.SetSpeed(Random.Range(minSpeed, maxSpeed));
        cloud.SetFloatAmplitude(Random.Range(minFloatAmplitude, maxFloatAmplitude));
        cloud.SetFloatFrequency(Random.Range(minFloatFrequency, maxFloatFrequency));
        cloud.SetWrapPadding(wrapPadding);
        bool dir = randomDirection ? (Random.value > 0.5f) : true;
        cloud.SetMoveRight(dir);
    }
}
