using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffectOfParticle : Singleton<TouchEffectOfParticle>
{
    [SerializeField] private ParticleSystem flowerParticle;

    private void Update()
    {
        
        CreateFlowerParticle();
    }

    public void CreateFlowerParticle()
    {
        // 마우스 버튼이 눌렀다 때면 파티클 재생, 이게 발생하지 않으면 아래 연산을 처리하지 않도록 서순 변경
        if (!Input.GetMouseButtonUp(0))
        {
            return;
        }

        // 마우스 위치를 가져오기
        Vector3 mPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mPosition.z = 0;

        // 파티클 시스템 위치를 마우스 위치로 업데이트
        flowerParticle.transform.position = mPosition;

        flowerParticle.Play();
    }
}
