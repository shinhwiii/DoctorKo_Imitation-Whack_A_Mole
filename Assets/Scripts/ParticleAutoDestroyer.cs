using UnityEngine;

public class ParticleAutoDestroyer : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // ��ƼŬ�� ��� ���� �ƴ϶�� ����
        if (!particle.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
