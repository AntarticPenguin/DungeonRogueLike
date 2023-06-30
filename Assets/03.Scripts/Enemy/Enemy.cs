using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damageAmount)
    {
        Debug.Log($"Take Damage: {damageAmount}");
    }
}
