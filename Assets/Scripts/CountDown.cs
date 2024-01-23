using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountDown : MonoBehaviour
{
    [System.Serializable]
    private class CountDownEvent : UnityEvent { }
    private CountDownEvent endOfCountDown;              // ī��Ʈ �ٿ� ���� �� �ܺ� �޼ҵ� ������ ���� �̺�Ʈ Ŭ���� ���

    private TextMeshProUGUI textCountDown;              // ī��Ʈ �ٿ� �ؽ�Ʈ�� ����ϴ� UI
    private AudioSource audioSource;                    // ī��Ʈ �ٿ� ���� ���

    [SerializeField]
    private int maxFontSize;                            // ��Ʈ�� �ִ� ũ��
    [SerializeField]
    private int minFontSize;                            // ��Ʈ�� �ּ� ũ��

    private void Awake()
    {
        endOfCountDown = new CountDownEvent();
        textCountDown = GetComponent<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
    }

    public void StartCountDown(UnityAction action, int start = 3, int end = 1)
    {
        StartCoroutine(OnCountDown(action, start, end));
    }

    private IEnumerator OnCountDown(UnityAction action, int start, int end)
    {
        // action �޼ҵ带 �̺�Ʈ�� ���
        endOfCountDown.AddListener(action);

        while (start > end - 1)
        {
            // ī��Ʈ �ٿ� ���� ���
            audioSource.Play();

            // ī��Ʈ �ٿ� �ؽ�Ʈ ����
            textCountDown.text = start.ToString();

            // ��Ʈ ũ�⸦ �����ϴ� �ִϸ��̼� (��� �Ϸ� �� �Ʒ� �ڵ� ����)
            yield return StartCoroutine("OnFontAnimation");

            // ī��Ʈ �ٿ� ũ�� 1 ����
            start--;
        }

        // action �޼ҵ带 ����
        endOfCountDown.Invoke();

        // action �޼ҵ带 �̺�Ʈ���� ����
        endOfCountDown.RemoveListener(action);

        // ī��Ʈ �ٿ� ������Ʈ�� ��Ȱ��ȭ    
        gameObject.SetActive(false);
    }

    private IEnumerator OnFontAnimation()
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            // ��Ʈ ũ�⸦ 200���� 100���� percent �ð� ���� ����
            textCountDown.fontSize = Mathf.Lerp(maxFontSize, minFontSize, percent);

            yield return null;
        }
    }
}