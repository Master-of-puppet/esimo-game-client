using Assets.Scripts.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;
public enum CardDrawSound
{
    // Nhị
    //----------------- Nhị vạn-----------------
    [AttributeCardDrawSound(0, 1, 0)]
    NhiVanj = 0,
    [AttributeCardDrawSound(0, 2, 1)]
    NhiDao = 1,
    //-----------------Nhị văn-----------------
    [AttributeCardDrawSound(1, 0, 0)]
    NhiVVan = 2,
    //----------------- Nhị Sách-----------------
    // đang không có âm , có âm sẽ cho vào index số 3 
    [AttributeCardDrawSound(2, 3, 0)]
    NhiSach = 3,

    // ===================Tam 
    // -----------------Tam vạn-----------------
    [AttributeCardDrawSound(3, 5, 0)]
    TamVan = 4,
    // -----------------Tam văn-----------------
    [AttributeCardDrawSound(4, 4, 0)]
    TamVVan = 5,
    //-----------------Tam sách-----------------
    [AttributeCardDrawSound(5, 6, 0)]
    TamSach = 6,

    //===================Tứ
    //-------------Tứ vạn-------------
    [AttributeCardDrawSound(6, 8, 0)]
    TuVanj = 7,
    [AttributeCardDrawSound(6, 9, 1)]
    TuXeBo = 8,
    //-------------Tứ văn-------------
    [AttributeCardDrawSound(7, 7, 0)]
    TuVVan = 9,
    //-------------Tứ sách-------------
    [AttributeCardDrawSound(8, 10, 0)]
    TuSach = 10,

    //===================Ngũ
    //-------------Ngũ vạn-------------
    [AttributeCardDrawSound(9, 13, 0)]
    NguVan = 11,
    [AttributeCardDrawSound(9, 14, 1)]
    NguChua = 12,
    [AttributeCardDrawSound(9, 15, 2)]
    ChuaDo = 13,
    //-------------Ngũ văn-------------
    [AttributeCardDrawSound(10, 11, 0)]
    NguVVan = 14,
    [AttributeCardDrawSound(10, 12, 1)]
    ChiNgoi = 15,
    //-------------Ngũ Sách-------------
    [AttributeCardDrawSound(11, 16, 0)]
    NguSach = 16,
    [AttributeCardDrawSound(11, 17, 1)]
    NguThuyen = 17,
    [AttributeCardDrawSound(11, 18, 2)]
    ThuyenBuom = 18,

    //=============Lục
    //----------Lục vạn----------
    [AttributeCardDrawSound(12, 20, 0)]
    LucVan = 19,
    // ----------Lục văn----------
    [AttributeCardDrawSound(13, 19, 0)]
    LucVVan = 20,
    // ----------Lục sách----------
    [AttributeCardDrawSound(14, 21, 0)]
    LucSach = 21,

