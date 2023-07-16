using UnityEngine;
using UnityEngine.InputSystem;

public class Scr_cnpt_BaseInteraction : MonoBehaviour
{ 
    [SerializeField]scr_cnpt_FormBehavior formBehavior;

    InputManager input;

    private void Awake()
    {
        input = InputManager.instance;
        
        
        input.playerInput.actions["PickObject"].performed += PickObject;
        
        //input.playerInput.actions["Skill_3"].performed += Skill_3;

    }

    private void PickObject(InputAction.CallbackContext context)
    {
        formBehavior._currentForm.PickObject();
    }
    
    private void Skill_3(InputAction.CallbackContext context)
    {
        formBehavior._currentForm.Skill_3();
    }



    private void OnDestroy()
    {
       
        input.playerInput.actions["PickObject"].performed -= PickObject;
        //input.playerInput.actions["Skill_3"].performed -= Skill_3;

    }
}


