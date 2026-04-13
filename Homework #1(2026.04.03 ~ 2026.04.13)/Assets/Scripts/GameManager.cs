using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 찾아야 할 아이템의 개수.
    /// </summary> 
    private int itemCount;

    /// <summary>
    /// 찾아야 할 아이템의 개수.
    /// </summary>
    public int ItemCount
    {
        get 
        { 
            return itemCount; 
        }
        set
        {
            if ((itemCount = value) <= 0)
            {
                Debug.Log("축하합니다! 모든 아이템을 모았습니다!");
            }
            else
            {
                Debug.Log($"남은 아이템 개수: {itemCount}개");
            }
        }
    }

    /// <summary>
    /// 종료 감지를 위한 키 누름 시간.
    /// </summary>
    [Tooltip("종료 감지를 위한 키 누름 시간")]
    [SerializeField]
    private float quitTime;

    /// <summary>
    /// 종료 감지를 위한 키 누름 시간 타이머.
    /// </summary>
    private float quitTimer;

    private void Reset()
    {
        quitTime = 2.0f;
    }

    private void Awake()
    {
        itemCount = FindObjectsOfType<Item>().Length;
        Debug.Log($"감지한 아이템 개수: {itemCount}개");

        quitTimer = quitTime;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            quitTimer -= Time.deltaTime;
            Debug.Log($"종료까지 남은 시간: {quitTimer:F2}초");
        }
        else
        {
            quitTimer = quitTime;
        }
    }

    private void LateUpdate()
    {
        if (quitTimer <= 0f)
        {
            Debug.Log("게임을 종료합니다.");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}