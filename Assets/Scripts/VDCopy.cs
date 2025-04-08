using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CopyTag
{
    SIDE_VIEW_TRANSITION_GET_READY,
    SIDE_VIEW_TRANSITION_PRESS,
    BACK_VIEW_TRANSITION_PRESS,
    TUTORIAL_INTRO,
    TUTORIAL_COIN_GARBAGE,
    TUTORIAL_COIN_HEALTH,
    TUTORIAL_JUMP,
    TUTORIAL_TRAMPOLINE,
    TUTORIAL_DOUBLE_JUMP,
    TUTORIAL_SLIDE,
    TUTORIAL_TRANSITION_BACK_VIEW,
    TUTORIAL_PRESS_TRANSITION_FORCE,
    TUTORIAL_MOVE_RIGHT,
    TUTORIAL_MOVE_LEFT,
    TUTORIAL_JUMP_BACK_SAND,
    TUTORIAL_DOUBLE_JUMP_BACK_SAND,
    TUTORIAL_SLIDE_BACK_SAND,
    TUTORIAL_MINIWHEEL,
    TUTORIAL_MINIWHEEL_STOP,
    TUTORIAL_RHYTHM,
    TUTORIAL_MANHOLE,
    TUTORIAL_COLLECTIBLE,
    TUTORIAL_INK,
    TUTORIAL_BOMB,
    TUTORIAL_SLIDE_SIDE_CITY,
    TUTORIAL_TRANSITION_BACK_VIEW_CITY,
    TUTORIAL_PORTAL,
    TUTORIAL_TRASHBIN_LEFT,
    TUTORIAL_TRASHBIN_MIDDLE,
    TUTORIAL_TRASHBIN_RIGHT,
    TUTORIAL_JUMP_BACK_CITY,
    TUTORIAL_TRUCK,
    TUTORIAL_SLIDE_BACK_CITY,
    TUTORIAL_FINISH,
    TUTORIAL_SURF_INTRO,
    TUTORIAL_SURF_ANIMALS,
    TUTORIAL_SURF_MISSIONS,
    TUTORIAL_SURF_CONTROLS,
    TUTORIAL_SURF_END,
    COSTUME_OWNED,
    COSTUME_EQUIP,
    SURFBOARD_OWNED,
    SURFBOARD_EQUIP,
    DAILY_REWARD_CLAIM,
    DAILY_REWARD_CLAIMED,
    QTA_EXPLANATION
}

