using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FondoMovimiento : MonoBehaviour
{
    [SerializeField] private Vector2 velocidadMovimiento;
    private Vector2 offset;
    private Material material;
    private Rigidbody2D playerRB;

    void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        playerRB = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Update() {
        offset = (playerRB.velocity.x * 0.1f) * velocidadMovimiento * Time.deltaTime;
        material.mainTextureOffset += offset; 
    }
}
