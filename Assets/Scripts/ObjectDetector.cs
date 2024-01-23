using UnityEngine;
using UnityEngine.Events;

public class ObjectDetector : MonoBehaviour
{
    [System.Serializable]
    public class RayCastEvent : UnityEvent<Transform> { }       // ���콺 Ŭ�� ������ ���� �̺�Ʈ Ŭ���� ����
                                                                // ��ϵǴ� �̺�Ʈ �޼ҵ�� Transform �Ű����� 1���� ������ �޼ҵ�
    [HideInInspector]
    public RayCastEvent raycastEvent = new RayCastEvent();      // �̺�Ʈ Ŭ���� �ν��Ͻ� ���� �� �޸� �Ҵ�

    private Camera mainCamera;                                  // ������ �����ϱ� ���� Camera
    private Ray ray;                                            // ������ ���� ���� ������ ���� Ray
    private RaycastHit hit;                                     // ������ �ε��� ������Ʈ ���� ������ ���� RaycastHit

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))                         // ���콺 ���� ��ư�� ������ ��
        {
            // ī�޶� ��ġ���� ȭ���� ���콺 ��ġ�� �����ϴ� ���� ����
            // ray.origin : ������ ���� ��ġ(ī�޶� ��ġ)
            // ray.direction : ������ ���� ����
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 2D ����͸� ���� 3D ������ ������Ʈ�� ���콺�� �����ϴ� ���
            // ������ �ε����� ������Ʈ�� �����ؼ� hit�� ����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // �ε��� ������Ʈ�� Transform ������ �Ű������� �̺�Ʈ ȣ��
                raycastEvent.Invoke(hit.transform);
            }
        }
    }
}
