using System;
using UnityEngine;
using UnityEngine.UI;

public class Hearts : MonoBehaviour
{
    [SerializeField]
    GameObject[] hearts;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int LossHeart()
    {
        if (hearts == null || hearts.Length == 0) return 0;
        for(int i = hearts.Length - 1; i >= 0; i--)
        {
            if (hearts[i].activeSelf)
            {
                hearts[i].SetActive(false);
                return i;
            }
        }
        return 0;
    }
}
