using System.Collections;
using TMPro;
using UnityEngine;

public class MoleHitTextViewer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 30.0f;            // 이동 속도
    private Vector2 defaultPosition;            // 이동 애니메이션이 있어서 초기 위치 지정
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
        // 오브젝트를 화면에 보이도록 설정
        gameObject.SetActive(true);
        // Score +XX, Score -300, Time +3과 같이 출력할 정보 결정
        textHit.text = hitData;

        // 텍스트가 위로 이동하며 점점 사라지는 OnAnination() 코루틴 실행
        StopCoroutine("OnAnimation");
        StartCoroutine("OnAnimation", color);
    }

    private IEnumerator OnAnimation(Color color)
    {
        // 오브젝트를 On/Off해서 사용하고, 이동 애니메이션을 했기 때문에 위치 리셋
        rectHit.anchoredPosition = defaultPosition;

        while (color.a > 0)
        {
            // Vector2.up 방향으로 이동
            rectHit.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
            // 투명도 1 -> 0으로 감소
            color.a -= Time.deltaTime;
            textHit.color = color;

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
