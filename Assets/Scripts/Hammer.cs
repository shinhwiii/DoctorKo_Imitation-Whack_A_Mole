using System.Collections;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField]
    private float maxY;                                 // 망치의 최대 y 위치
    [SerializeField]
    private float minY;                                 // 망치의 최소 y 위치
    [SerializeField]
    private GameObject moleHitEffectPrefab;             // 두더지 타격 효과 프리펩
    [SerializeField]
    private AudioClip[] audioClips;                     // 두더지를 타격했을 때 재생되는 사운드
    [SerializeField]
    private MoleHitTextViewer[] moleHitTextViewer;      // 타격한 두더지 위치에 타격 정보 텍스트 출력
    [SerializeField]
    private GameController gameController;              // 점수 증가를 위한 GameController
    [SerializeField]
    private ObjectDetector objectDetector;              // 마우스 클릭으로 오브젝트 선택을 위한 ObjectDetector
    private Movement3D movement3D;                      // 망치 오브젝트 이동을 위한 Movement3D
    private AudioSource audioSource;                    // 두더지를 타격했을 때 소리를 재생하는 AudioSource

    private void Awake()
    {
        movement3D = GetComponent<Movement3D>();
        audioSource = GetComponent<AudioSource>();

        // OnHit 메소드를 ObjectDetector Class의 raycastEvent에 이벤트 등록
        // ObjectDetector의 raycastEvent.Invoke(hit.transform); 메소드가 
        // 호출될 때마다 OnHit(Transform target) 메소드가 호출됨
        objectDetector.raycastEvent.AddListener(OnHit);
    }

    private void OnHit(Transform target)
    {
        if (target.CompareTag("Mole"))
        {
            MoleFSM mole = target.GetComponent<MoleFSM>();

            // 두더지가 홀 안에 있을 때는 공격 불가
            if (mole.MoleState == MoleState.UnderGround)
                return;

            // 망치의 위치 설정
            transform.position = new Vector3(target.position.x, minY, target.position.z);

            // 망치에 맞았기 때문에 두더지의 상태를 바로 "UnderGround"로 설정
            mole.ChangeState(MoleState.UnderGround);

            // 카메라 흔들기
            ShakeCamera.Instance.OnShakeCamera(0.1f, 0.1f);

            // 두더지 타격 효과 생성 (Particle의 색상을 두더지 색상과 동일하게 설정)
            GameObject clone = Instantiate(moleHitEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem.MainModule main = clone.GetComponent<ParticleSystem>().main;
            main.startColor = mole.GetComponent<MeshRenderer>().material.color;

            // 두더지 색상에 따라 처리 (점수, 시간, 사운드 재생)
            MoleHitProcess(mole);

            // 망치를 위로 이동시키는 코루틴 생성
            StartCoroutine("MoveUp");
        }
    }

    private IEnumerator MoveUp()
    {
        // 이동 방향 (0, 1, 0) [위]
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
            gameController.NormalMoleHitCount++;    // 기본 두더지 타격 횟수 1 증가
            gameController.Combo++;
            // 기본 점수 50에 10콤보당 0.5씩 더한다.
            float scoreMultiple = 1 + gameController.Combo / 10 * 0.5f;
            int getScore = (int)(scoreMultiple * 50);

            // 콤보가 합쳐져 계산된 점수 getScore를 Score에 더한다.
            gameController.Score += getScore;

            moleHitTextViewer[mole.MoleIndex].OnHit("Score +" + getScore, Color.white);
        }
        else if (mole.MoleType == MoleType.Red)
        {
            gameController.RedMoleHitCount++;    // 빨간 두더지 타격 횟수 1 증가
            gameController.Combo = 0;
            gameController.Score -= 300;

            moleHitTextViewer[mole.MoleIndex].OnHit("Score -300", Color.red);
        }
        else if (mole.MoleType == MoleType.Blue)
        {
            gameController.BlueMoleHitCount++;    // 파란 두더지 타격 횟수 1 증가
            gameController.Combo++;
            gameController.CurrentTime += 3;

            moleHitTextViewer[mole.MoleIndex].OnHit("Time +3", new Color(0, 0.5f, 0.5f, 1));
        }
        else if (mole.MoleType == MoleType.Gold)
        {
            gameController.GoldMoleHitCount++;    // 골드 두더지 타격 횟수 1 증가
            gameController.Combo++;
            // 기본 점수 500에 30콤보당 0.5씩 더한다.
            float scoreMultiple = 1 + gameController.Combo / 30 * 0.5f;
            int getScore = (int)(scoreMultiple * 500);

            // 콤보가 합쳐져 계산된 점수 getScore를 Score에 더한다.
            gameController.Score += getScore;

            moleHitTextViewer[mole.MoleIndex].OnHit("Score +" + getScore, Color.yellow);
        }

        // 사운드 재생 (Normal = 0, Red = 1, Blue = 2, Gold = 3)
        PlaySound((int)mole.MoleType);
    }

    private void PlaySound(int index)
    {
        audioSource.Stop();
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }
}
