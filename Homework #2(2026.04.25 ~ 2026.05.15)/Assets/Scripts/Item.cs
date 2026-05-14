using UnityEngine;

/// <summary>
/// 아이템을 구현합니다.
/// </summary>
public class Item : MonoBehaviour
{
    private GameManager _manager;
    private bool _isCollected;

    [SerializeField]
    private string playerTag;

    private void Reset()
    {
        playerTag = "Player";
    }

    private void Awake()
    {
        _manager = FindFirstObjectByType<GameManager>();

        if (string.IsNullOrEmpty(playerTag))
        {
#if UNITY_EDITOR
            Debug.LogWarning("Player Tag 필드가 할당되지 않았습니다!");
#endif
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isCollected || string.IsNullOrEmpty(playerTag) || !other.CompareTag(playerTag))
        {
            return;
        }

        _isCollected = true;

        if (_manager != null)
        {
            --_manager.itemCount;
        }

        gameObject.SetActive(false);
    }
}
