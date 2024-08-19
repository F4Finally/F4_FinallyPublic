using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScrolling : MonoBehaviour
{
    [SerializeField] Renderer _renderer;
    [SerializeField] float speed = 1.0f;

    private void Update()
    {
        float move = Time.deltaTime * speed;
        _renderer.material.mainTextureOffset += Vector2.right * move;
    }

}
