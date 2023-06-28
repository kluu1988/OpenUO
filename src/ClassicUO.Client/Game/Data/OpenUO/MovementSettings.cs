using System;

namespace ClassicUO.Game.Data.OpenUO;

public class MovementSettings
{
    public int MoveSpeedWalkingUnmounted => MovementSpeed.STEP_DELAY_WALK;
    public int MoveSpeedRunningUnmounted => MovementSpeed.STEP_DELAY_RUN;

    //Default move speed with a mount
    public int MoveSpeedWalkingMounted => MovementSpeed.STEP_DELAY_MOUNT_WALK;
    public int MoveSpeedRunningMounted => MovementSpeed.STEP_DELAY_MOUNT_RUN;

    //Turning delay
    public int TurnDelay => Constants.TURN_DELAY;
}