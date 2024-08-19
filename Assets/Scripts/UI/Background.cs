using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private Transform target; // 이어지는 배경?
    [SerializeField] private float scrollAmount;
    [SerializeField] private float moveSpeed; // 이동 속도
    [SerializeField] private Vector3 moveDirection; // 이동 방향

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        if(transform.position.x <= -scrollAmount)
        {
            transform.position = target.position - moveDirection * scrollAmount;
        }
    }

}
