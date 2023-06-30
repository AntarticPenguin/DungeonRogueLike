using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private float _horizontal;
    private float _vertical;
    
    public bool IsPressAttack { get; private set; }
    public Vector3 InputDirection { get; private set; } = Vector3.zero;

    private void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        IsPressAttack = Input.GetMouseButtonDown(0);
        InputDirection = new Vector3(_horizontal, 0, _vertical).normalized;
    }
}
