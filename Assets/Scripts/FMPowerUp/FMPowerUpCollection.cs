using System;
using System.Collections.Generic;
public class FMPowerUpCollection
{
    public static Dictionary<string, PowerUpData> powerUpList = new Dictionary<string, PowerUpData>()
{
        { "SpeedUp", new PowerUpData("2", "0", (string val) => new FMPowerUp_SpeedUp().PowerUpEffect(val)) },
};
}
public enum PowerUpType
{
SpeedUp,
}