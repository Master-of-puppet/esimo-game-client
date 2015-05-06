using UnityEngine;
using System.Collections;

public class TournamentOverView : MonoBehaviour {
    enum Round { 
        ROUND_ONE=0,
        ROUND_TWO,
        ROUND_THREE,
        ROUND_FOUR
    }
    string Round_Four_Table = "RoundFourTable";
    string Round_Two_Table = "RoundTwoTable";
    string Round_One_Table = "RoundOneTable";
    #region
    public CUIHandle btnBack, btnNext;
    public UILabel lblRoundOne, lblRoundTwo, lblRoundThree, lblRoundFour;
    public UISprite spriteRouneOne, spriteRoundTwo, spriteRoundThree, spriteRoundFour;
    public GameObject FirstRound, SecondRound, ThirdRound, LastRound;
    #endregion

    //Viewing Round.
    Round RoundNow = Round.ROUND_ONE;


    void Awake() { 
        CUIHandle.AddClick(btnBack, OnClickBack);
        CUIHandle.AddClick(btnNext, OnClickNext);
    }
	void Start () {
        TurnLightRoundViewing();
        NGUITools.SetActive(btnBack.gameObject, false);
	}

    void OnDestroy() {
        CUIHandle.RemoveClick(btnBack, OnClickBack);
        CUIHandle.RemoveClick(btnNext, OnClickNext);
    }

    void OnClickBack(GameObject targetObj) {
        RoundNow = RoundNow - 1;
        TurnLightRoundViewing();
        NGUITools.SetActive(btnNext.gameObject, true);
        if (RoundNow == Round.ROUND_ONE)
            targetObj.SetActive(false);
    }
    void OnClickNext(GameObject targetObj) {
        RoundNow = RoundNow + 1;
        TurnLightRoundViewing();
        NGUITools.SetActive(btnBack.gameObject, true);
        if (RoundNow == Round.ROUND_FOUR)
            targetObj.SetActive(false);
    }

    void TurnLightRoundViewing() {
        switch (RoundNow) { 
        
            case Round.ROUND_ONE:
                TurnOnLight(lblRoundOne, spriteRouneOne);
                TurnOffLight(lblRoundTwo, spriteRoundTwo);
                TurnOffLight(lblRoundThree, spriteRoundThree);
                TurnOffLight(lblRoundFour, spriteRoundFour);
                EnableRound(FirstRound);
                break;
            case Round.ROUND_TWO:
                TurnOffLight(lblRoundOne, spriteRouneOne);
                TurnOnLight(lblRoundTwo, spriteRoundTwo);
                TurnOffLight(lblRoundThree, spriteRoundThree);
                TurnOffLight(lblRoundFour, spriteRoundFour);
                EnableRound(SecondRound);
                break;
            case Round.ROUND_THREE:
                TurnOffLight(lblRoundOne, spriteRouneOne);
                TurnOffLight(lblRoundTwo, spriteRoundTwo);
                TurnOnLight(lblRoundThree, spriteRoundThree);
                TurnOffLight(lblRoundFour, spriteRoundFour);
                EnableRound(ThirdRound);
                break;
            case Round.ROUND_FOUR:
                TurnOffLight(lblRoundOne, spriteRouneOne);
                TurnOffLight(lblRoundTwo, spriteRoundTwo);
                TurnOffLight(lblRoundThree, spriteRoundThree);
                TurnOnLight(lblRoundFour, spriteRoundFour);
                EnableRound(LastRound);
                break;
        }
    }

    private void TurnOffLight(UILabel round, UISprite sprite) {
        round.color = new Color(254f, 203f, 0f,255f);
        sprite.spriteName = "DotDark";
    }

    private void TurnOnLight(UILabel round, UISprite sprite)
    {
        round.color = Color.white;
        sprite.spriteName = "DotLight";
    }

    void EnableRound(GameObject obj)
    {
        NGUITools.SetActive(FirstRound, false);
        NGUITools.SetActive(SecondRound, false);
        NGUITools.SetActive(ThirdRound, false);
        NGUITools.SetActive(LastRound, false);
        if (obj)
        {
            NGUITools.SetActive(obj, true);
        }
    }
}
