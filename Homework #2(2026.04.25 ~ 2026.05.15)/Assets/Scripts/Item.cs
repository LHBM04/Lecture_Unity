using UnityEngine;

/// <summary>
/// 아이템을 구현합니다.
/// </summary>
public class Item : MonoBehaviour
{
    private GameManager manager;

    [SerializeField]
    private string playerTag;

    private void Reset()
    {
        playerTag = "Player";
    }

    private void Awake()
    {
        manager = FindFirstObjectByType<GameManager>();

        if (string.IsNullOrEmpty(playerTag))
        {
#if UNITY_EDITOR
            Debug.LogWarning("Player Tag 필드가 할당되지 않았습니다!");
#endif
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            manager.ItemCount--;
            gameObject.SetActive(false);
        }
    }
}