using UnityEngine;
using System.Collections;

public class QuestItemView : MonoBehaviour
{

    #region Unity Editor
    public UISprite spBackground,spIconStar;
    public CUIHandle btnGetGifts;
    public UILabel lbQuestTitle, lbDescription, lbChip, lbExp,lbRequirementTitle,lbNumberProgressed;
    public UITable tbQuestionRequirement;
    public UITexture image;
    public UIProgressBar stepQuest;
    public GameObject tween;

    #endregion
    public void TweenFinished()
    {
        //Debug.Log("===========> tween.activeSelf : " + tween.activeSelf + " ----- tween.activeInHierarchy : " + tween.activeInHierarchy);
        //if (tween.activeSelf || tween.activeInHierarchy)
        //    spBackground.height = 380;
        //else{
        //    spBackground.height = 120;
        //}
    }
}
