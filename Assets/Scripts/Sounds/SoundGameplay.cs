using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using Assets.Scripts.Gameplay;

public class SoundGameplay
{
    private static AudioManager.SoundEffect[] RedCardSounds = new AudioManager.SoundEffect[] { AudioManager.SoundEffect.LenXeLaPhong, AudioManager.SoundEffect.ThuMotConXemSao, AudioManager.SoundEffect.TayDo };
    private static int redCardSoundIndex = 0;
    private static GameUtil.ListCardID[] cardTom = new GameUtil.ListCardID[] { GameUtil.ListCardID.TAM_VAN_J, GameUtil.ListCardID.TAM_VAN_J, GameUtil.ListCardID.THAT_VAN_W };

    public static SoundGameplay _instances;
    public static SoundGameplay Instances
    {
        get
        {
            if (_instances == null)
                _instances = new SoundGameplay();
            return _instances;
        }
    }
    public int soundCardId = 0;

    public SoundGameplay()
    {
        
    }

    IEnumerator _PlaySoundTheEnd(float timeDeplay)
    {
        yield return new WaitForSeconds(timeDeplay);

        if (GameModelChan.YourController.PlayerState >= EPlayerController.EPlayerState.ready)
        {
            switch (GameModelChan.YourController.summary.result)
            {
                case PlayerControllerChan.FinishGame.ResultSprite.U:
                case PlayerControllerChan.FinishGame.ResultSprite.U_TRON:
                case PlayerControllerChan.FinishGame.ResultSprite.U_XUONG:
                case PlayerControllerChan.FinishGame.ResultSprite.XAO_KHAN:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.Fulllaying);
                    break;
                case PlayerControllerChan.FinishGame.ResultSprite.VE_NHAT:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.Nhat);
                    break;
                case PlayerControllerChan.FinishGame.ResultSprite.VE_NHI:
                case PlayerControllerChan.FinishGame.ResultSprite.VE_BA:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.NhiBa);
                    break;
                case PlayerControllerChan.FinishGame.ResultSprite.VE_TU:
                case PlayerControllerChan.FinishGame.ResultSprite.None:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.Bet);
                    break;
                case PlayerControllerChan.FinishGame.ResultSprite.MOM:
                    AudioManager.Instance.Play(AudioManager.SoundEffect.Mom);
                    break;
            }
        }
    }

    public void PlaySoundDrawCard(int[] soundId)
    {
        GameManager.Instance.StartCoroutine(_PlaySoundDrawCard(soundId));
    }
    IEnumerator _PlaySoundDrawCard(int[] soundId){
        for (int i = 0; i < soundId.Length; i++)
        {
            bool? isPlayDone = null;
            AudioManager.Instance.Play((AudioManager.SoundEffect)soundId[i], delegate()
            {
                isPlayDone = true;
            });
            while (isPlayDone == null)
                yield return new WaitForEndOfFrame();
        }
            
    }
    public void PlaySoundInGame(int soundId,PlayerControllerChan currentPlayer)
    {
        switch((AudioManager.SoundEffect)soundId){
            case AudioManager.SoundEffect.TayDo:
                AudioManager.Instance.Play(AudioManager.SoundEffect.TayDo);
                break;
            case AudioManager.SoundEffect.ThuMotConXemSao:
                currentPlayer.playSoundThuMotConXemSao = true;
                AudioManager.Instance.Play(AudioManager.SoundEffect.ThuMotConXemSao);
                break;
            case AudioManager.SoundEffect.BoTomLeoOmLeo:
                AudioManager.Instance.Play(AudioManager.SoundEffect.BoTomLeoOmLeo);
                break;
            case AudioManager.SoundEffect.ChayThoiKoNhaTrenUDe:
                AudioManager.Instance.Play(AudioManager.SoundEffect.ChayThoiKoNhaTrenUDe);
                break;
            case AudioManager.SoundEffect.BatVangoiNhiVan:
                AudioManager.Instance.Play(AudioManager.SoundEffect.BatVangoiNhiVan);
                break;
            case AudioManager.SoundEffect.NhiVanGoiBatVan:
                AudioManager.Instance.Play(AudioManager.SoundEffect.NhiVanGoiBatVan);
                break;
            case AudioManager.SoundEffect.TamVanGoiThatSach:
                AudioManager.Instance.Play(AudioManager.SoundEffect.TamVanGoiThatSach);
                break;
            case AudioManager.SoundEffect.ThatSachGoiTamVan:
                AudioManager.Instance.Play(AudioManager.SoundEffect.ThatSachGoiTamVan);
                break;
            case AudioManager.SoundEffect.DanhChiNgoiChoChiDung:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DanhChiNgoiChoChiDung);
                break;
            case AudioManager.SoundEffect.DanhChiDungChoChiNgoi:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DanhChiDungChoChiNgoi);
                break;
            case AudioManager.SoundEffect.LenXeLaPhong:
                AudioManager.Instance.Play(AudioManager.SoundEffect.LenXeLaPhong);
                currentPlayer.playSoundLenXeLaPhong = true;
                break;
            case AudioManager.SoundEffect.DiMotMinhCayDa:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DiMotMinhCayDa);
                break;
              // an bai
            case AudioManager.SoundEffect.ChayDiDau:
                AudioManager.Instance.Play(AudioManager.SoundEffect.ChayDiDau);
                break;
            case AudioManager.SoundEffect.CauDay:
                AudioManager.Instance.Play(AudioManager.SoundEffect.CauDay);
                break;
            case AudioManager.SoundEffect.DinhMucRoi:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DinhMucRoi);
                break;
            case AudioManager.SoundEffect.ChiuCHiu:
                AudioManager.Instance.Play(AudioManager.SoundEffect.ChiuCHiu);
                break;
            case AudioManager.SoundEffect.AnChoSo:
                AudioManager.Instance.Play(AudioManager.SoundEffect.AnChoSo);
                break;
            case AudioManager.SoundEffect.AnGiDanhDay:
                AudioManager.Instance.Play(AudioManager.SoundEffect.AnGiDanhDay);
                break;
            case AudioManager.SoundEffect.AnLuon:
                AudioManager.Instance.Play(AudioManager.SoundEffect.AnLuon);
                break;
            case AudioManager.SoundEffect.CauLai:
                AudioManager.Instance.Play(AudioManager.SoundEffect.CauLai);
                break;
            case AudioManager.SoundEffect.DanhCaiLayAiMaU:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DanhCaiLayAiMaU);
                break;
            case AudioManager.SoundEffect.DeDuoiAn:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DeDuoiAn);
                break;
            case AudioManager.SoundEffect.DuoiDung:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DuoiDung);
                break;
            case AudioManager.SoundEffect.DuoiLuon:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DuoiLuon);
                break;
            case AudioManager.SoundEffect.HetDoRoi:
                AudioManager.Instance.Play(AudioManager.SoundEffect.HetDoRoi);
                break;
            case AudioManager.SoundEffect.Duoi:
                AudioManager.Instance.Play(AudioManager.SoundEffect.Duoi);
                break;
            case AudioManager.SoundEffect.BaoLangKoChan:
                AudioManager.Instance.Play(AudioManager.SoundEffect.BaoLangKoChan);
                break;
        }
    }
    public void PlaySoundStealCard(int[] listCardId, PlayerControllerChan currentPlayer,PlayerControllerChan lastPlayer)
    {
        if (listCardId.Length == 4)// chiu'
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.ChiuCHiu);
        }
        else if (checkDinhMucRoi(listCardId, currentPlayer))
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.DinhMucRoi);
        }
        else if (checkCauDay(listCardId, currentPlayer))
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.CauDay);
        }
        else if (checkAnChoSo(listCardId, lastPlayer, currentPlayer))
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.AnChoSo);
        }
    }

    public AudioManager.SoundEffect getSoundChayThoiKoNhaTrenUDe()
    {
        return AudioManager.SoundEffect.ChayThoiKoNhaTrenUDe;
    }

    /**
     * cardID: Cây đánh ra
     * currentPlayer: Người đánh bài
     * nextPlayer: Người có lượt
     * 
     **/
    public void playAudioDisCard(int cardID, PlayerControllerChan currentPlayer, PlayerControllerChan nextPlayer, PlayerControllerChan lastPlayer)
    {
        if (!currentPlayer.isBatBao)
        {
            if (checkDiMotCayDa(cardID, nextPlayer))
            {
                AudioManager.Instance.Play(AudioManager.SoundEffect.DiMotMinhCayDa);
            }
            else if (checkTranhBiNhaTrenDi(cardID, lastPlayer))
            {
                if (!currentPlayer.playSounChayKoNhaTrenDe)
                {
                    AudioManager.SoundEffect soundPlayCheKoDi = getSoundChayThoiKoNhaTrenUDe();
                    AudioManager.Instance.Play(AudioManager.SoundEffect.ChayThoiKoNhaTrenUDe);
                    if (soundPlayCheKoDi == AudioManager.SoundEffect.ChayThoiKoNhaTrenUDe)
                        currentPlayer.playSounChayKoNhaTrenDe = true;
                }
            }
            else
            {
                // kiểm tra cây đỏ dánh lần đầu tiên
                if (GameUtil.IscardRed(cardID) && isFirstRound(currentPlayer))
                {
                    // kiểm tra cây tiếp cây đỏ cây gì
                    if (checkChayDiDauSound(nextPlayer))
                    {
                        AudioManager.Instance.Play(AudioManager.SoundEffect.ChayDiDau);
                    }
                    else
                    {
                        AudioManager.SoundEffect soundPlay = getSoundRedCard();
                        // kiểm tra đã ăn cây đỏ nào chưa!
                        if (!checkAnCayDo(cardID, currentPlayer))
                        {
                            AudioManager.Instance.Play(soundPlay);
                            // kiểm tra âm thanh đang bật là lên xe là phóng hay là thử một cây xem sao
                            if (soundPlay == AudioManager.SoundEffect.LenXeLaPhong)
                            {
                                currentPlayer.playSoundLenXeLaPhong = true;
                            }
                            else if (soundPlay == AudioManager.SoundEffect.ThuMotConXemSao)
                            {
                                currentPlayer.playSoundThuMotConXemSao = true;
                            }
                        }
                        else
                        {
                            playSoundNormal(cardID, currentPlayer);
                        }
                    }
                }
                else
                {
                    playSoundNormal(cardID, currentPlayer);
                }
            }
        }
    }
    private bool checkAnCayDo(int cardId,PlayerControllerChan currentPlayer)
    {
        if (currentPlayer.mCardSteal.Count > 0)
        {
            for (int i = 0; i < currentPlayer.mCardSteal.Count; i++)
            {
                if (GameUtil.IscardRed(currentPlayer.mCardSteal[i].steals[0].CardId) || GameUtil.IscardRed(currentPlayer.mCardSteal[i].steals[1].CardId))
                {
                    return true;
                }
            }
        }
            return false;
    }
   
    // lấy âm thanh đánh cây đỏ lần đâu tiên tẩy dỏ, thử một cây xem sao,câu đỏ, lên xe là phóng
    private AudioManager.SoundEffect getSoundRedCard()
    {
        if (++redCardSoundIndex >= RedCardSounds.Length)
        {
            redCardSoundIndex = 0;
        }
        return RedCardSounds[redCardSoundIndex];
    }

    private Boolean checkDiMotCayDa(int cardID, PlayerControllerChan nextPlayer)
    {
        if (nextPlayer.mCardTrash.Count > 0)
        {
            if (cardID == nextPlayer.mCardTrash[nextPlayer.mCardTrash.Count - 1].CardId)
            {
                return true;
            }
        }
        return false;
    }

    private Boolean isFirstRound(PlayerControllerChan currentPlayer)
    {
        return (currentPlayer.mCardDiscardedAndDraw.Count == 1);
    }

    private Boolean checkChayDiDauSound(PlayerControllerChan nextPlayer)
    {
        if (nextPlayer.mCardDiscardedAndDraw.Count > 0 && GameUtil.IscardRed(nextPlayer.mCardDiscardedAndDraw[nextPlayer.mCardDiscardedAndDraw.Count - 1].CardId) && nextPlayer.playSoundLenXeLaPhong)
            return true;
        else
            return false;
    }
    public int isPlayBoTomOmLeo(PlayerControllerChan currentPlayer)
    {
        int count = 0;
        for (int i = 0; i < currentPlayer.mCardDiscarded.Count; i++)
        {
            for (int j = 0; j < cardTom.Length; j++)
            {
                if (currentPlayer.mCardDiscarded[i].CardId == (int)cardTom[i])
                {
                    count++;
                }
            }
        }
        return count;
    }

    public AudioManager.SoundEffect getSoundBoTomOmLeo()
    {
        return AudioManager.SoundEffect.BoTomLeoOmLeo;
    }

    private void playSoundNormal(int cardID, PlayerControllerChan currentPlayer)
    {
        switch ((GameUtil.ListCardID)cardID)
        {
            case GameUtil.ListCardID.NGU_VAN_W:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DanhChiNgoiChoChiDung);
                break;
            case GameUtil.ListCardID.CHI_CHI:
                AudioManager.Instance.Play(AudioManager.SoundEffect.DanhChiDungChoChiNgoi);
                break;
            case GameUtil.ListCardID.TAM_VAN_J:
            case GameUtil.ListCardID.TAM_SACH:
            case GameUtil.ListCardID.THAT_VAN_W:
                {
                    if (!currentPlayer.playSoundBoTomOmLeo)
                    {
                        AudioManager.SoundEffect soundBotom = getSoundBoTomOmLeo();
                        AudioManager.Instance.Play(AudioManager.SoundEffect.BoTomLeoOmLeo);
                        if (soundBotom == AudioManager.SoundEffect.BoTomLeoOmLeo)
                        {
                            currentPlayer.playSoundBoTomOmLeo = true;
                        }
                    }
                }
                break;
            case GameUtil.ListCardID.BAT_VAN_J:
                AudioManager.Instance.Play(AudioManager.SoundEffect.BatVangoiNhiVan);
                break;
            case GameUtil.ListCardID.NHI_VAN_W:
                AudioManager.Instance.Play(AudioManager.SoundEffect.NhiVanGoiBatVan);
                break;
            case GameUtil.ListCardID.TAM_VAN_W:
                AudioManager.Instance.Play(AudioManager.SoundEffect.TamVanGoiThatSach);
                break;
            case GameUtil.ListCardID.THAT_SACH:
                AudioManager.Instance.Play(AudioManager.SoundEffect.ThatSachGoiTamVan);
                break;
        }
    }
    public bool checkTranhBiNhaTrenDi(int cardID, PlayerControllerChan lastPlayer)
    {
        if (null != lastPlayer && lastPlayer.mCardSteal.Count > 0) {
            if (cardID == lastPlayer.mCardSteal[lastPlayer.mCardSteal.Count - 1].steals[0].CardId && 
                cardID == lastPlayer.mCardSteal[lastPlayer.mCardSteal.Count - 1].steals[1].CardId)
            {
                return true;
            }
        }
        
        return false;
    }

    public void nhoGanAndDayRoi(int cardId)
    {
        int soudID = 0;
        soudID = RandomNumber(new int[] {( int)AudioManager.SoundEffect.NhoGanCon,(int)AudioManager.SoundEffect.URoi});
        if (soudID == (int)AudioManager.SoundEffect.NhoGanCon)
        {
            AudioManager.Instance.playSounUXaAndUGan(cardId, false);
        }
        else
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.URoi);
        }
    }
    private int RandomNumber(int[] arr)
    {
        int numberRandom=0;
        System.Random random = new System.Random();
        numberRandom = random.Next(0, arr.Length);
        return arr[numberRandom];
        
    }

    public void RandomNumberAndPlaySound(int[] arr)
    {
        int numberRandom = 0;
        System.Random random = new System.Random();
        numberRandom = random.Next(0, arr.Length);
        AudioManager.Instance.Play((AudioManager.SoundEffect)arr[numberRandom]);
    }

    //đánh cây đỏ vòng 1,2 sau đó lại ăn cây đỏ nhưng phải hạ cây đen xuống
    private bool checkCauDay(int[] listCardID, PlayerControllerChan currentPlayer)
    {
        // check vòng 1,2
        if (currentPlayer.mCardDiscardedAndDraw.Count < 3)
        {
            // check ăn cây đỏ và hạ cây đen
            if (listCardID.Length == 2 && GameUtil.IscardRed(listCardID[0]) && !GameUtil.IscardRed(listCardID[1]))
            {
                //đánh cây đỏ             
                foreach(ECard card in currentPlayer.mCardDiscarded)
                {
                    if (GameUtil.IscardRed(card.CardId))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //Dính mực rồi (Đánh quân đỏ sau lại ăn chẵn đỏ)
    private bool checkDinhMucRoi(int[] listCardID,PlayerControllerChan currentPlayer)
    {
        if (isFirstRound(currentPlayer))
        {
            if (currentPlayer.mCardDiscarded.Count > 0)
            {
                ECard card = currentPlayer.mCardDiscarded[currentPlayer.mCardDiscarded.Count - 1];//????
                if (GameUtil.IscardRed(card.CardId))// cây đánh ngay trước là cây đỏ
                {
                    if (listCardID.Length == 2)
                    {
                        if (listCardID[0] == listCardID[1] && GameUtil.IscardRed(listCardID[0]))// ăn chắn đỏ
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool checkAnChoSo(int[] listCardID,PlayerControllerChan lastPlayer,PlayerControllerChan currentPlayer)
    {
        if (lastPlayer.playSoundThuMotConXemSao)
        {
            lastPlayer.playSoundThuMotConXemSao = false;
            if (lastPlayer.mCardDiscarded.Count == 1)
            {
                if (currentPlayer.mCardDiscarded.Count > 1)
                {
                    return false;
                }
                else if (currentPlayer.mCardDiscarded.Count == 1 && currentPlayer.mCardDiscardedAndDraw[0].CardId == listCardID[0])
                {
                    return false;
                } 
                else 
                {
                    if (listCardID.Length == 2)
                    {
                        if (listCardID[0] == lastPlayer.mCardDiscarded[0].CardId)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
   
    public void ShowAudioOther(int index,int cardId)
    {
        switch (index)
        {
            case 1:
                AudioManager.Instance.Play(AudioManager.SoundEffect.VaoGaDi);
                break;
            case 2:
                AudioManager.Instance.Play(AudioManager.SoundEffect.Nhai);
                break;
            case 3:
                AudioManager.Instance.Play(AudioManager.SoundEffect.HoaCaLang);
                break;
            case 4:
                AudioManager.Instance.Play(AudioManager.SoundEffect.BatBaoNgoiImNhe);
                break;
            case 52:
                AudioManager.Instance.Play(AudioManager.SoundEffect.ThanToOng);
                break;
            case 42:
                AudioManager.Instance.Play(AudioManager.SoundEffect.UCuaMinh);
                break;
            case 6:
                AudioManager.Instance.Play(AudioManager.SoundEffect.URoi);
                break;
        }
    }
    
    Dictionary<int, int> dicSound = new Dictionary<int, int>();
    public void ResetSoundWhenNewGame()
    {
        dicSound = new Dictionary<int, int>();
    }
    public List<CardDrawSound> GetList(int id)
    {
        List<CardDrawSound> list = new List<CardDrawSound>();
        string[] name = Utility.EnumUtility.GetEnumName(typeof(CardDrawSound));
        for (int i = 0; i < name.Length; i++)
        {
            CardDrawSound e = ConvertSoundCardDraw(name[i]);
            if (id == Utility.EnumUtility.GetAttribute<AttributeCardDrawSound>(e).CardId)
            {
                list.Add(e);
            }
        }
        return list;
    }

    public CardDrawSound ConvertSoundCardDraw(string name)
    {
        MemberInfo[] memberInfos = typeof(CardDrawSound).GetMembers(BindingFlags.Public | BindingFlags.Static);
        for (int i = 0; i < memberInfos.Length; i++)
        {
            if (memberInfos[i].Name == name) 
            {
                return (CardDrawSound)Enum.Parse(typeof(CardDrawSound), memberInfos[i].Name);
            }
        }
        return CardDrawSound.none;
    }

    public void PlayerShowCard(int cardID, bool next)
    {
        List<CardDrawSound> listSoundDraw = GetList(cardID);
        CardDrawSound e = listSoundDraw[0];
        AttributeCardDrawSound attributeCardSound = Utility.EnumUtility.GetAttribute<AttributeCardDrawSound>(e);
        if (attributeCardSound.SoundId != -1)
        {
            if (dicSound.ContainsKey(attributeCardSound.CardId))
            {
                if (next)
                {
                    if (dicSound[attributeCardSound.CardId] == listSoundDraw.Count - 1)
                    {
                        dicSound[attributeCardSound.CardId] = attributeCardSound.IndexSoundOfCard;
                        AudioManager.Instance.PlaySoundDrawCard(attributeCardSound.SoundId);
                    }
                    else
                    {
                        dicSound[attributeCardSound.CardId] = dicSound[attributeCardSound.CardId] + 1;
                        CardDrawSound nextSound = listSoundDraw.Find(c => Utility.EnumUtility.GetAttribute<AttributeCardDrawSound>(c).IndexSoundOfCard == dicSound  [attributeCardSound.CardId]);
                        AudioManager.Instance.PlaySoundDrawCard(Utility.EnumUtility.GetAttribute<AttributeCardDrawSound>(nextSound).SoundId);
                    }
                }
                else
                {
                    CardDrawSound nextSound = listSoundDraw.Find(c => Utility.EnumUtility.GetAttribute<AttributeCardDrawSound>(c).IndexSoundOfCard == dicSound[attributeCardSound.CardId]);
                    AudioManager.Instance.PlaySoundDrawCard(Utility.EnumUtility.GetAttribute<AttributeCardDrawSound>(nextSound).SoundId);
                }

            }
            else
            {
                dicSound.Add(attributeCardSound.CardId, attributeCardSound.IndexSoundOfCard);
                AudioManager.Instance.PlaySoundDrawCard(attributeCardSound.SoundId);
                soundCardId = attributeCardSound.SoundId;
            }
        }
    }
    public void PlaySoundBocCai(int cardId,int PlayerNumber,int soundid)
    {
        if (soundid == (int)AudioManager.SoundEffect.CamOnCon)
        {
            AudioManager.Instance.playSoundBocCai(cardId, AudioManager.SoundEffect.CamOnCon);
        }
        else
        {
            if (PlayerNumber == 4)
            {
                switch ((GameUtil.ListCardID)cardId)
                {
                    case GameUtil.ListCardID.NHI_SACH:
                    case GameUtil.ListCardID.NHI_VAN_J:
                    case GameUtil.ListCardID.NHI_VAN_W:
                        AudioManager.Instance.Play(AudioManager.SoundEffect.NhiTien);
                        break;
                    case GameUtil.ListCardID.TAM_SACH:
                    case GameUtil.ListCardID.TAM_VAN_J:
                    case GameUtil.ListCardID.TAM_VAN_W:
                        AudioManager.Instance.Play(AudioManager.SoundEffect.TamDoi);
                        break;
                    case GameUtil.ListCardID.TU_SACH:
                    case GameUtil.ListCardID.TU_VAN_J:
                    case GameUtil.ListCardID.TU_VAN_W:
                        AudioManager.Instance.Play(AudioManager.SoundEffect.TuTut);
                        break;
                    case GameUtil.ListCardID.NGU_VAN_J:
                    case GameUtil.ListCardID.NGU_VAN_W:
                    case GameUtil.ListCardID.NGU_SACH:
                        AudioManager.Instance.Play(AudioManager.SoundEffect.NhatNguu);
                        break;

                    case GameUtil.ListCardID.LUC_VAN_J:
                    case GameUtil.ListCardID.LUC_VAN_W:
                    case GameUtil.ListCardID.LUC_SACH:
                        AudioManager.Instance.Play(AudioManager.SoundEffect.LucTien);
                        break;

                    case GameUtil.ListCardID.THAT_SACH:
                    case GameUtil.ListCardID.THAT_VAN_J:
                    case GameUtil.ListCardID.THAT_VAN_W:
                        AudioManager.Instance.Play(AudioManager.SoundEffect.ThatDoi);
                        break;
                    case GameUtil.ListCardID.BAT_VAN_J:
                    case GameUtil.ListCardID.BAT_VAN_W:
                    case GameUtil.ListCardID.BAT_SACH:
                        AudioManager.Instance.Play(AudioManager.SoundEffect.BatTut);
                        break;
                    case GameUtil.ListCardID.CUU_SACH:
                    case GameUtil.ListCardID.CUU_VAN_J:
                    case GameUtil.ListCardID.CUU_VAN_W:
                        AudioManager.Instance.Play(AudioManager.SoundEffect.NhatCuu);
                        break;
                    case GameUtil.ListCardID.CHI_CHI:
                        AudioManager.Instance.Play(AudioManager.SoundEffect.NhatChiChi);
                        break;
                }
            }
            else
            {
                AudioManager.Instance.playSoundBocCai(cardId, AudioManager.SoundEffect.Cai);
            }
        }
    }
}