    //=============Thất
    // ---------Thất vạn-------------
    [AttributeCardDrawSound(15, 24, 0)]
    ThatVan = 22,
    //----------Thất văn-------------
    [AttributeCardDrawSound(16, 22, 0)]
    ThatVVan = 23,
    [AttributeCardDrawSound(16, 23, 1)]
    ThatTom = 24,
    //----------Thất sách----------
    [AttributeCardDrawSound(17, 25, 0)]
    ThatSach = 25,
    //=============Bát
    //-------------Bát vạn-------------
    [AttributeCardDrawSound(18, 28, 0)]
    BatVan = 26,
    [AttributeCardDrawSound(18, 29, 1)]
    BatCa = 27,
    [AttributeCardDrawSound(18, 30, 2)]
    CaChep = 28,
    //---------Bát văn-------------
    [AttributeCardDrawSound(19, 26, 0)]
    BatVVan = 29,
    [AttributeCardDrawSound(19, 27, 1)]
    BatDen = 30,
    //----------- Bát sách------------
    [AttributeCardDrawSound(20, 31, 0)]
    BatSach = 31,
    //=============Cửu
    //--------------Cửu vạn--------------
    [AttributeCardDrawSound(21, 35, 0)]
    CuuVan = 32,
    [AttributeCardDrawSound(21, 36, 1)]
    BocVac = 33,
    //-----------Cửu văn-------------
    [AttributeCardDrawSound(22, 32, 0)]
    CuuVVan = 34,
    [AttributeCardDrawSound(22, 33, 1)]
    CuuDen = 35,
    [AttributeCardDrawSound(22, 34, 2)]
    MotSach = 36,
    //-------------- Cửu sách-------------
    [AttributeCardDrawSound(23, 37, 0)]
    CuuSach = 37,
    //-------------Chi Chi-------------
    [AttributeCardDrawSound(24, 38, 0)]
    ChiChi = 38,
    [AttributeCardDrawSound(24, 39, 1)]
    ChiDung = 39,
    [AttributeCardDrawSound(-1, -1, 0)]
    none = -1,

}

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    private const float fadeTime = 3.0f;

    private EPlayState oldState = EPlayState.normal;
    
    /// <summary>
    /// Get or initialize the static singleton instance
    /// </summary>
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/AudioManager"));
                obj.name = "__AudioManager";
                _instance = obj.GetComponent<AudioManager>();
                _instance.Init();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public void Init()
    {
        SetVolumeEffect(StoreGame.LoadInt(StoreGame.EType.SOUND_EFFECT, (int)StoreGame.EToggle.ON) == (int)StoreGame.EToggle.OFF);

        SetVolumeBackground(StoreGame.LoadInt(StoreGame.EType.SOUND_BACKGROUND, (int)StoreGame.EToggle.ON) == (int)StoreGame.EToggle.OFF);
    }

    public void OnApplicationQuit()
    {
        _instance = null;
    }

    bool previousEffectFocus, previousBackgroundFocus;
    void OnApplicationFocus(bool focusStatus)
    {
        if (!focusStatus)
        {
            previousEffectFocus = sourceEffect.mute;
            previousBackgroundFocus = sourceBackground.mute;
        }
        else
        {
            sourceEffect.mute = previousEffectFocus;
            sourceBackground.mute = previousBackgroundFocus;
        }

        
    }

    public enum EPlayState
    {
        normal,
        loop,
        fadingOut,
        force,
    }
    public enum CipCardSoundEffect
    {
        AnGiDanhDay=0,
        AnLuon=1,
        BachThuChi=2,
        BaoLangKoChan=3,
        BatTut=4,
        Cai=5,
        CamOnCon=6,
        CauLai=7,
        DanhCaiLayAiMaU=8,
        DeDuoiAn=9,
        DuoiDung=10,
        DuoiLuon=11,
        Duoi=12,
        HetDoRoi=13,
        LucTien=14,
        NhanhLenNao=15,
        NhatChiChi=16,
        NhatCuu=17,
        NhatNguu=18,
        NhiSach=19,
        NhiTien=20,
        QuaChiXet=21,
        TamDoi=22,
        TatNuocTheoMua=23,
        ThatDoi=24,
        TreoTranhTraiBimNgiAnTien=25,
        TuTut=26,
    }
   
    public enum SoundEffect
    {
        ChiaBai = 0,
        SelectCard = 1,
        StealCard = 2,
        Discard = 3,
        Fulllaying = 4,
        Mom = 5,
        NhiBa = 6,
        Bet = 7,
        Nhat = 8,
        Button = 9,
        OneSeconds = 10,
        OneSecondsRushed = 11,
        LayingMeld = 12,
        OrderCard = 13,
        Warning_Your_Turn = 14,
        Warning_Lay_Meld = 15,

        // cuoc sac
        Thong=41,
        Chi=42,
        Xuong=43,
        Leo=44,
        Leo2=45,
        Leo3=46,
        Leo4=47,
        Tom=48,
        Tom2=49,
        Tom3=50,
        Tom4=51,
        BachDinh=52,
        TamDo=53,
        BachThu=54,
        BachThuChi=55,
        CoChiu=56,
        CoChiu2=57,
        CoChiu3=58,
        CoChiu4=59,
        CoThienKhai=60,
        CoThienKhai2=61,
        CoThienKhai3=62,
        CoThienKhai4=63,
        CoThienKhai5 = 64,
        ChiuU=65,
        UBon=66,
        ThienU=67,
        DiaU=68,
        ThapThanh=69,
        KinhTuChi=70,
        HoaDoiCuaPhat=71,
        NhaLauXeHoi=72,
        NhaLauXeHoiHoaDoiCuaPhat=73,
        NguOngBatCa=74,
        CaLoiSanDinh=75,
        CaNhayDauThuyen=76,
        CuoDatTrongHoa=77,
        BachThuChiuUChi=78,
        BachThuUBonChi=79,
        ChiuDiaU=80,
        DiaUBon=81,


        // ran dom
      //(dangh ngu va)n
        DanhChiNgoiChoChiDung = 82,
        DanhChiDungChoChiNgoi = 83,//chi chi
        BoTomLeoOmLeo = 84,//danh tam van tam sach hoac that vvan
        BatVangoiNhiVan =85, //danh bat van
        NhiVanGoiBatVan=86,//(Đánh nhị văn)
        ThatSachGoiTamVan = 87,//(danh tam van)
        TamVanGoiThatSach = 88,// danh tam van
        ThuMotConXemSao=89,
        LenXeLaPhong=90,
        TayDo=91,
        CauDay=92,
        DinhMucRoi=93,
        DiMotMinhCayDa=94,

        AnChoSo = 99,
        ChayThoiKoNhaTrenUDe = 108,
        ChiuCHiu=100,
        VaoGaDi = 95,
        Nhai = 96,
        HoaCaLang = 97,
        BatBaoNgoiImNhe = 98,
        URoi=105,
      
        ChayDiDau=107,

        ThanToOngCoTiHaiSan=109,
        ThanToOng=110,

        UConBat=113,
        UConLuc=114,

        CaoTayBoc=123,
        KhiDiVoDanCaoTayBoc=124,
        UCuaMinh=125,
        NhiVan=126,

        // audio random
        ChaCoVeoGi=129,
        MuoiCHan=130,
        HaiTom=131,
        HaiLeo=132,
        NhoGanConChiCHi=133,
        NhoGanConCuSach=134,
        NhoGanConTamSach=135,
        NhoGanConTamVan=136,
        UConNgu=140,
        UConTam=141,
        UConThat=142,
        NhoGanCon=143,
        NhoXaCon=144,
        CoAnBon=145,
        HaiAnBon=146,
        BaAnBon=147,
        BonAnBon=148,
        ChiBachThu = 149,
        AnGiDanhDay = 150,
        AnLuon = 151,
        BaoLangKoChan = 152,
        BatTut = 153,
        Cai = 154,
        CamOnCon = 155,
        CauLai = 156,
        DanhCaiLayAiMaU = 157,
        DeDuoiAn = 158,
        DuoiDung = 159,
        DuoiLuon = 160,
        Duoi = 161,
        HetDoRoi = 162,
        LucTien = 163,
        NhanhLenNao = 164,
        NhatChiChi = 165,
        NhatCuu = 166,
        NhatNguu = 167,
        NhiSach = 168,
        NhiTien = 169,
        QuaChiXet = 170,
        TamDoi = 171,
        TatNuocTheoMua = 172,
        ThatDoi = 173,
        TreoTranhTraiVimNgiAnTien = 174,
        TuTut = 175,

        heo = 176,
        danh_khong_lai_thoi = 177,
        dem_la = 178,
        doi_heo = 179,
        hang_dau_chat_di = 180,
        hang_day = 181,
        luc_phe_bon = 182,
        nhan_ba_len = 183,
        san_bang_tat_ca = 184, // sảnh từ 6 đến 9 
        sanh_3 = 185,
        sanh_4 = 186,
        sanh_5 = 187,
        sanh_6 = 188,
        sanh_7 = 189,
        sanh_8 = 190,
        sanh_9 = 191,
        sanh_10 = 192,
        sanh_rong = 193,
        sanh_tu_3_den_at = 194,
        song_bang_tinh_cam = 195, // sảnh từ 6 đến 9 
        tu_quy = 196,
        tu_quy_dau = 197,
        tu_quy_day = 198,
        tu_quy_heo = 199,
        xin_con_heo = 200,
        xin_doi_heo = 201,

        AnDi = 202,
        AnDiKhongMom = 203,
        Den3Cay = 204,
        DenAnChot = 205,
        AnCay = 206,
        AnChot = 207,
        Cau = 208,
        ToDanhBeGui = 209,
        UTron = 210,
        UXuong = 211,
        XaoKhan = 212
    }
    public AudioSource sourceCardDraw;
    public AudioSource sourceBackground;
    public AudioSource sourceEffect;
    public AudioSource AudioSourceCardsourceEffect;
    public AudioClip[] clipEffect;
    public AudioClip[] cardDrawEffect;
    public List<AudioClip> clipBackground;
    public AudioClip[] AudioClipCardSoundEffect;

    float _length = 0f;
    public static float LastPlayLength { get { return Instance._length; } }
    
    public void Play(SoundEffect type, CallBackFunction callback)
    {
        StartCoroutine(_Play(type, callback));
    }

    System.Collections.IEnumerator _Play(SoundEffect type, CallBackFunction callback)
    {
        Play(type);
        while (sourceEffect.isPlaying)
            yield return new WaitForEndOfFrame();
        callback();
    }

    public void Play(SoundEffect type)
    {
        Play(type, EPlayState.normal);
    }
    public void Play(SoundEffect type, EPlayState state)
    {
        if (oldState == EPlayState.force && sourceEffect.isPlaying)
            return;

        if(state == EPlayState.force)
            sourceEffect.Stop();

        _length = clipEffect[(int)type].length;
        sourceEffect.clip = clipEffect[(int)type];
        sourceEffect.loop = false;
        switch(state)
        {
            case EPlayState.fadingOut:
                sTween.audioTo(gameObject, fadeTime, 0, 0, null);
                break;
            case EPlayState.loop:
                sourceEffect.loop = true;
                break;
        }
        sourceEffect.Play();
        oldState = state;
    }
    public void PlayCardSound(CipCardSoundEffect type)
    {
        PlayCardSound(type, EPlayState.normal);

    }
    public void PlayCardSound(CipCardSoundEffect type, EPlayState state)
    {
        if (oldState == EPlayState.force && AudioSourceCardsourceEffect.isPlaying)
            return;
        if (state == EPlayState.force)
            AudioSourceCardsourceEffect.Stop();
        _length = AudioClipCardSoundEffect[(int)type].length;
        AudioSourceCardsourceEffect.clip = AudioClipCardSoundEffect[(int)type];
        AudioSourceCardsourceEffect.loop = false;
        switch (state)
        {
            case EPlayState.fadingOut:
                sTween.audioTo(gameObject, fadeTime, 0, 0, null);
                break;
            case EPlayState.loop:
                AudioSourceCardsourceEffect.loop = true;
                break;
        }
        AudioSourceCardsourceEffect.Play();
        oldState = state;
    }

    public void PlaySoundDrawCard(int type)
    {
        if(!AudioManager.Instance.sourceEffect.mute)
            PlaySoundDrawCard(type, EPlayState.normal);
    }
    public void PlaySoundDrawCard(int type, EPlayState state)
    {
        if (oldState == EPlayState.force && sourceCardDraw.isPlaying)
            return;

        if (state == EPlayState.force)
            sourceCardDraw.Stop();

        _length = cardDrawEffect[type].length;
        sourceCardDraw.clip = cardDrawEffect[(int)type];
        sourceCardDraw.loop = false;
        switch (state)
        {
            case EPlayState.fadingOut:
                sTween.audioTo(gameObject, fadeTime, 0, 0, null);
                break;
            case EPlayState.loop:
                sourceCardDraw.loop = true;
                break;
        }
        sourceCardDraw.Play();
        oldState = state;
    }

    public void StopSoundDraw(int type)
    {
        if (sourceCardDraw.isPlaying && sourceEffect.clip == cardDrawEffect[(int)type])
            sourceCardDraw.Stop();
    }


    public void Stop(SoundEffect type)
    {
        if (sourceEffect.isPlaying && sourceEffect.clip == clipEffect[(int)type])
            sourceEffect.Stop();
    }

    public void PlayBackground()
    {
        //sourceBackground.clip = clipBackground[0];
        //sourceBackground.loop = true;
        //sourceBackground.Play();
    }

    public void SetVolumeBackground(bool _isMute)
    {
        sourceBackground.mute = _isMute;

        StoreGame.SaveInt(StoreGame.EType.SOUND_BACKGROUND, _isMute ? (int)StoreGame.EToggle.OFF : (int)StoreGame.EToggle.ON);
    }

    public void SetVolumeEffect(bool _isMute)
    {
        sourceEffect.mute = _isMute;

        StoreGame.SaveInt(StoreGame.EType.SOUND_EFFECT, _isMute ? (int)StoreGame.EToggle.OFF : (int)StoreGame.EToggle.ON);
    }

    

    float valueTime;
    int indexSound = 0;
    int[] SoundXuong;
    public void PlaySounXuong(int[] SoundXuong,int caridU)
    {
        bool flag = true;
        if (SoundXuong.Length != 0)
        {
            if (SoundXuong.Length == 1)
            {
                if (SoundXuong[0] == 42 || SoundXuong[0] == 52)
                {
                    SoundGameplay.Instances.ShowAudioOther(SoundXuong[0],0);
                }
                else if (SoundXuong[0] == 43)
                {
                    int[] arrSoundUXuong = { 18, 19, 20, 12, 3, 14, 9, 10, 11, 3, 4, 5, 15, 16, 17 };
                    for (int i = 0; i < arrSoundUXuong.Length; i++)
                    {
                        if (caridU == arrSoundUXuong[i])
                        {
                            getCardU(caridU);
                            flag = true;
                            break;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    if (!flag)
                    {
                        SoundGameplay.Instances.RandomNumberAndPlaySound(new int[] { 43, 129 });
                    }
                }
                else if (SoundXuong[0]==69)
                {
                    SoundGameplay.Instances.RandomNumberAndPlaySound(new int[] { 69, 130 });
                }
                else if (SoundXuong[0] == (int)AudioManager.SoundEffect.Leo2)
                {
                    SoundGameplay.Instances.RandomNumberAndPlaySound(new  int[] { 45, 132 });
                }
                else if (SoundXuong[0] == (int)AudioManager.SoundEffect.Tom2)
                {
                    SoundGameplay.Instances.RandomNumberAndPlaySound(new int[] { 49, 131 });
                }
                else if (SoundXuong[0] == (int)AudioManager.SoundEffect.BachThuChi)
                {
                    SoundGameplay.Instances.RandomNumberAndPlaySound(new int[] { (int)AudioManager.SoundEffect.ChiBachThu,
                        (int) AudioManager.SoundEffect.BachThuChi });
                }
                else
                {
                    this.SoundXuong = SoundXuong;
                    AudioManager.Instance.Play((SoundEffect)this.SoundXuong[0]);
                    valueTime = AudioManager.LastPlayLength;
                    UpdateTime();
                }
            }

            else
            {
                this.SoundXuong = SoundXuong;
                AudioManager.Instance.Play((SoundEffect)this.SoundXuong[0]);

                valueTime = AudioManager.LastPlayLength;
                UpdateTime();
            }
        }
    }

    void UpdateTime()
    {
        valueTime -= Time.deltaTime;       
        if (valueTime > 0)
        {
            Invoke("UpdateTime", Time.deltaTime);
        }
        else
        {
            ++indexSound;
            
            if (indexSound < this.SoundXuong.Length)
            {
                AudioManager.Instance.Play((SoundEffect)this.SoundXuong[indexSound]);
                valueTime = AudioManager.LastPlayLength;
                Invoke("UpdateTime", Time.deltaTime);
            }
            else
            {
                indexSound = 0;
                return;
            }
        }
    }
    int cardIDNhoXaAndNhoGan = 0;
    float soundvalue = 0;
    public void playSounUXaAndUGan(int cardID,bool nhoXa)
    {
        cardIDNhoXaAndNhoGan = cardID;
        if (nhoXa)
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.NhoXaCon);
            soundvalue = AudioManager.LastPlayLength;
            updateSound();
        }
        else
        {
            AudioManager.Instance.Play(AudioManager.SoundEffect.NhoGanCon);
            soundvalue = AudioManager.LastPlayLength;
            updateSound();
        }
    }
   
    void updateSound()
    {
        soundvalue -= Time.deltaTime;
        if (soundvalue > 0)
        {
            Invoke("updateSound", Time.deltaTime);
        }
        else
        {
            //SoundGameplay.Instances.PlaySuondBaiU(SoundGameplay.Instances.soundCardId);
            SoundGameplay.Instances.PlayerShowCard(cardIDNhoXaAndNhoGan, false);
        }
    }

    void getCardU(int cardID)
    {
        switch ((GameUtil.ListCardID) cardID)
        {
            case GameUtil.ListCardID.BAT_VAN_J:
            case GameUtil.ListCardID.BAT_VAN_W:
            case GameUtil.ListCardID.BAT_SACH:
                AudioManager.Instance.Play(AudioManager.SoundEffect.UConBat);
                break;
            case GameUtil.ListCardID.LUC_VAN_J:
            case GameUtil.ListCardID.LUC_VAN_W:
            case GameUtil.ListCardID.LUC_SACH:
                AudioManager.Instance.Play(AudioManager.SoundEffect.UConLuc);
                break;
            case GameUtil.ListCardID.NGU_VAN_J:
            case GameUtil.ListCardID.NGU_VAN_W:
            case GameUtil.ListCardID.NGU_SACH:
                AudioManager.Instance.Play(AudioManager.SoundEffect.UConNgu);
                break;
            case GameUtil.ListCardID.TAM_SACH:
            case GameUtil.ListCardID.TAM_VAN_J:
            case GameUtil.ListCardID.TAM_VAN_W:
                AudioManager.Instance.Play(AudioManager.SoundEffect.UConTam);
                break;
            case GameUtil.ListCardID.THAT_SACH:
            case GameUtil.ListCardID.THAT_VAN_J:
            case GameUtil.ListCardID.THAT_VAN_W:
                AudioManager.Instance.Play(AudioManager.SoundEffect.UConThat);
                break;
        }
    }
    float soundLength = 0;
    int cardIdBocCai = 0;
    public void playSoundBocCai(int cardID,AudioManager.SoundEffect soundEffect)
    {
        cardIdBocCai = cardID;
        AudioManager.Instance.Play(soundEffect);
        soundLength = AudioManager.LastPlayLength;
        updateSoundBocCai();

    }

    void updateSoundBocCai()
    {
        soundLength -= Time.deltaTime;
        if (soundLength > 0)
        {
            Invoke("updateSoundBocCai", Time.deltaTime);
        }
        else
        {
            SoundGameplay.Instances.PlayerShowCard(cardIdBocCai, false);
        }
    }

}

public class AttributeCardDrawSound : System.Attribute
{
    int _soundIndex;
    int _cardId;
    int _indexSoundOfCard;

    public int IndexSoundOfCard
    {
        get { return _indexSoundOfCard; }
        set { _indexSoundOfCard = value; }
    }
    public int CardId
    {
        get { return _cardId; }
        set { _cardId = value; }
    }
    public int SoundId
    {
        get { return _soundIndex; }
        set { _soundIndex = value; }
    }
    public AttributeCardDrawSound(int cardId, int soundIndex,int indexSoundOfCard)
    {
        _soundIndex = soundIndex;
        _cardId = cardId;
        _indexSoundOfCard = indexSoundOfCard;
    }
    //
  
}
