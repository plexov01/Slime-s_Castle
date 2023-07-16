using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager instance = null;

    public PlayerInput playerInput;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("Slime").Enable();
        playerInput.actions.FindActionMap("UI").Enable();
        playerInput.actions.FindActionMap("UI_default").Enable();
    }

    public void SaveActionsInPlayerInput()
    {
        string path = Application.streamingAssetsPath + "/SavedActionsPlayerInput.json";
        string data = playerInput.actions.ToJson();
        File.WriteAllText(path,data);
    }
    
    public void LoadActionsInPlayerInput()
    {
        string path = Application.streamingAssetsPath + "/SavedActionsPlayerInput.json";
        string data = playerInput.actions.ToJson();
        File.WriteAllText(path,data);
    }


}
