using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DepLao : MonoBehaviour
{
    Rigidbody2D _depLao;

    private void Start()
    {
        _depLao = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (_depLao.velocity.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
