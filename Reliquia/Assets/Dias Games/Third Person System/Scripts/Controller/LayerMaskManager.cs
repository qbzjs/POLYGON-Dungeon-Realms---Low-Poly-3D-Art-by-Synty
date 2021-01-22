using UnityEngine;

public static class LayerMaskManager
{
    public static LayerMask IgnoreOnlyPlayer = ~(1 << 15);
    public static LayerMask GroundMask = (1 << 14);
    public static LayerMask ClimbMask = (1 << 18) | (1 << 19);
    public static LayerMask LadderMask = (1 << 20);
    public static LayerMask LowerClimbMask = (1 << 17);
}
