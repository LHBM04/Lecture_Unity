using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    /// <summary>
    /// 게임 매니저.
    /// </summary>
    [Tooltip("게임 매니저.")]
    [SerializeField]
    private GameManager manager;

    private void Reset()
    {
        manager = FindFirstObjectByType<GameManager>();
    }

    private void Awake()
    {
        manager = manager ?? FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌하면 점수를 증가시키고 아이템을 비활성화
        if (other.CompareTag("Player"))
        {
            manager.LeftItem--;
            gameObject.SetActive(false);
        }
    }
}