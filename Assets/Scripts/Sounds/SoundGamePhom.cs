using UnityEngine;
using System.Collections;

public class SoundGamePhom
{
    public static SoundGamePhom _instances;
    public static SoundGamePhom Instances
    {
        get
        {
            if (_instances == null)
                _instances = new SoundGamePhom();
            return _instances;
        }
    }

    public void PlaySoundDisCard(PlayerControllerPhom currentPlayer, PlayerControllerPhom nextPlayer)
    {
        if (currentPlayer.mCardTrash.Count == 1 && currentPlayer.mCardTrash[0].parentCard.Rank.Value >= 10)
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.ToDanhBeGui, AudioManager.EPlayState.force);
        }
        if (currentPlayer.mCardTrash.Count >= 3 && nextPlayer.mCardTrash.Count == 3 && nextPlayer.mCardHand.FindAll(c => c.originSide != c.currentSide).Count == 0 && nextPlayer.mCardMelds.Count == 0)
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.AnDiKhongMom, AudioManager.EPlayState.force);
        }
    }
    public bool PlaySoundStealCard(PlayerControllerPhom currentPlayer, PlayerControllerPhom lastPlayer)
    {
        bool isStealCard = false;
        if (currentPlayer.mCardTrash.Count == 3 && lastPlayer.mCardTrash.Count >= 3)
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.AnChot, AudioManager.EPlayState.force);
            isStealCard = true;
        }
        else if (currentPlayer.mCardTrash.Count > 0 && currentPlayer.mCardTrash[currentPlayer.mCardTrash.Count - 1].parentCard.Rank.Value == lastPlayer.mCardTrash[lastPlayer.mCardTrash.Count - 1].parentCard.Rank.Value
            && currentPlayer.mCardTrash[currentPlayer.mCardTrash.Count - 1].originSide == currentPlayer.mCardTrash[currentPlayer.mCardTrash.Count - 1].currentSide
        )
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.Cau, AudioManager.EPlayState.force);
            isStealCard = true;
        }
        else
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.AnCay, AudioManager.EPlayState.force);
            isStealCard = true;
        }
        return isStealCard;
    }
    public void PlaySound(GamePlayPhom game)
    {

        if (game.gameFinishType == GamePlayPhom.EFinishType.U_DEN_THUONG || game.gameFinishType == GamePlayPhom.EFinishType.U_DEN_TRON)
        {
            GamePlayPhom.Summary sum = game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.AN_CAY_CHOT_DEN);
            if (sum != null)
            {
                AudioManager.Instance.Play(AudioManager.SoundEffect.DenAnChot, AudioManager.EPlayState.force);
            }
            else
            {
                AudioManager.Instance.Play(AudioManager.SoundEffect.Den3Cay, AudioManager.EPlayState.force);
            }
        }
        else
        {

            switch (game.gameFinishType)
            {
                case GamePlayPhom.EFinishType.U_TRON:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.UTron, AudioManager.EPlayState.force);
                    break;
                case GamePlayPhom.EFinishType.U_XUONG:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.UXuong, AudioManager.EPlayState.force);
                    break;
                case GamePlayPhom.EFinishType.NORMAL:
                    GamePlayPhom.Summary sum = game.summaryGame.Find(o => o.action == GamePlayPhom.Summary.EAction.XAO_KHAN);
                    if (sum != null)
                        AudioManager.Instance.Play(AudioManager.SoundEffect.XaoKhan, AudioManager.EPlayState.force);
                    break;
            }
        }
        game.StartCoroutine(_PlaySoundTheEnd(AudioManager.LastPlayLength));
    }

    IEnumerator _PlaySoundTheEnd(float timeDeplay)
    {
        yield return new WaitForSeconds(timeDeplay);

        if (GameModelPhom.YourController.PlayerState >= PlayerControllerPhom.EPlayerState.ready)
        {
            switch (GameModelPhom.YourController.summary.result)
            {
                case PlayerControllerPhom.FinishGame.ResultSprite.U:
                case PlayerControllerPhom.FinishGame.ResultSprite.U_TRON:
                case PlayerControllerPhom.FinishGame.ResultSprite.U_XUONG:
                case PlayerControllerPhom.FinishGame.ResultSprite.XAO_KHAN:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.Fulllaying);
                    break;
                case PlayerControllerPhom.FinishGame.ResultSprite.VE_NHAT:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.Nhat);
                    break;
                case PlayerControllerPhom.FinishGame.ResultSprite.VE_NHI:
                case PlayerControllerPhom.FinishGame.ResultSprite.VE_BA:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.NhiBa);
                    break;
                case PlayerControllerPhom.FinishGame.ResultSprite.VE_TU:
                case PlayerControllerPhom.FinishGame.ResultSprite.None:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.Bet);
                    break;
                case PlayerControllerPhom.FinishGame.ResultSprite.MOM:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.Mom);
                    break;
            }
        }
    }

}