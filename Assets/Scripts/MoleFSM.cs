using System.Collections;
using UnityEngine;

// ���Ͽ� ���, ���� ���, ����->���� �̵�, ����->���� �̵�
public enum MoleState { UnderGround = 0, OverGround, MoveUp, MoveDown }

// �δ��� ���� (�⺻, ���� -, �ð� +, ���� 500+)
public enum MoleType { Normal = 0, Red, Blue, Gold }

public class MoleFSM : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;  // �޺� �ʱ�ȭ�� ���� GameController
    [SerializeField]
    private float waitTimeOverGround;       // ���鿡 �ö�ͼ� ����������� �ɸ��� �ð�
    [SerializeField]
    private float limitMinY;                // ������ �� �ִ� �ּ� y ��ġ
    [SerializeField]
    private float limitMaxY;                // �ö�� �� �ִ� �ִ� y ��ġ

    private Movement3D movement3D;          // ���Ʒ� �̵��� ���� movement3D
    private MeshRenderer meshRenderer;      // �δ����� ���� ������ ���� meshRenderer

    private MoleType moleType;              // �δ����� ����
    private Color defaultColor;             // �⺻ �δ����� ����

    // �δ����� ���� ���� (set�� MoleFSM Ŭ���� ���ο�����)
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

    // �δ����� ��ġ�Ǿ� �ִ� ���� (���� ��ܺ��� 0)
    [field: SerializeField]
    public int MoleIndex { private set; get; }

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
        meshRenderer = GetComponent<MeshRenderer>();

        defaultColor = meshRenderer.material.color;         // �δ����� ���� ���� ����

        ChangeState(MoleState.UnderGround);
    }

    public void ChangeState(MoleState newState)
    {
        // ������ ������ ToString()�� �̿��Ͽ� ���ڿ��� ��ȯ�ϸ�
        // "UnderGround"�� ���� ������ ��� �̸� ��ȯ

        // ������ ��� ���̴� ���� ����
        StopCoroutine(MoleState.ToString());
        // ���� ����
        MoleState = newState;
        // ���ο� ���� ���
        StartCoroutine(MoleState.ToString());
    }

    /// <summary>
    /// �δ����� �ٴڿ��� ����ϴ� ���·� ���ʿ� �ٴ� ��ġ�� �δ��� ��ġ ����
    /// </summary>
    private IEnumerator UnderGround()
    {
        // �̵� ������ (0, 0, 0) [����]
        movement3D.MoveTo(Vector3.zero);
        // �δ����� y ��ġ�� Ȧ�� �����ִ� limitMinY ��ġ�� ����
        transform.position = new Vector3(transform.position.x, limitMinY, transform.position.z);

        yield return null;
    }

    /// <summary>
    /// �δ����� Ȧ �ۿ� �����ִ� ���·� waitTimeOverGround ���� ���
    /// </summary>
    private IEnumerator OverGround()
    {
        // �̵� ������ (0, 0, 0) [����]
        movement3D.MoveTo(Vector3.zero);
        // �δ����� y ��ġ�� Ȧ�� �����ִ� limitMinY ��ġ�� ����
        transform.position = new Vector3(transform.position.x, limitMaxY, transform.position.z);

        // waitTimeOverGround ���� ���
        yield return new WaitForSeconds(waitTimeOverGround);

        // �δ����� ���¸� MoveDown���� ����
        ChangeState(MoleState.MoveDown);
    }

    /// <summary>
    /// �δ����� Ȧ �ۿ� �����ִ� ���� (maxYPosOverGround ��ġ���� ���� �̵�)
    /// </summary>
    private IEnumerator MoveUp()
    {
        // �̵� ������ (0, 1, 0) [����]
        movement3D.MoveTo(Vector3.up);

        while (true)
        {
            // �δ����� y ��ġ�� limitMaxY�� �����ϸ� ���� ����
            if (transform.position.y >= limitMaxY)
            {
                // OverGround ���·� ����
                ChangeState(MoleState.OverGround);
                break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// �δ����� Ȧ�� ���� ���� (minYPosUnderGround ��ġ���� ���� �̵�)
    /// </summary>
    private IEnumerator MoveDown()
    {
        // �̵� ������ (0, 1, 0) [����]
        movement3D.MoveTo(Vector3.down);

        while (true)
        {
            // �δ����� y ��ġ�� limitMaxY�� �����ϸ� ���� ����
            if (transform.position.y <= limitMinY)
            {
                break;      // while() �Ʒ��� ������ ���� �̵� �Ϸ�� break;
            }

            yield return null;
        }

        // ��ġ�� Ÿ�ݴ����� �ʰ� �ڿ������� �������� ���� ��� ȣ��
        if (moleType == MoleType.Normal)        // ��ġ�� Ÿ�ݴ����� �ʰ� �������� �� �δ����� �Ӽ��� Normal�̸� �޺� �ʱ�ȭ
        {
            gameController.Combo = 0;
        }

        // UnderGround ���·� ����
        ChangeState(MoleState.UnderGround);
    }
}
