using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 게임 내 흐름을 제어합니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    private int itemCount;
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
#if UNITY_EDITOR
                Debug.Log("축하합니다! 모든 아이템을 모았습니다!");
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log($"남은 아이템 개수: {itemCount}개");
#endif
            }
        }
    }

    [SerializeField]
    private InputActionReference pauseAction;

    [SerializeField]
    private InputActionReference quitAction;

    private bool isPaused;

    [SerializeField]
    private float quitTime;
    private float quitTimer;

    private void Reset()
    {
        quitTime = 1.0f;
    }

    private void Awake()
    {
        itemCount = FindObjectsByType<Item>(FindObjectsSortMode.None).Length;
#if UNITY_EDITOR
        Debug.Log($"감지한 아이템 개수: {itemCount}개");
#endif

        quitTimer = quitTime;
    }

    private void OnEnable()
    {
        pauseAction.action.performed += OnPause;
        pauseAction.action.Enable();

        quitAction.action.performed += OnQuit;
        quitAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPause;
        pauseAction.action.Disable();

        quitAction.action.performed -= OnQuit;
        quitAction.action.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;

#if UNITY_EDITOR
            Debug.Log("게임이 일시정지되었습니다.");
#endif
        }
        else
        {
            Time.timeScale = 1f;

#if UNITY_EDITOR
            Debug.Log("게임이 재개되었습니다.");
#endif
        }
    }

    private void OnQuit(InputAction.CallbackContext context)
    {
        if (context.performed || context.started)
        {
            if ((quitTimer -= Time.deltaTime) <= 0f)
            {
#if UNITY_EDITOR
                Debug.Log("게임을 종료합니다.");
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
        else
        {
            quitTimer = quitTime;
        }
    }
}