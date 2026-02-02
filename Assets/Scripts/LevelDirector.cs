using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    [SerializeField] private Spawner spawner;

    private void Start()
    {
        if (spawner == null) spawner = FindFirstObjectByType<Spawner>();
    }
}
