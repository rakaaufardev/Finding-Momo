using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FMSurfCharacterActionMachine_GameOver : FMCharacterActionState
{
    private FMSurfCharacter surfCharacter;

    public override void Init(VDCharacter inCharacter)
    {
        if (!isInitialized)
        {
            isInitialized = true;

            surfCharacter = inCharacter as FMSurfCharacter;
            inputController = surfCharacter.GetInputController();
            actionMachine = surfCharacter.GetActionMachine();
        }
    }

    public override IEnumerator Enter(object prev, params object[] values)
    {
        FMUIWindowController.Get.CloseWindow();
        Transform centerCamera = surfCharacter.Camera.CenterCamera;

        surfCharacter.OnGameOver();

        surfCharacter.RootFoamEffect.gameObject.SetActive(false);
        surfCharacter.TransformSurfboard.gameObject.SetActive(false);
        surfCharacter.ResetAnimation("Up");
        surfCharacter.ResetAnimation("Down");
        surfCharacter.ResetAnimation("Idle");
        surfCharacter.ResetAnimation("Jump");
        surfCharacter.TriggerAnimation("Fall");

        surfCharacter.RootVisual.SetParent(centerCamera);
        surfCharacter.RootVisual.DOLocalMove(Vector3.down * 5f, 0.25f);
        surfCharacter.RootVisual.DOScale(Vector3.one * 3f, 0.25f);

        yield return new WaitForSeconds(.25f);

        surfCharacter.RootVisual.DOLocalMove(Vector3.down * 3f, 0.75f);

        yield return new WaitForSeconds(0.75f);

        surfCharacter.RootVisual.DOLocalMove(Vector3.down * 20f, 0.5f);
        FMSceneController.Get().PlayParticle("WorldParticle_Splash_x3", centerCamera, Vector3.down * 6.5f);
        FMSoundController.Get().PlaySFX(SFX.SFX_Splash);

        yield return new WaitForSeconds(0.5f);
        GameObject.Destroy(surfCharacter.RootVisual.gameObject);
    }

    public override void DoUpdate()
    {

    }

    public override void DoFixedUpdate()
    {

    }

    public override IEnumerator Exit(object next)
    {
        yield return null;
    }
}
