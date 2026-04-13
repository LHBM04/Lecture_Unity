using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 점수.
    /// </summary> 
    [Tooltip("점수.")]
    [SerializeField]
    private int leftItem;

    /// <summary>
    /// 점수.
    /// </summary>
    public int LeftItem
    {
        get 
        { 
            return leftItem; 
        }
        set
        {
            if ((leftItem = value) <= 0)
            {
                Debug.Log("축하합니다! 모든 아이템을 모았습니다!");
            }
            else
            {
                Debug.Log($"남은 아이템 개수: {leftItem}");
            }
        }
    }

    private void Reset()
    {
        leftItem = FindObjectsOfType<ScoreItem>().Length;
    }

    private void Awake()
    {
        leftItem = FindObjectsOfType<ScoreItem>().Length;
    }
}