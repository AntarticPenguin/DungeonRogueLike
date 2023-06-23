using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] int _damageAmount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log($"Trigger Hit:{other.name}");
            if (other.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_damageAmount);
            }
        }
    }
}