public class VDCopy
{
    //value use list string for prepare localization language
    private static Dictionary<CopyTag, List<string>> copyDesktop = new Dictionary<CopyTag, List<string>>()
    {
        {
            CopyTag.SIDE_VIEW_TRANSITION_GET_READY,
            new List<string>()
            {
                "Get Ready Press <size=120%><sprite name=\"ArrowUp\" name=\"ArrowUp\"></size> To Change Track!<br><size=200%>{0}</size>",
            }
        },

        {
            CopyTag.SIDE_VIEW_TRANSITION_PRESS,
            new List<string>()
            {
                "Press <size=120%><sprite name=\"ArrowUp\" name=\"ArrowUp\"></size> Button To Change Track!",
            }
        },

        {
            CopyTag.BACK_VIEW_TRANSITION_PRESS,
            new List<string>()
            {
                "Press <size=120%><sprite name=\"ArrowRight\"></size> in the green circle!",
            }
        },

        {
            CopyTag.TUTORIAL_INTRO,
            new List<string>()
            {
                "Let's go! Press <sprite name=Space>  to start running!",
            }
        },

        {
            CopyTag.TUTORIAL_JUMP,
            new List<string>()
            {
                "Press <sprite name=ArrowUp> To Jump",
            }
        },

        {
            CopyTag.TUTORIAL_TRAMPOLINE,
            new List<string>()
            {
                "Try  jump on top of trampoline it will give you jump boost!!",
            }
        },
        
        {
            CopyTag.TUTORIAL_DOUBLE_JUMP,
            new List<string>()
            {
                "<sprite name=ArrowUp> X2 to Double Jump",
            }
        },
        
        {
            CopyTag.TUTORIAL_SLIDE,
            new List<string>()
            {
                "Hold <sprite name=ArrowDown> to Slide",
            }
        },
        
        {
            CopyTag.TUTORIAL_TRANSITION_BACK_VIEW,
            new List<string>()
            {
                "There's no way to run you need to change direction <br> Press <sprite name=ArrowUp> key to Change Direction! ",
            }
        },
        
        {
            CopyTag.TUTORIAL_PRESS_TRANSITION_FORCE,
            new List<string>()
            {
                "Now Press <sprite name=ArrowUp> key to change direction.  <b> Next Time  You can also choose another lane beside this one </b>",
            }
        },
        
        {
            CopyTag.TUTORIAL_MOVE_RIGHT,
            new List<string>()
            {
                "<color=#ff0000> You can't jump and walk on the sunbed! You will lose health. </color> <br> Press <sprite name=ArrowRight> to change direction to the right",
            }
        },
        
        {
            CopyTag.TUTORIAL_MOVE_LEFT,
            new List<string>()
            {
                "You can also Press <sprite name=ArrowLeft> To Move Left",
            }
        },
        
        {
            CopyTag.TUTORIAL_JUMP_BACK_SAND,
            new List<string>()
            {
                "Press <sprite name=ArrowUp> To Jump",
            }
        },
        
        {
            CopyTag.TUTORIAL_DOUBLE_JUMP_BACK_SAND,
            new List<string>()
            {
                "<sprite name=ArrowUp> X2 to Double Jump",
            }
        },
        
        {
            CopyTag.TUTORIAL_SLIDE_BACK_SAND,
            new List<string>()
            {
                "Hold <sprite name=ArrowDown> to Slide",
            }
        },
        
        {
            CopyTag.TUTORIAL_RHYTHM,
            new List<string>()
            {
                "When The Rhythm Appear Press <sprite name=ArrowRight> when white circle reach <b> <color=#008000> Green area </color></b>",
            }
        },
        
        {
            CopyTag.TUTORIAL_MANHOLE,
            new List<string>()
            {
                "<color=#ff0000>  CAREFUL!!! </color> there's a manhole a head you need to jump or you will lose all your health",
            }
        },
        
        {
            CopyTag.TUTORIAL_COLLECTIBLE,
            new List<string>()
            {
                "Collect Big Coin to earn extra <color=#FFD700><b>score</b></color>!",
            }
        },
        
        {
            CopyTag.TUTORIAL_INK,
            new List<string>()
            {
                "Watch out! If you bump into Arnold he'll shoot <color=#000000> INK</color> at you like this again! <sprite name=ArrowUp> X2 to Double Jump",
            }
        },
        
        {
            CopyTag.TUTORIAL_BOMB,
            new List<string>()
            {
                "Be careful! Don't hit <color=#ff0000> THE BOMB </color> or you'll lose health! Press <sprite name=ArrowUp> To Jump",
            }
        },
        
        {
            CopyTag.TUTORIAL_SLIDE_SIDE_CITY,
            new List<string>()
            {
                "Sometimes you have to choose either go Double Jump<sprite name=ArrowUp>  or Slide <sprite name=ArrowDown> ",
            }
        },
        
        {
            CopyTag.TUTORIAL_TRANSITION_BACK_VIEW_CITY,
            new List<string>()
            {
                "There's no way to run you need to change direction <br> Press <sprite name=ArrowUp> key to Change Direction! ",
            }
        },
        
        {
            CopyTag.TUTORIAL_PORTAL,
            new List<string>()
            {
                "Look! There's a portal! Let's get inside to get more fun!",
            }
        },
        
        {
            CopyTag.TUTORIAL_TRASHBIN_LEFT,
            new List<string>()
            {
                "<color=#ff0000> You can't jump and walk on the TrashBin! You will lose health. </color>",
            }
        },
        
        {
            CopyTag.TUTORIAL_JUMP_BACK_CITY,
            new List<string>()
            {
                "Press <sprite name=ArrowUp> To Jump",
            }
        },
        
        {
            CopyTag.TUTORIAL_TRUCK,
            new List<string>()
            {
                "You can Do Double Jump to get on top of the truck it's completly save sometimes there a collectible too on top! <sprite name=ArrowUp> X2 to Double Jump",
            }
        },
        
        {
            CopyTag.TUTORIAL_SLIDE_BACK_CITY,
            new List<string>()
            {
                "Hold <sprite name=ArrowDown> to Slide",
            }
        },
        
        {
            CopyTag.TUTORIAL_FINISH,
            new List<string>()
            {
                "The tutorial is now complete. <br> <size=150%> Good luck! </size>",
            }
        },
        
        {
            CopyTag.TUTORIAL_SURF_INTRO,
            new List<string>()
            {
                "Welcome to\nsurf game! <br>Here are mission that you must complete. Finish those all mission and got additional sweat reward!",
            }
        },
        
        {
            CopyTag.TUTORIAL_SURF_CONTROLS,
            new List<string>()
            {
                "Press and hold <sprite name=Space> To Surf Up<br>Release <sprite name=Space> To Surf Down",
            }
        },
        
        {
            CopyTag.COSTUME_EQUIP,
            new List<string>()
            {
                "Equip",
            }
        },

        {
            CopyTag.COSTUME_OWNED,
            new List<string>()
            {
                "Owned",
            }
        },

        {
            CopyTag.SURFBOARD_EQUIP,
            new List<string>()
            {
                "Equip",
            }
        },

        {
            CopyTag.SURFBOARD_OWNED,
            new List<string>()
            {
                "Owned",
            }
        },

        {
            CopyTag.DAILY_REWARD_CLAIM,
            new List<string>()
            {
                "Free Daily",
            }
        },

        {
            CopyTag.DAILY_REWARD_CLAIMED,
            new List<string>()
            {
                "Claimed",
            }
        },
    };

