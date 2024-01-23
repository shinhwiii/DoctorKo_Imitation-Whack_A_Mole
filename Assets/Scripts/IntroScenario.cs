using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScenario : MonoBehaviour
{
    [SerializeField]
    private Movement3D[] movementMoles;         // �δ������� ���� �̵���Ű�� ���� Movement3D
    [SerializeField]
    private GameObject[] textMoles;             // �δ��� ���� ���� ȹ�� ������ ȿ�� ��� text
    [SerializeField]
    private GameObject textPressAnyKey;         // "Press Any Key" text
    [SerializeField]
    private float maxY = 1.5f;                  // �δ����� �ö�� �� �ִ� �ִ� ����
    private int currentIndex = 0;               // �δ����� ������� �����ϵ��� ������ ����

    private void Awake()
    {
        StartCoroutine("Scenario");
    }

    private IEnumerator Scenario()
    {
        // �δ����� Normal -> Red -> Blue -> Gold ������� ����
        while (currentIndex < movementMoles.Length)
        {
            yield return StartCoroutine("MoveMole");
        }

        // "Press Any Key" �ؽ�Ʈ ���
        textPressAnyKey.SetActive(true);

        // ���콺 ���� ��ư�� ������ "Game" Scene���� �̵�
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("Game");
            }

            yield return null;
        }
    }

    private IEnumerator MoveMole()
    {
        movementMoles[currentIndex].MoveTo(Vector3.up);

        while (true)
        {
            // �δ����� ��ǥ ������ �����ϸ� while() �ݺ��� ����
            if (movementMoles[currentIndex].transform.position.y >= maxY)
            {
                movementMoles[currentIndex].MoveTo(Vector3.zero);

                break;
            }

            yield return null;
        }

        // �δ����� ������ �� ȿ�� �ؽ�Ʈ ���
        textMoles[currentIndex].SetActive(true);

        // ���� �δ����� �����ϵ��� �ε��� ����
        currentIndex++;
    }
}
