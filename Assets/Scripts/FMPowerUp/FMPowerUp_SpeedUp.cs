using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FMPowerUp_SpeedUp : FMPowerUp
{
    public override void PowerUpEffect(string value)
    {
        float speedIncreament = Convert.ToInt32(value);

        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene scene = currentScene as FMMainScene;
        FMWorld currentWorld = scene.GetCurrentWorldObject();
        FMMainCharacter character = currentWorld.GetCharacter() as FMMainCharacter;

        character.Speed += speedIncreament;
    }
}
