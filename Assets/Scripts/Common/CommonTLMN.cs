using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class CommonTLMN
{
    public static int RULE_DEM_LA_CHIP_COMPARE_BETTING = 20;
    public static int RULE_XEP_HANG_CHIP_COMPARE_BETTING = 5;
    public const int MULTI_MONEY_OF_XEP_HANG = 10;

    public static bool ValidateChipToRateBetting(int betting)
    {
        return GameManager.Instance.mInfo.chip >= (betting * RULE_XEP_HANG_CHIP_COMPARE_BETTING);
    }
    public static bool ValidateChipToCountCardBetting(int betting)
    {
        return GameManager.Instance.mInfo.chip >= (betting * RULE_DEM_LA_CHIP_COMPARE_BETTING);
    }

    public static bool ValidateChipToBetting(int betting, bool isCountCard)
    {
        int rule = isCountCard ? RULE_DEM_LA_CHIP_COMPARE_BETTING : RULE_XEP_HANG_CHIP_COMPARE_BETTING;
        return GameManager.Instance.mInfo.chip >= (betting * rule);
    }

    public static bool ValidateChipToBetting(int betting, LobbyTLMN.GameConfig.GameTypeLTMN gameType)
    {
        int rule = gameType == LobbyTLMN.GameConfig.GameTypeLTMN.XEP_HANG ? RULE_DEM_LA_CHIP_COMPARE_BETTING : RULE_XEP_HANG_CHIP_COMPARE_BETTING;
        return GameManager.Instance.mInfo.chip >= (betting * rule);
    }
    public static bool ValidateChipToBetting(int betting)
    {
        return GameManager.Instance.mInfo.chip >= (betting * RULE_XEP_HANG_CHIP_COMPARE_BETTING);
    }
}

