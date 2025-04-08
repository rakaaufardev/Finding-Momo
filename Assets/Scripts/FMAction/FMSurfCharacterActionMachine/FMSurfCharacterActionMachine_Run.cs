using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMSurfCharacterActionMachine_Run : FMCharacterActionState
{
    private FMSurfCharacter surfCharacter;
    private FMCameraController cameraController;
    private bool isJump;
    private bool isHighJump;
    private float jumpForce;

    public bool IsHighJump 
    {
        get
        {
            return isHighJump;
        }
        set
        {
            isHighJump = value;
        }
    }

    public override void Init(VDCharacter inCharacter)
    {
        if (!isInitialized)
        {
            isInitialized = true;

            surfCharacter = inCharacter as FMSurfCharacter;
            inputController = surfCharacter.GetInputController();
            actionMachine = surfCharacter.GetActionMachine();
        }

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        SurfWorld surfWorld = mainScene.GetCurrentWorldObject() as SurfWorld;
        cameraController = surfWorld.CameraController;
    }

    public override IEnumerator Enter(object prev, params object[] values)
    {
        surfCharacter.ResetAnimation("Idle");
        surfCharacter.TriggerAnimation("Down");

        yield return null;
    }

    public override void DoUpdate()
    {
        if (inputController.IsSurfUp() && !isJump)
        {
            if (surfCharacter.transform.position.y < VDParameter.CHARACTER_MAX_SURF)
            {
                surfCharacter.ResetAnimation("Down");
                surfCharacter.ResetAnimation("Jump");
                surfCharacter.TriggerAnimation("Up");
            }
        }
        else
        {
            if (surfCharacter.transform.position.y > VDParameter.CHARACTER_MIN_SURF)
            {
                if (isJump)
                {
                    if (surfCharacter.transform.position.y < VDParameter.CHARACTER_MAX_SURF)
                    {
                        surfCharacter.ResetAnimation("Up");
                        surfCharacter.ResetAnimation("Jump");
                        surfCharacter.TriggerAnimation("Down");
                    }
                }
            }
        }

        if (isJump)
        {
            if (surfCharacter.transform.position.y >= VDParameter.SWITCH_CAMERA_JUMP_POSITION)
            {
                surfCharacter.ResetAnimation("Up");
                surfCharacter.TriggerAnimation("Jump");
            }
        }
        else
        {
            if (inputController.IsSurfDown())
            {
                surfCharacter.ResetAnimation("Up");
                surfCharacter.ResetAnimation("Jump");
                surfCharacter.TriggerAnimation("Down");
            }
        }
    }

    public override void DoFixedUpdate()
    {
        float velocityX = surfCharacter.CharacterRigidBody.velocity.x;
        float velocityZ = surfCharacter.CharacterRigidBody.velocity.z;

        if (inputController.IsSurfUp() && !isJump)
        {
            if (surfCharacter.transform.position.y < VDParameter.CHARACTER_MAX_SURF)
            {
                surfCharacter.CharacterRigidBody.velocity += new Vector3(velocityX, VDParameter.CHARACTER_SURFING_VALUE * Time.deltaTime, velocityZ);

                float normalizedTime = Mathf.Clamp01(Time.deltaTime);
                float easingFactor = 1 - Mathf.Sqrt(1 - Mathf.Pow(normalizedTime, 2));
                jumpForce += VDParameter.CHARACTER_THRESHOLD_JUMP_FORCE_SURF * easingFactor;
            }
            else
            {
                isJump = true;
                surfCharacter.CharacterRigidBody.velocity += new Vector3(velocityX, jumpForce * VDParameter.CHARACTER_MULTIPLIER_JUMP_FORCE_SURF * Time.deltaTime, velocityZ);
            }
        }
        else
        {
            if (surfCharacter.transform.position.y > VDParameter.CHARACTER_MIN_SURF)
            {
                if (isJump)
                {
                    if (surfCharacter.transform.position.y < VDParameter.CHARACTER_MAX_SURF)
                    {
                        jumpForce = 0f;
                        isJump = false;
                        isHighJump = false;
                        surfCharacter.CharacterRigidBody.velocity = new Vector3(velocityX, VDParameter.CHARACTER_MIN_SURF, velocityZ);
                        cameraController.ActivateCamera(SurfCameraMode.SideView);
                        surfCharacter.RootFoamEffect.gameObject.SetActive(true);

                        FMSceneController.Get().PlayParticle("WorldParticle_Splash", surfCharacter.transform, Vector3.zero);
                        FMSoundController.Get().PlaySFX(SFX.SFX_Splash);
                    }
                }

                if (isHighJump)
                {
                    surfCharacter.CharacterRigidBody.velocity += new Vector3(velocityX, -(VDParameter.CHARACTER_GRAVITY_HIGH_FALL_SURF * Time.deltaTime), velocityZ);

                    if (inputController.IsQuickLandPressed())
                    {
                        surfCharacter.QuickFall();
                    }
                }
                else if (isJump)
                {
                    surfCharacter.CharacterRigidBody.velocity += new Vector3(velocityX, -(VDParameter.CHARACTER_GRAVITY_FALL_SURF * Time.deltaTime), velocityZ);
                }
                else
                {
                    surfCharacter.CharacterRigidBody.velocity += new Vector3(velocityX, -(VDParameter.CHARACTER_SURFING_VALUE * Time.deltaTime), velocityZ);
                }
            }
            else
            {
                surfCharacter.CharacterRigidBody.velocity = new Vector3(velocityX, VDParameter.CHARACTER_MIN_SURF, velocityZ);
            }
        }


        if (isJump)
        {
            if (surfCharacter.transform.position.y >= VDParameter.SWITCH_CAMERA_JUMP_POSITION)
            {
                isHighJump = true;
                cameraController.ActivateCamera(SurfCameraMode.JumpView);
                surfCharacter.RootFoamEffect.gameObject.SetActive(false);
            }
        }
        else
        {
            if (inputController.IsSurfDown())
            {
                jumpForce = 0f;
            }
        }

        surfCharacter.Move();
    }

    public override IEnumerator Exit(object next)
    {
        yield return null;
    }
}
