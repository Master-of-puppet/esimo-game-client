using UnityEngine;
using System.Collections;

public class CardTextureChan : ECardTexture
{
    public UISlider processBar;
    public override int getHeight()
    {
        return 70;
    }

    public override int getWith()
    {
        return 50;
    }

    public override void OnUpdate()
    {
        if (card != null && ((ChanCard)card).timeExpire > 0)
        {
            if (!processBar.gameObject.activeSelf)
                processBar.gameObject.SetActive(true);

            timeCountdown -= Time.deltaTime;
            processBar.value = timeCountdown / ((ChanCard)card).timeExpire;

            if (timeCountdown <= 0)
            {
                ((ChanCard)card).timeExpire = 0f;
                processBar.gameObject.SetActive(false);
            }
        }
    }

    public override void SetValue()
    {
        texture.mainTexture = (Texture)Resources.Load("Images/Card/" + card.CardId, typeof(Texture));
        texture.shader = (Shader)Resources.Load("Shaders/Unlit - Transparent Colored", typeof(Shader));
    }
    public float timeCountdown = 10f;
    public void setTimeCountDown()
    {
        this.timeCountdown = ((ChanCard)card).timeExpire;
    }

    public override void OnClick()
    {
        if (gameObject.GetComponent<iTween>() && gameObject.GetComponent<iTween>().isRunning)

            if (!enabled) return;

        if (card.CardId == -1)
        {
            if (GameModelChan.CurrentState != GameModelChan.EGameState.dealClient) return;
            if (GameModelChan.game.dealCardEffect.currentStep != GameplayDealCardEffect.StepDealCard.WAIT_PICK) return;
            if (GameModelChan.YourController != null && GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer) GameModelChan.game.dealCardEffect.CardClick(card);
        }
        else
        {
            if (GameModelChan.CurrentState != GameModelChan.EGameState.playing)
                return;
            GameModelChan.game.CardClick(card);
        }
    }
}