    private static Dictionary<CopyTag, List<string>> copyMobile = new Dictionary<CopyTag, List<string>>()
    {
        {
            CopyTag.SIDE_VIEW_TRANSITION_GET_READY,
            new List<string>()
            {
                "SWIPE UP in <br><size=200%>{0}</size>",
            }
        },

        {
            CopyTag.SIDE_VIEW_TRANSITION_PRESS,
            new List<string>()
            {
                "SWIPE UP!",
            }
        },
        
        {
            CopyTag.BACK_VIEW_TRANSITION_PRESS,
            new List<string>()
            {
                "TAP when the White Circle reaches the <b><color=#00FF00>Green Area</color></b>!",
            }
        },

        {
            CopyTag.TUTORIAL_INTRO,
            new List<string>()
            {
                "TAP to start!",
            }
        },

        {
            CopyTag.TUTORIAL_COIN_GARBAGE,
            new List<string>()
            {
                "<size=87.5%>Pick up the garbages!\nThey are bad for environment!</size>",
            }
        },
        
        {
            CopyTag.TUTORIAL_COIN_HEALTH,
            new List<string>()
            {
                "Fill all garbages for extra <b><color=#ff0000>HEALTH</color></b>!",
            }
        },
        
        {
            CopyTag.TUTORIAL_JUMP,
            new List<string>()
            {
                "Jump\nTap/Swipe Up",
            }
        },

        {
            CopyTag.TUTORIAL_TRAMPOLINE,
            new List<string>()
            {
                "Jump on the parasol!",
            }
        },

        {
            CopyTag.TUTORIAL_DOUBLE_JUMP,
            new List<string>()
            {
                "Double Jump\nx2 Swipe up / Tap",
            }
        },

        {
            CopyTag.TUTORIAL_SLIDE,
            new List<string>()
            {
                "Slide\nSwipe Down",
            }
        },

        {
            CopyTag.TUTORIAL_TRANSITION_BACK_VIEW,
            new List<string>()
            {
                "Change direction\nSwipe Up!",
            }
        },

        {
            CopyTag.TUTORIAL_PRESS_TRANSITION_FORCE,
            new List<string>()
            {
                "Swipe Up!",
            }
        },

        {
            CopyTag.TUTORIAL_MOVE_RIGHT,
            new List<string>()
            {
                "Go Right\nSwipe Right!",
            }
        },

        {
            CopyTag.TUTORIAL_MOVE_LEFT,
            new List<string>()
            {
                "Go Left\nSwipe Left!",
            }
        },

        {
            CopyTag.TUTORIAL_JUMP_BACK_SAND,
            new List<string>()
            {
                "Jump Tap/Swipe Up!",
            }
        },

        {
            CopyTag.TUTORIAL_DOUBLE_JUMP_BACK_SAND,
            new List<string>()
            {
                "Double Jump/nx2 Swipe up / Tap",
            }
        },

        {
            CopyTag.TUTORIAL_SLIDE_BACK_SAND,
            new List<string>()
            {
                "Slide Swipe Down!",
            }
        },

        {
            CopyTag.TUTORIAL_RHYTHM,
            new List<string>()
            {
                "Tap when the <b><color=#00FF00>White Circle</color> hits <b><color=#00FF00>Green</color>!",
            }
        },

        {
            CopyTag.TUTORIAL_MANHOLE,
            new List<string>()
            {
                "Avoid the manhole!\nIt drains health!",
            }
        },
        
        {
            CopyTag.TUTORIAL_MINIWHEEL,
            new List<string>()
            {
                "Get Big Garbage to play Wheel minigame!",
            }
        },
        
        {
            CopyTag.TUTORIAL_MINIWHEEL_STOP,
            new List<string>()
            {
                "Match the Big Garbage with the Wheel to get reward!",
            }
        },

        {
            CopyTag.TUTORIAL_COLLECTIBLE,
            new List<string>()
            {
                "Big Garbage Bonus <color=#FFD700><b>COINS</b></color>!",
            }
        },

        {
            CopyTag.TUTORIAL_INK,
            new List<string>()
            {
                "Dodge arnold!",
            }
        },

        {
            CopyTag.TUTORIAL_BOMB,
            new List<string>()
            {
                "Dodge The bomb!",
            }
        },

        {
            CopyTag.TUTORIAL_SLIDE_SIDE_CITY,
            new List<string>()
            {
                "Slide or Jump!",
            }
        },

        {
            CopyTag.TUTORIAL_TRANSITION_BACK_VIEW_CITY,
            new List<string>()
            {
                "SWIPE UP!",
            }
        },

        {
            CopyTag.TUTORIAL_PORTAL,
            new List<string>()
            {
                "A sea turtle needs help!\nEnter the portal!",
            }
        },

        {
            CopyTag.TUTORIAL_TRASHBIN_LEFT,
            new List<string>()
            {
                "Move <color=#ff0000><b>RIGHT</b></color>\nswipe <color=#ff0000><b>RIGHT</b></color>!",
            }
        },
        
        {
            CopyTag.TUTORIAL_TRASHBIN_MIDDLE,
            new List<string>()
            {
                "Don't go on top of trashbin!",
            }
        },
        
        {
            CopyTag.TUTORIAL_TRASHBIN_RIGHT,
            new List<string>()
            {
                "Go Left Swipe Left!",
            }
        },

        {
            CopyTag.TUTORIAL_JUMP_BACK_CITY,
            new List<string>()
            {
                "Jump Tap/Swipe Up!",
            }
        },

        {
            CopyTag.TUTORIAL_TRUCK,
            new List<string>()
            {
                "Try running on the truck!",
            }
        },

        {
            CopyTag.TUTORIAL_SLIDE_BACK_CITY,
            new List<string>()
            {
                "Slide Swipe Down!",
            }
        },

        {
            CopyTag.TUTORIAL_FINISH,
            new List<string>()
            {
                "The Tutorial is over!",
            }
        },

        {
            CopyTag.TUTORIAL_SURF_INTRO,
            new List<string>()
            {
                "Welcome to the\n<size=150%>Surf Minigame!</size>",
            }
        },
        
        {
            CopyTag.TUTORIAL_SURF_ANIMALS,
            new List<string>()
            {
                "Rescue the sea turtle!",
            }
        },
        
        {
            CopyTag.TUTORIAL_SURF_MISSIONS,
            new List<string>()
            {
                "Complete other mission get bonus score!",
            }
        },

        {
            CopyTag.TUTORIAL_SURF_CONTROLS,
            new List<string>()
            {
                "Surf up <color=#ff0000><b>Tap</b></color> and <color=#ff0000><b>Hold</b></color>\nSurf Down <color=#ff0000><b>Release</b></color>",
            }
        },

        {
            CopyTag.TUTORIAL_SURF_END,
            new List<string>()
            {
                "<size=300%>GO!</size>",
            }
        },

        {
            CopyTag.COSTUME_EQUIP,
            new List<string>()
            {
                "Equip",
            }
        },

        {
            CopyTag.COSTUME_OWNED,
            new List<string>()
            {
                "Owned",
            }
        },

        {
            CopyTag.DAILY_REWARD_CLAIM,
            new List<string>()
            {
                "Free Daily",
            }
        },

        {
            CopyTag.DAILY_REWARD_CLAIMED,
            new List<string>()
            {
                "Claimed",
            }
        },

        {
            CopyTag.QTA_EXPLANATION,
            new List<string>()
            {
                "<b><color=#00FF00>Green</color></b>: Fills up the Garbage Icon<br><b><color=#FFFF00>Yellow</color></b>: Get some Coins<br><b><color=#ff0000>Red</color></b>: Ink fills the screen",
            }
        },
    };

    public static string GetCopy(CopyTag copyTag)
    {
        string result = string.Empty;

#if UNITY_STANDALONE
        result = copyDesktop[copyTag][0];
#elif UNITY_ANDROID
        result = copyMobile[copyTag][0];
#endif

        return result;
    }
}
