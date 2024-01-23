using System.Collections;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField]
    private float maxY;                                 // ��ġ�� �ִ� y ��ġ
    [SerializeField]
    private float minY;                                 // ��ġ�� �ּ� y ��ġ
    [SerializeField]
    private GameObject moleHitEffectPrefab;             // �δ��� Ÿ�� ȿ�� ������
    [SerializeField]
    private AudioClip[] audioClips;                     // �δ����� Ÿ������ �� ����Ǵ� ����
    [SerializeField]
    private MoleHitTextViewer[] moleHitTextViewer;      // Ÿ���� �δ��� ��ġ�� Ÿ�� ���� �ؽ�Ʈ ���
    [SerializeField]
    private GameController gameController;              // ���� ������ ���� GameController
    [SerializeField]
    private ObjectDetector objectDetector;              // ���콺 Ŭ������ ������Ʈ ������ ���� ObjectDetector
    private Movement3D movement3D;                      // ��ġ ������Ʈ �̵��� ���� Movement3D
    private AudioSource audioSource;                    // �δ����� Ÿ������ �� �Ҹ��� ����ϴ� AudioSource

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
        audioSource = GetComponent<AudioSource>();

        // OnHit �޼ҵ带 ObjectDetector Class�� raycastEvent�� �̺�Ʈ ���
        // ObjectDetector�� raycastEvent.Invoke(hit.transform); �޼ҵ尡 
        // ȣ��� ������ OnHit(Transform target) �޼ҵ尡 ȣ���
        objectDetector.raycastEvent.AddListener(OnHit);
    }

    private void OnHit(Transform target)
    {
        if (target.CompareTag("Mole"))
        {
            MoleFSM mole = target.GetComponent<MoleFSM>();

            // �δ����� Ȧ �ȿ� ���� ���� ���� �Ұ�
            if (mole.MoleState == MoleState.UnderGround)
                return;

            // ��ġ�� ��ġ ����
            transform.position = new Vector3(target.position.x, minY, target.position.z);

            // ��ġ�� �¾ұ� ������ �δ����� ���¸� �ٷ� "UnderGround"�� ����
            mole.ChangeState(MoleState.UnderGround);

            // ī�޶� ����
            ShakeCamera.Instance.OnShakeCamera(0.1f, 0.1f);

            // �δ��� Ÿ�� ȿ�� ���� (Particle�� ������ �δ��� ����� �����ϰ� ����)
            GameObject clone = Instantiate(moleHitEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem.MainModule main = clone.GetComponent<ParticleSystem>().main;
            main.startColor = mole.GetComponent<MeshRenderer>().material.color;

            // �δ��� ���� ���� ó�� (����, �ð�, ���� ���)
            MoleHitProcess(mole);

            // ��ġ�� ���� �̵���Ű�� �ڷ�ƾ ����
            StartCoroutine("MoveUp");
        }
    }

    private IEnumerator MoveUp()
    {
        // �̵� ���� (0, 1, 0) [��]
        movement3D.MoveTo(Vector3.up);

        while (true)
        {
            if (transform.position.y >= maxY)
            {
                movement3D.MoveTo(Vector3.zero);

                break;
            }

            yield return null;
        }
    }

    private void MoleHitProcess(MoleFSM mole)
    {
        if (mole.MoleType == MoleType.Normal)
        {
            gameController.NormalMoleHitCount++;    // �⺻ �δ��� Ÿ�� Ƚ�� 1 ����
            gameController.Combo++;
            // �⺻ ���� 50�� 10�޺��� 0.5�� ���Ѵ�.
            float scoreMultiple = 1 + gameController.Combo / 10 * 0.5f;
            int getScore = (int)(scoreMultiple * 50);

            // �޺��� ������ ���� ���� getScore�� Score�� ���Ѵ�.
            gameController.Score += getScore;

            moleHitTextViewer[mole.MoleIndex].OnHit("Score +" + getScore, Color.white);
        }
        else if (mole.MoleType == MoleType.Red)
        {
            gameController.RedMoleHitCount++;    // ���� �δ��� Ÿ�� Ƚ�� 1 ����
            gameController.Combo = 0;
            gameController.Score -= 300;

            moleHitTextViewer[mole.MoleIndex].OnHit("Score -300", Color.red);
        }
        else if (mole.MoleType == MoleType.Blue)
        {
            gameController.BlueMoleHitCount++;    // �Ķ� �δ��� Ÿ�� Ƚ�� 1 ����
            gameController.Combo++;
            gameController.CurrentTime += 3;

            moleHitTextViewer[mole.MoleIndex].OnHit("Time +3", new Color(0, 0.5f, 0.5f, 1));
        }
        else if (mole.MoleType == MoleType.Gold)
        {
            gameController.GoldMoleHitCount++;    // ��� �δ��� Ÿ�� Ƚ�� 1 ����
            gameController.Combo++;
            // �⺻ ���� 500�� 30�޺��� 0.5�� ���Ѵ�.
            float scoreMultiple = 1 + gameController.Combo / 30 * 0.5f;
            int getScore = (int)(scoreMultiple * 500);

            // �޺��� ������ ���� ���� getScore�� Score�� ���Ѵ�.
            gameController.Score += getScore;

            moleHitTextViewer[mole.MoleIndex].OnHit("Score +" + getScore, Color.yellow);
        }

        // ���� ��� (Normal = 0, Red = 1, Blue = 2, Gold = 3)
        PlaySound((int)mole.MoleType);
    }

    private void PlaySound(int index)
    {
        audioSource.Stop();
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }
}
