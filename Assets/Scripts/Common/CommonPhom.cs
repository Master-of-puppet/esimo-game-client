using UnityEngine;
using System.Collections;

public class CommonPhom{

    public static int RULE_CHIP_COMPARE_BETTING = 5;

    public static bool ValidateChipToBetting(int betting)
    {
        return GameManager.Instance.mInfo.chip >= (betting * RULE_CHIP_COMPARE_BETTING);
    }
}
