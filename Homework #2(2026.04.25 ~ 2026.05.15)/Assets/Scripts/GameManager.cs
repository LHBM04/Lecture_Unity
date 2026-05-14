using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class GameManager : MonoBehaviour
{
    private int _itemCount;

    public int itemCount
    {
        get
        {
            return _itemCount;
        }
        set
        {
            _itemCount = value;

#if UNITY_EDITOR
            Debug.Log(_itemCount <= 0
                ? "축하합니다! 모든 아이템을 모았습니다!"
                : $"남은 아이템 개수: {_itemCount}개");
#endif
        }
    }

    private bool _isPaused;

    [SerializeField]
    private float _quitHoldTime = 1.0f;

    private float _quitTimer;
    private bool _isQuitPressed;

    private void Reset()
    {
        _quitHoldTime = 1.0f;
    }

    private void Awake()
    {
        _itemCount = FindObjectsByType<Item>(FindObjectsSortMode.None).Length;
        _quitTimer = _quitHoldTime;

#if UNITY_EDITOR
        Debug.Log($"감지한 아이템 개수: {_itemCount}개");
#endif
    }

    private void Update()
    {
        HandleQuitHold();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        SetPaused(!_isPaused);
    }

    public void OnQuit(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            _isQuitPressed = true;
        }
        else if (context.canceled)
        {
            _isQuitPressed = false;
            _quitTimer = _quitHoldTime;
        }
    }

    private void HandleQuitHold()
    {
        if (!_isQuitPressed)
        {
            return;
        }

        if ((_quitTimer -= Time.unscaledDeltaTime) > 0.0f)
        {
            return;
        }

        QuitGame();

        _isQuitPressed = false;
        _quitTimer = _quitHoldTime;
    }

    private void SetPaused(bool paused)
    {
        _isPaused = paused;
        Time.timeScale = _isPaused ? 0.0f : 1.0f;

#if UNITY_EDITOR
        Debug.Log(_isPaused ? "게임이 일시정지되었습니다." : "게임이 재개되었습니다.");
#endif
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("게임을 종료합니다.");
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        Time.timeScale = 1.0f;
    }
}