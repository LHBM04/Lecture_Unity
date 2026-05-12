using UnityEngine;

/// <summary>
/// 아이템을 구현합니다.
/// </summary>
public class Item : MonoBehaviour
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
        if (other.CompareTag("Player"))
        {
            manager.ItemCount--;
            gameObject.SetActive(false);
        }
    }
}