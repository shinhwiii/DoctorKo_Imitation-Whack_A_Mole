using UnityEngine;
using UnityEngine.Events;

public class ObjectDetector : MonoBehaviour
{
    [System.Serializable]
    public class RayCastEvent : UnityEvent<Transform> { }       // 마우스 클릭 감지를 위한 이벤트 클래스 정의
                                                                // 등록되는 이벤트 메소드는 Transform 매개변수 1개를 가지는 메소드
    [HideInInspector]
    public RayCastEvent raycastEvent = new RayCastEvent();      // 이벤트 클래스 인스턴스 생성 및 메모리 할당

    private Camera mainCamera;                                  // 광선을 생성하기 위한 Camera
    private Ray ray;                                            // 생성된 광선 정보 저장을 위한 Ray
    private RaycastHit hit;                                     // 광선에 부딪힌 오브젝트 정보 저장을 위한 RaycastHit

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))                         // 마우스 왼쪽 버튼을 눌렀을 때
        {
            // 카메라 위치에서 화면의 마우스 위치를 관통하는 광선 생성
            // ray.origin : 광선의 시작 위치(카메라 위치)
            // ray.direction : 광선의 진행 방향
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 2D 모니터를 통해 3D 월드의 오브젝트를 마우스로 선택하는 방법
            // 광선에 부딪히는 오브젝트를 검출해서 hit에 저장
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // 부딪힌 오브젝트의 Transform 정보를 매개변수로 이벤트 호출
                raycastEvent.Invoke(hit.transform);
            }
        }
    }
}
