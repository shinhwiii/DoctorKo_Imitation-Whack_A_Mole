using System.Collections;
using UnityEngine;

// 지하에 대기, 지상에 대기, 지하->지상 이동, 지상->지하 이동
public enum MoleState { UnderGround = 0, OverGround, MoveUp, MoveDown }

// 두더지 종류 (기본, 점수 -, 시간 +, 점수 500+)
public enum MoleType { Normal = 0, Red, Blue, Gold }

public class MoleFSM : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;  // 콤보 초기화를 위한 GameController
    [SerializeField]
    private float waitTimeOverGround;       // 지면에 올라와서 내려가기까지 걸리는 시간
    [SerializeField]
    private float limitMinY;                // 내려갈 수 있는 최소 y 위치
    [SerializeField]
    private float limitMaxY;                // 올라올 수 있는 최대 y 위치

    private Movement3D movement3D;          // 위아래 이동을 위한 movement3D
    private MeshRenderer meshRenderer;      // 두더지의 색상 설정을 위한 meshRenderer

    private MoleType moleType;              // 두더지의 종류
    private Color defaultColor;             // 기본 두더지의 색상

    // 두더지의 현재 상태 (set은 MoleFSM 클래스 내부에서만)
    public MoleState MoleState { private set; get; }

    public MoleType MoleType
    {
        set
        {
            moleType = value;

            switch (moleType)
            {
                case MoleType.Normal:
                    meshRenderer.material.color = defaultColor;
                    break;
                case MoleType.Red:
                    meshRenderer.material.color = Color.red;
                    break;
                case MoleType.Blue:
                    meshRenderer.material.color = Color.blue;
                    break;
                case MoleType.Gold:
                    meshRenderer.material.color = Color.yellow;
                    break;
            }
        }
        get => moleType;
    }

    // 두더지가 배치되어 있는 순번 (왼쪽 상단부터 0)
    [field: SerializeField]
    public int MoleIndex { private set; get; }

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
        meshRenderer = GetComponent<MeshRenderer>();

        defaultColor = meshRenderer.material.color;         // 두더지의 최초 색상 저장

        ChangeState(MoleState.UnderGround);
    }

    public void ChangeState(MoleState newState)
    {
        // 열거형 변수를 ToString()을 이용하여 문자열로 변환하면
        // "UnderGround"와 같이 열거형 요소 이름 변환

        // 이전에 재생 중이던 상태 종료
        StopCoroutine(MoleState.ToString());
        // 상태 변경
        MoleState = newState;
        // 새로운 상태 재생
        StartCoroutine(MoleState.ToString());
    }

    /// <summary>
    /// 두더지가 바닥에서 대기하는 상태로 최초에 바닥 위치로 두더지 위치 설정
    /// </summary>
    private IEnumerator UnderGround()
    {
        // 이동 방향을 (0, 0, 0) [정지]
        movement3D.MoveTo(Vector3.zero);
        // 두더지의 y 위치를 홀에 숨어있는 limitMinY 위치로 설정
        transform.position = new Vector3(transform.position.x, limitMinY, transform.position.z);

        yield return null;
    }

    /// <summary>
    /// 두더지가 홀 밖에 나와있는 상태로 waitTimeOverGround 동안 대기
    /// </summary>
    private IEnumerator OverGround()
    {
        // 이동 방향을 (0, 0, 0) [정지]
        movement3D.MoveTo(Vector3.zero);
        // 두더지의 y 위치를 홀에 숨어있는 limitMinY 위치로 설정
        transform.position = new Vector3(transform.position.x, limitMaxY, transform.position.z);

        // waitTimeOverGround 동안 대기
        yield return new WaitForSeconds(waitTimeOverGround);

        // 두더지의 상태를 MoveDown으로 변경
        ChangeState(MoleState.MoveDown);
    }

    /// <summary>
    /// 두더지가 홀 밖에 나와있는 상태 (maxYPosOverGround 위치까지 위로 이동)
    /// </summary>
    private IEnumerator MoveUp()
    {
        // 이동 방향을 (0, 1, 0) [정지]
        movement3D.MoveTo(Vector3.up);

        while (true)
        {
            // 두더지의 y 위치가 limitMaxY에 도달하면 상태 변경
            if (transform.position.y >= limitMaxY)
            {
                // OverGround 상태로 변경
                ChangeState(MoleState.OverGround);
                break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 두더지가 홀로 들어가는 상태 (minYPosUnderGround 위치까지 위로 이동)
    /// </summary>
    private IEnumerator MoveDown()
    {
        // 이동 방향을 (0, 1, 0) [정지]
        movement3D.MoveTo(Vector3.down);

        while (true)
        {
            // 두더지의 y 위치가 limitMaxY에 도달하면 상태 변경
            if (transform.position.y <= limitMinY)
            {
                break;      // while() 아래쪽 실행을 위해 이동 완료시 break;
            }

            yield return null;
        }

        // 망치에 타격당하지 않고 자연스럽게 구멍으로 들어갔을 경우 호출
        if (moleType == MoleType.Normal)        // 망치에 타격당하지 않고 구멍으로 들어간 두더지의 속성이 Normal이면 콤보 초기화
        {
            gameController.Combo = 0;
        }

        // UnderGround 상태로 변경
        ChangeState(MoleState.UnderGround);
    }
}
