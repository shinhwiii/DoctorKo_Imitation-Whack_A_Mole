using System.Collections;
using TMPro;
using UnityEngine;

public class MoleHitTextViewer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 30.0f;            // �̵� �ӵ�
    private Vector2 defaultPosition;            // �̵� �ִϸ��̼��� �־ �ʱ� ��ġ ����
    private TextMeshProUGUI textHit;
    private RectTransform rectHit;

    private void Awake()
    {
        textHit = GetComponent<TextMeshProUGUI>();
        rectHit = GetComponent<RectTransform>();
        defaultPosition = rectHit.anchoredPosition;

        gameObject.SetActive(false);
    }

    public void OnHit(string hitData, Color color)
    {
        // ������Ʈ�� ȭ�鿡 ���̵��� ����
        gameObject.SetActive(true);
        // Score +XX, Score -300, Time +3�� ���� ����� ���� ����
        textHit.text = hitData;

        // �ؽ�Ʈ�� ���� �̵��ϸ� ���� ������� OnAnination() �ڷ�ƾ ����
        StopCoroutine("OnAnimation");
        StartCoroutine("OnAnimation", color);
    }

    private IEnumerator OnAnimation(Color color)
    {
        // ������Ʈ�� On/Off�ؼ� ����ϰ�, �̵� �ִϸ��̼��� �߱� ������ ��ġ ����
        rectHit.anchoredPosition = defaultPosition;

        while (color.a > 0)
        {
            // Vector2.up �������� �̵�
            rectHit.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
            // ���� 1 -> 0���� ����
            color.a -= Time.deltaTime;
            textHit.color = color;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
