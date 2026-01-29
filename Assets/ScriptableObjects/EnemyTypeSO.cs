using UnityEngine;

[CreateAssetMenu(menuName = "BeeSO/EnemyTypeSO", fileName = "EnemyTypeSO")]
public class EnemyTypeSO : ScriptableObject
{
    public float speed = 1.0f;
    public int maxHP = 5;
    public int xpReward = 1;
    public float _scale = 1.0f;
    public bool isBoss = false;
}
