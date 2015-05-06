using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SoundGamePlayTLMN
{
    static SoundGamePlayTLMN _instance;

    public static SoundGamePlayTLMN Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SoundGamePlayTLMN();
            return _instance;
        }
    }

    public void PlaySound(GameModelTLMN model, GamePlayTLMN game, List<ECard> lastDisCard, PlayerControllerTLMN currentController)
    {
        if (model.listDiscard == null || model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count == 0) { return; }

        if (model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count == 1)
        {
            if (game.cardController.TypeLastDiscard == CardControllerTLMN.EMultiType.Vertical && lastDisCard.Count > 2)
            {
                PlaySoundWhenFirstTurn(lastDisCard);
            }
            else if (lastDisCard.Count == 1)
            {
                if (lastDisCard[0].parentCard.Rank == CardLib.Model.Rank.THREE && lastDisCard[0].parentCard.Suit == CardLib.Model.Suit.SPADE_CLUB_DIAMOND_HEART[0] && currentController.mCardHand.Count != 0 && ((LobbyTLMN)GameManager.Instance.selectedLobby).config.GAME_TYPE_TLMN == LobbyTLMN.GameConfig.GameTypeLTMN.DEM_LA)
                {
                    AudioManager.Instance.Play(AudioManager.SoundEffect.danh_khong_lai_thoi, AudioManager.EPlayState.force);
                }
            }
        }
        switch (lastDisCard.Count)
        {
            case 1:
                if (lastDisCard[0].parentCard.Rank == CardLib.Model.Rank.TWO)
                {
                    if (lastDisCard[0].parentCard.Suit == CardLib.Model.Suit.SPADE_CLUB_DIAMOND_HEART[3])
                    {
                        AudioManager.Instance.Play(AudioManager.SoundEffect.hang_dau_chat_di, AudioManager.EPlayState.force);
                    }
                    else
                    {
                        AudioManager.Instance.Play(AudioManager.SoundEffect.heo, AudioManager.EPlayState.force);
                    }
                }
                else if (lastDisCard[0].parentCard.Rank == CardLib.Model.Rank.ACE && lastDisCard[0].parentCard.Suit == CardLib.Model.Suit.SPADE_CLUB_DIAMOND_HEART[3])
                {
                    AudioManager.Instance.Play(AudioManager.SoundEffect.xin_con_heo, AudioManager.EPlayState.force);
                }
                break;
            case 2:

                if (isContainsACEHeart(lastDisCard))
                {
                    AudioManager.Instance.Play(AudioManager.SoundEffect.xin_doi_heo, AudioManager.EPlayState.force);
                }
                if (lastDisCard[0].parentCard.Rank == CardLib.Model.Rank.TWO)
                {
                    if (isContainsTwoHeart(lastDisCard))
                    {
                        AudioManager.Instance.Play(AudioManager.SoundEffect.tu_quy_dau, AudioManager.EPlayState.force);
                    }
                    else
                    {
                        AudioManager.Instance.Play(AudioManager.SoundEffect.doi_heo, AudioManager.EPlayState.force);
                    }
                }
                break;
            case 4:
                if (game.cardController.TypeLastDiscard == CardControllerTLMN.EMultiType.Horizontal)
                {
                    List<ECard> lastCardTrash = model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count - 1];
                    if (lastCardTrash.Count == 2)
                    {
                        if (isContainsTwoHeart(lastCardTrash))
                        {
                            AudioManager.Instance.Play(AudioManager.SoundEffect.tu_quy_day, AudioManager.EPlayState.force);
                        }
                        else
                        {
                            AudioManager.Instance.Play(AudioManager.SoundEffect.tu_quy, AudioManager.EPlayState.force);
                        }
                    }
                    else
                    {
                        AudioManager.Instance.Play(AudioManager.SoundEffect.tu_quy, AudioManager.EPlayState.force);
                    }

                }
                else if (game.cardController.TypeLastDiscard == CardControllerTLMN.EMultiType.Vertical)
                {
                    if (lastDisCard[0].parentCard.Rank == CardLib.Model.Rank.SIX && lastDisCard[lastDisCard.Count - 1].parentCard.Rank == CardLib.Model.Rank.NINE)
                    {
                        AudioManager.SoundEffect[] effect = { AudioManager.SoundEffect.sanh_4, AudioManager.SoundEffect.san_bang_tat_ca, AudioManager.SoundEffect.song_bang_tinh_cam };
                        System.Random r = new System.Random();
                        AudioManager.Instance.Play(effect[r.Next(effect.Length)], AudioManager.EPlayState.force);
                    }
                    else if (lastDisCard[0].parentCard.Rank == CardLib.Model.Rank.THREE && lastDisCard[lastDisCard.Count - 1].parentCard.Rank == CardLib.Model.Rank.SIX)
                    {

                    }
                }
                break;
            case 6:
            case 8:
                List<ECard> lastCardTrashDoiThong = model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count - 1];
                if (game.cardController.TypeLastDiscard == CardControllerTLMN.EMultiType.MoreThreePairs && lastDisCard.Count == 6 && !isContainsTwoHeart(lastCardTrashDoiThong))
                {

                }
                else if (game.cardController.TypeLastDiscard == CardControllerTLMN.EMultiType.MoreThreePairs && lastDisCard.Count == 8 && !isContainsTwoHeart(lastCardTrashDoiThong))
                {

                }
                else if ((game.cardController.TypeLastDiscard == CardControllerTLMN.EMultiType.MoreThreePairs && lastDisCard.Count == 6 && isContainsTwoHeart(lastCardTrashDoiThong)) || (game.cardController.TypeLastDiscard == CardControllerTLMN.EMultiType.MoreThreePairs && lastDisCard.Count == 8 && isContainsTwoHeart(lastCardTrashDoiThong)))
                {
                    AudioManager.Instance.Play(AudioManager.SoundEffect.hang_day, AudioManager.EPlayState.force);
                }
                break;
        }
    }

    private bool isContainsTwoHeart(List<ECard> cards)
    {
        foreach (ECard card in cards)
        {
            if (card.parentCard.Rank == CardLib.Model.Rank.TWO && card.parentCard.Suit == CardLib.Model.Suit.SPADE_CLUB_DIAMOND_HEART[3])
            {
                return true;
            }
        }
        return false;
    }
    private bool isContainsACEHeart(List<ECard> cards)
    {
        foreach (TLMNCard card in cards)
        {
            if (card.parentCard.Rank == CardLib.Model.Rank.ACE && card.parentCard.Suit == CardLib.Model.Suit.SPADE_CLUB_DIAMOND_HEART[3])
            {
                return true;
            }
        }
        return false;

    }
    private void PlaySoundWhenFirstTurn(List<ECard> lastDisCard)
    {
        switch (lastDisCard.Count)
        {
            case 3:
                AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_3, AudioManager.EPlayState.force);
                break;
            case 4:
                if (lastDisCard[0].parentCard.Rank == CardLib.Model.Rank.SIX && lastDisCard[lastDisCard.Count - 1].parentCard.Rank == CardLib.Model.Rank.NINE)
                {
                    AudioManager.SoundEffect[] effect = { AudioManager.SoundEffect.sanh_4, AudioManager.SoundEffect.san_bang_tat_ca, AudioManager.SoundEffect.song_bang_tinh_cam };
                    System.Random r = new System.Random();
                    int random = r.Next(effect.Length);
                    AudioManager.Instance.Play(effect[random], AudioManager.EPlayState.force);
                }
                else
                {
                    AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_4, AudioManager.EPlayState.force);
                }
                break;
            case 5:
                AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_5, AudioManager.EPlayState.force);
                break;
            case 6:
                AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_6, AudioManager.EPlayState.force);
                break;
            case 7:
                AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_7, AudioManager.EPlayState.force);
                break;
            case 8:
                AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_8, AudioManager.EPlayState.force);
                break;
            case 9:
                AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_9, AudioManager.EPlayState.force);
                break;
            case 10:
                AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_10, AudioManager.EPlayState.force);
                break;

        }
    }
    public void CheckFinishNormal(GameModelTLMN model)
    {
        if (model.listDiscard == null || model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count == 0) return;

        List<ECard> lastCardTrash = model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp][model.listDiscard[(int)GameModelTLMN.EDiscard.FaceUp].Count - 1];
        if (lastCardTrash.Count == 1)
        {
            if (lastCardTrash[0].parentCard.Rank == CardLib.Model.Rank.THREE && lastCardTrash[0].parentCard.Suit == CardLib.Model.Suit.SPADE_CLUB_DIAMOND_HEART[0] && ((LobbyTLMN)GameManager.Instance.selectedLobby).config.GAME_TYPE_TLMN == LobbyTLMN.GameConfig.GameTypeLTMN.DEM_LA)
            {
                AudioManager.Instance.Play(AudioManager.SoundEffect.nhan_ba_len, AudioManager.EPlayState.force);
            }
            else
            {
                PlaySoundFinishGame(GamePlayTLMN.EFinishByThangTrangType.NONE);
            }
        }
        else
        {
            PlaySoundFinishGame(GamePlayTLMN.EFinishByThangTrangType.NONE);
        }
    }
    public void PlaySoundFinishGame(GamePlayTLMN.EFinishByThangTrangType FinishType)
    {
        switch (FinishType)
        {
            case GamePlayTLMN.EFinishByThangTrangType.BA_DOI_THONG_CO_CAI:
                //AudioManager.Instance.Play(AudioManager.SoundEffect.luc_p);
                break;
            case GamePlayTLMN.EFinishByThangTrangType.BON_DOI_THONG_CO_CAI:
                //AudioManager.Instance.Play(AudioManager.SoundEffect.luc_p);
                break;
            case GamePlayTLMN.EFinishByThangTrangType.HAI_TU_QUY:
                //AudioManager.Instance.Play(AudioManager.SoundEffect.h
                break;
            case GamePlayTLMN.EFinishByThangTrangType.NAM_DOI_THONG:
                ///AudioManager.Instance.p
                break;
            case GamePlayTLMN.EFinishByThangTrangType.SANH_3_DEN_A:
                AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_tu_3_den_at, AudioManager.EPlayState.force);
                break;
            case GamePlayTLMN.EFinishByThangTrangType.SANH_RONG:
                AudioManager.Instance.Play(AudioManager.SoundEffect.sanh_rong, AudioManager.EPlayState.force);
                break;
            case GamePlayTLMN.EFinishByThangTrangType.SAU_DOI_THONG:
                AudioManager.Instance.Play(AudioManager.SoundEffect.luc_phe_bon, AudioManager.EPlayState.force);
                break;
            case GamePlayTLMN.EFinishByThangTrangType.TU_QUY_2:
                // AudioManager.Instance.Play(AudioManager.SoundEffect.luc_p);
                break;
            case GamePlayTLMN.EFinishByThangTrangType.TU_QUY_BANG_CAI:
                // AudioManager.Instance.Play(AudioManager.SoundEffect.luc_p);
                break;
            default:
                AudioManager.Instance.Play(AudioManager.SoundEffect.dem_la, AudioManager.EPlayState.force);
                break;
        }
    }
}
