using UnityEngine;
using UnityEngine.InputSystem;

public class scr_pipeGrate : scr_grate_abstract
{
    private scr_pipeGrate()
    {
        slimeSlipAnimationName = "pipeGrateSlipAnimation";
    }

    protected override void Interact(InputAction.CallbackContext context)
    {
        if (playerIsClose && 
            scr_cnpt_FormBehavior.instance._currentForm.GetType().ToString() 
            == "scr_SlimeForm")
        {
            InputManager.instance.playerInput.actions.FindActionMap("Slime").Disable();
            scr_Player.instance.GetComponent<Animator>().Play(slimeSlipAnimationName);

            
            if (scr_SlimeForm.isPipeCrawling)
            {
                scr_CameraManager.instance.SwitchCameraState(Cameras.main);
            }
            else
            {
                scr_CameraManager.instance.SwitchCameraState(Cameras.pipe);
            }
            scr_SlimeForm.isPipeCrawling = !scr_SlimeForm.isPipeCrawling;
            scr_cnpt_FormBehavior.canChangeForm = !scr_cnpt_FormBehavior.canChangeForm;

            StartCoroutine(Teleport());
        }
    }

}
