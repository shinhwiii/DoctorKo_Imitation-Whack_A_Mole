using System.Collections;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    // 싱글톤 처리를 위한 instance 변수 선언
    private static ShakeCamera instance;
    // 외부에서 Get 접근만 가능하도록 instance property 선언
    public static ShakeCamera Instance => instance;

    private float shakeTime;
    private float shakeIntensity;

    /// <summary>
    /// Main Camera 오브젝트에 컴포넌트로 적용하면
    /// 게임을 실행할 때 메모리 할당 / 생성자 메모리 실행
    /// 이때 자기 자신의 정보를 instance 변수에 저장
    /// </summary>
    public ShakeCamera()
    {
        instance = this;
    }

    /// <summary>
    /// 외부에서 카메라 흔들림을 조작할 때 호출하는 메소드
    /// ex) OnShakeCamera(1);           => 1초간 0.1의 세기로 흔들림
    /// ex) OnShakeCamera(0.5f, 1);     => 0.5초간 1의 세기로 흔들림
    /// </summary>
    /// <param name="shakeTime"> 카메라 흔들림 지속 시간 (설정하지 않으면 default 1.0f)
    /// <param name="shakeIntensity"> 카메라 흔들림 세기 (설정하지 않으면 default 0.1f)
    public void OnShakeCamera(float shakeTime = 1.0f, float shakeIntensity = 0.1f)
    {
        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;

        StopCoroutine("ShakeByPosition");
        StartCoroutine("ShakeByPosition");
    }

    /// <summary>
    /// 카메라를 shakeTime 동안 shakeIntensity의 세리고 흔드는 코루틴 함수
    /// </summary>
    private IEnumerator ShakeByPosition()
    {
        // 흔들리기 직전의 시작 위치 (흔들림 종료 후 돌아오는 위치)
        Vector3 startPosition = transform.position;

        while (shakeTime > 0f)
        {
            // 초기 위치로부터 구 범위(Size 1) * shakeIntensity의 범위 안에서 카메라 위치 흔들기
            transform.position = startPosition + Random.insideUnitSphere * shakeIntensity;

            // 시간 감소
            shakeTime -= Time.deltaTime;

            yield return null;
        }

        transform.position = startPosition;
    }
}
