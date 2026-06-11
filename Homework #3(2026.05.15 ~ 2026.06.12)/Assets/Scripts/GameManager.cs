using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<GameManager>();
                    singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";
                }
            }

            return instance;
        }
    }

    public PlayerController player;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private Transform ballTransform;

    [Header("Spots")]
    [SerializeField]
    private SpotData dribbleSpots;

    [SerializeField]
    private SpotData freeKickSpots;

    [SerializeField]
    private SpotData penaltyKickSpot;

    public enum GameMode
    {
        Dribble,
        FreeKick,
        PenaltyKick
    }

    private GameMode currentGameMode;
    public GameMode CurrentGameMode => currentGameMode;

    private PlayerInputActions inputActions;
    private InputAction restartAction;
    private InputAction dribbleModeAction;
    private InputAction freeKickModeAction;
    private InputAction penaltyKickModeAction;

    private void Reset()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        ballTransform = GameObject.FindGameObjectWithTag("Ball").transform;
    }

    private void Awake()
    {
        playerTransform = playerTransform ?? GameObject.FindGameObjectWithTag("Player").transform;
        ballTransform = ballTransform ?? GameObject.FindGameObjectWithTag("Ball").transform;

        inputActions = new PlayerInputActions();
        restartAction = inputActions.System.Restart;
        dribbleModeAction = inputActions.System.DribbleMode;
        freeKickModeAction = inputActions.System.FreeKickMode;
        penaltyKickModeAction = inputActions.System.PenaltyKickMode;
    }

    private void OnEnable()
    {
        restartAction.Enable();
        restartAction.performed += Restart;

        dribbleModeAction.Enable();
        dribbleModeAction.performed += ChangeDribbleMode;

        freeKickModeAction.Enable();
        freeKickModeAction.performed += ChangeFreekickMode;

        penaltyKickModeAction.Enable();
        penaltyKickModeAction.performed += ChangePenaltyKickMode;
    }
    private void Start()
    {
        playerTransform.position = dribbleSpots.playerSpot.position;
        ballTransform.position = dribbleSpots.ballSpot.position;
    }

    private void OnDisable()
    {
        restartAction.Disable();
        restartAction.performed -= Restart;

        dribbleModeAction.Disable();
        dribbleModeAction.performed -= ChangeDribbleMode;

        freeKickModeAction.Disable();
        freeKickModeAction.performed -= ChangeFreekickMode;

        penaltyKickModeAction.Disable();
        penaltyKickModeAction.performed -= ChangePenaltyKickMode;
    }

    private void Restart(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Game");
    }

    private void ChangeDribbleMode(InputAction.CallbackContext context)
    {
        currentGameMode = GameMode.Dribble;

        playerTransform.position = new Vector3(
            dribbleSpots.playerSpot.position.x,
            playerTransform.position.y,
            dribbleSpots.playerSpot.position.z
        );

        ballTransform.position = new Vector3(
             dribbleSpots.ballSpot.position.x,
             playerTransform.position.y,
             dribbleSpots.ballSpot.position.z
        );

        if (playerTransform.TryGetComponent(out PlayerController playerController))
        {
            playerController.ChangeMode(currentGameMode);
        }
    }

    private void ChangeFreekickMode(InputAction.CallbackContext context)
    {
        currentGameMode = GameMode.FreeKick;

        playerTransform.position = new Vector3(
            freeKickSpots.playerSpot.position.x,
            playerTransform.position.y,
            freeKickSpots.playerSpot.position.z
        );

        ballTransform.position = new Vector3(
             freeKickSpots.ballSpot.position.x,
             playerTransform.position.y,
             freeKickSpots.ballSpot.position.z
        );

        if (playerTransform.TryGetComponent(out PlayerController playerController))
        {
            playerController.ChangeMode(currentGameMode);
        }
    }

    private void ChangePenaltyKickMode(InputAction.CallbackContext context)
    {
        currentGameMode = GameMode.PenaltyKick;

        playerTransform.position = new Vector3(
             penaltyKickSpot.playerSpot.position.x,
             playerTransform.position.y,
             penaltyKickSpot.playerSpot.position.z
         );

        ballTransform.position = new Vector3(
             penaltyKickSpot.ballSpot.position.x,
             playerTransform.position.y,
             penaltyKickSpot.ballSpot.position.z
        );

        if (playerTransform.TryGetComponent(out PlayerController playerController))
        {
            playerController.ChangeMode(currentGameMode);
        }
    }
}
