using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameplayDealCardEffect
{
    public enum StepDealCard
    {
        NONE = 0,
        START = 1,
        WAIT_PICK = 2,
		PICKED = 3,
        WAIT_ROTATE = 4,
        DONE = 5,
    }
    
    GameObject[] listPickObject = new GameObject[5];
    public StepDealCard currentStep = StepDealCard.NONE;
    public int firstCardServerResponse = -1;
    public string firstPlayer;
    public delegate void DelegateResponseMessage();
    public static event DelegateResponseMessage EventResponseMessage;
    public void OnInit()
    {

        const float radiusOfCircleRotation = 110f;
		Vector3 vectorTop = GameModelChan.game.mPlaymat.GetPointCircle(GameModelChan.game.mPlaymat.locationRotation[4].localPosition,radiusOfCircleRotation,270f);
        GameModelChan.game.mPlaymat.locationRotation[0].localPosition = vectorTop;
        Vector3 vectorLeft = GameModelChan.game.mPlaymat.GetPointCircle(GameModelChan.game.mPlaymat.locationRotation[4].localPosition, radiusOfCircleRotation, 180f);
        GameModelChan.game.mPlaymat.locationRotation[1].localPosition = vectorLeft;
        Vector3 vectorBottom = GameModelChan.game.mPlaymat.GetPointCircle(GameModelChan.game.mPlaymat.locationRotation[4].localPosition, radiusOfCircleRotation, 90f);
        GameModelChan.game.mPlaymat.locationRotation[2].localPosition = vectorBottom;
        Vector3 vectorRight = GameModelChan.game.mPlaymat.GetPointCircle(GameModelChan.game.mPlaymat.locationRotation[4].localPosition, radiusOfCircleRotation, 0f);
        GameModelChan.game.mPlaymat.locationRotation[3].localPosition = vectorRight;

        serverRequestUpdateHandStep1 = false;
        serverRequestUpdateHandStep2 = false;
        currentStep = StepDealCard.START;
        GameModelChan.game.StartCoroutine(DealCard());
    }

    public void OnDestroy()
    {
        currentStep = StepDealCard.NONE;
        Array.ForEach<GameObject>(listPickObject, obj => GameObject.Destroy(obj));
        GameModelChan.DealCardDone = true;
    }

    IEnumerator DealCard()
    {
        float time = 0.3f;

        #region CHIA BAI
        Hashtable hash = iTween.Hash("islocal", true, "time", time, "position", Vector3.zero);

        int index = 0;
        GameObject[] oldObj = new GameObject[2];
        while (index < 2)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    ChanCard c = new ChanCard(0, -1);
                    c.Instantiate(GameModelChan.game.mPlaymat.locationDealCard[i + (j * 5)]);
                    GameObject obj = c.gameObject;

                    obj.name = "Card " + index + " " + i + " " + j + " - " + (index * (i + 1) + (j));

                    Vector3 pos;
                    if (oldObj[j] != null)
                        pos = oldObj[j].transform.parent.localPosition - GameModelChan.game.mPlaymat.locationDealCard[i + (j * 5)].localPosition;
                    else
                        pos = GameModelChan.game.mPlaymat.locationDealCard[i + (j * 5)].localPosition;

                    obj.transform.localPosition = pos;
                    obj.transform.localScale /= 1.75f;
                    obj.transform.localRotation = new Quaternion(0f, 0f, 0.9f, 0.4f);

                    iTween.MoveTo(obj, hash);
                    oldObj[j] = obj;
                }
                yield return new WaitForSeconds(time);
            }
            index++;
        }


        #endregion

        yield return new WaitForSeconds(0.5f);

        #region GỘP BÀI
        for (int i = 0; i < 5; i++)
        {
            if (i < 2)
            {
                while (GameModelChan.game.mPlaymat.locationDealCard[i + (1 * 5)].childCount > 0)
                {
                    Transform tran = GameModelChan.game.mPlaymat.locationDealCard[i + (1 * 5)].GetChild(0);
                    tran.parent = GameModelChan.game.mPlaymat.locationDealCard[i + (0 * 5)];
                    iTween.MoveTo(tran.gameObject, hash);
                }
            }
            else
            {
                while (GameModelChan.game.mPlaymat.locationDealCard[i + (0 * 5)].childCount > 0)
                {
                    Transform tran = GameModelChan.game.mPlaymat.locationDealCard[i + (0 * 5)].GetChild(0);
                    tran.parent = GameModelChan.game.mPlaymat.locationDealCard[i + (1 * 5)];
                    iTween.MoveTo(tran.gameObject, hash);
                }
            }
        }
        #endregion

        yield return new WaitForSeconds(time + 0.05f);

        #region DI CHUYỂN BÀI CHO ĐỀU
        for (int i = 0; i < 2; i++)
        {
            while (GameModelChan.game.mPlaymat.locationDealCard[i].childCount > 0)
            {
                Transform tran = GameModelChan.game.mPlaymat.locationDealCard[i].GetChild(0);
                tran.parent = GameModelChan.game.mPlaymat.locationDealCard[4 - i];
                iTween.MoveTo(tran.gameObject, hash);
            }
        }

        //Destroy hết child giữ lại 1 card
        List<GameObject> listObj = new List<GameObject>();
        foreach (Transform trans in GameModelChan.game.mPlaymat.locationDealCard)
        //for(int i=0;i<GameModel.game.mPlaymat.locationDealCard.Length;i++)
        {
            //Transform trans = GameModel.game.mPlaymat.locationDealCard[i];
            while (trans.childCount >= 2)
            {
                GameObject obj = trans.GetChild(0).gameObject;
                obj.transform.parent = null;
                GameObject.Destroy(obj);
            }

            if (trans.childCount > 0)
                listObj.Add(trans.GetChild(0).gameObject);
        }
        listPickObject[0] = listObj[0];
        listPickObject[1] = listObj[1];
        listPickObject[2] = listObj[2];
        listPickObject[3] = listObj[3];
        listPickObject[4] = listObj[4];
        #endregion

        currentStep = StepDealCard.WAIT_PICK;
        if (EventResponseMessage != null)
            EventResponseMessage();
    }

    bool serverRequestUpdateHandStep1 = false;
    bool serverRequestUpdateHandStep2 = false;
    int saveFirstPick;
    public void Pick(int firstPick, int donePick)
    {
        if (currentStep != StepDealCard.DONE)
        {
            //Server yêu cầu fist pick
            if (firstPick >= 0)
            {
                CardClick(listPickObject[firstPick].GetComponent<ECardTexture>().card);
                serverRequestUpdateHandStep1 = true;
                saveFirstPick = firstPick;
            }
                
            //Server thông báo đã hoàn tất xong pick
            if (donePick >= 0)
            {
                //Nếu server chưa trả về first pick thì tự động pick
                if (pickAfterDealcard == null)
                {
                    System.Random rand = new System.Random();
                    int fisrtPick = -1;
                    while (fisrtPick == donePick)
                        fisrtPick = rand.Next(0, 4);
					if (listPickObject.Length > 0 && listPickObject[firstPick] != null)
                        CardClick(listPickObject[firstPick].GetComponent<ECardTexture>().card);
                }

                //Nếu bot pick phải một vị trí không tồn tại thì tự động pick lại cho bot
                if (listPickObject[donePick] == null)
                    donePick = Array.FindIndex<GameObject>(listPickObject, o => o != null);

                //Nếu trường hợp server tự động tính sai trả về firstPick == donePick thì tự động chuyển donePick bất kỳ khác
                if(saveFirstPick == donePick)
                     donePick = Array.FindIndex<GameObject>(listPickObject, o => o != null && o != pickAfterDealcard.gameObject);

                CardClick(listPickObject[donePick].GetComponent<ECardTexture>().card);
                serverRequestUpdateHandStep2 = true;
            }
        }
    }

    int indexPick = -1;
    ECard pickAfterDealcard = null;
    public void CardClick(ECard card)
    {
        if (card != null && currentStep == StepDealCard.WAIT_PICK)
        {
            if (pickAfterDealcard == null)
            {
                indexPick = Array.FindIndex<GameObject>(listPickObject, obj => obj == card.gameObject);
                pickAfterDealcard = card;
                MoveCard_FirstPick(pickAfterDealcard);
                pickAfterDealcard.gameObject.SetActive(false);
            }
            else if (pickAfterDealcard != card)
            {
                currentStep = StepDealCard.PICKED;
                GameModelChan.game.StartCoroutine(OnPickDone(card));
            }
        }
    }

    /// <summary>
    /// Di chuyển các card khi lần đầu pick
    /// </summary>
    void MoveCard_FirstPick(ECard card)
    {
        #region THÔNG BÁO VỚI SERVER LÀ ĐÃ THỰC HIỆN XONG BƯỚC 1
		if (!serverRequestUpdateHandStep1 && GameModelChan.YourController !=null && GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer)
        {
            int index = Array.FindIndex<GameObject>(listPickObject, obj => obj == card.gameObject);
            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] 
            {
	            Fields.ACTION, "firstPick", "slotChoice", Utility.SetEsObject(null, "firstPick", indexPick)
	        }));
        }
        #endregion

        int indexPicked = Array.FindIndex<GameObject>(listPickObject, obj => obj == card.gameObject);
        int currentIndex = -1;
        for (int i = 0; i < GameModelChan.game.mPlaymat.locationRotation.Length - 1; i++)
        {
            if (indexPick == i)
            {
                currentIndex = GetNextGroupCard(i);
                listPickObject[currentIndex].transform.parent = GameModelChan.game.mPlaymat.locationRotation[i];
            }
            else
            {
                if (currentIndex != -1)
                {
                    currentIndex = GetNextGroupCard(currentIndex);
                    listPickObject[currentIndex].transform.parent = GameModelChan.game.mPlaymat.locationRotation[i];
                }
                else
                    listPickObject[i].transform.parent = GameModelChan.game.mPlaymat.locationRotation[i];
            }
        }

        //Di chuyển ra giữa và destroy những cái ko cần thiết đi
        //Tới Công rảnh thì code hộ anh mấy cái đoạn trên vứt vào dưới này luôn nhé.
        DealCardRotation.GetListRotation.ForEach(dealCard =>
            {
                MoveAllChildToCenter(dealCard.transform, 0.5f);
                if (GameModelChan.ListPlayerInGame.Find(p => p.IsHasQuit == false && p.mSide == dealCard.sideStart) == null)
                    GameObject.Destroy(dealCard.transform.GetChild(0).gameObject);
            }
        );
    }

    /// <summary>
    /// Thực hiện sau khi người chơi pick lần thứ 2
    /// </summary>
    IEnumerator OnPickDone(ECard card)
    {
        #region THÔNG BÁO VỚI SERVER LÀ ĐÃ THỰC HIỆN XONG BƯỚC 2
		if (!serverRequestUpdateHandStep2 && GameModelChan.YourController !=null && GameModelChan.IndexInTurn == GameModelChan.YourController.slotServer)
        {
            int index = Array.FindIndex<GameObject>(listPickObject, obj => obj == card.gameObject);
            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] {
	            Fields.ACTION, "getHand", "slotChoice", Utility.SetEsObject(null, "donePick", index)
	        }));
        }
        #endregion

        #region CHỜ SERVER TRẢ VỀ THÔNG TIN NGƯỜI CHƠI
        while (string.IsNullOrEmpty(firstPlayer) || firstCardServerResponse == -1)
            yield return new WaitForEndOfFrame();

        pickAfterDealcard.CardId = firstCardServerResponse;
        pickAfterDealcard.gameObject.transform.parent = card.gameObject.transform.parent;
        pickAfterDealcard.gameObject.transform.localPosition = Vector3.zero;
        pickAfterDealcard.gameObject.SetActive(true);
        pickAfterDealcard.gameObject.transform.localPosition = new Vector3(pickAfterDealcard.gameObject.transform.localPosition.x, pickAfterDealcard.gameObject.transform.localPosition.y, -2f);
        iTween.RotateTo(pickAfterDealcard.gameObject, new Vector3(0f, 0f, 0f), 0.5f);
        yield return new WaitForSeconds(0.5f);
        #endregion

        List<DealCardRotation> listDealcard = new List<DealCardRotation>();

        #region QUY ĐỊNH VỊ TRÍ SẼ ĐẾN CỦA CÁC MÔ BÀI
        PlayerControllerChan playerFirst = GameModelChan.ListPlayerInGame.Find(p => p.username == firstPlayer);
        DealCardRotation dealFirst = DealCardRotation.GetListRotation.Find(d => d.transform.childCount == 2);
        listDealcard.Add(dealFirst);
        dealFirst.StartRotate(playerFirst.mSide);
        PlayerControllerChan nextPlayer = GameModelChan.GetNextPlayer(playerFirst.slotServer);
        DealCardRotation nextDeal = dealFirst.GetNextAvalible;
        for (int i = 0; i < GameModelChan.ListPlayerInGame.Count - 1; i++)
        {
            listDealcard.Add(nextDeal);
            nextDeal.StartRotate(nextPlayer.mSide);
            nextPlayer = GameModelChan.GetNextPlayer(nextPlayer.slotServer);
            nextDeal = nextDeal.GetNextAvalible;
        }
        #endregion

        while (!listDealcard.TrueForAll(d => d.isComplete))
            yield return new WaitForFixedUpdate();    
        
        #region DI CHUYỂN VỀ VỊ TRÍ CÓ AVATAR
        DealCardRotation.GetListRotation.ForEach(dealCard => {
            if (dealCard.transform.childCount > 0)
                MoveAllChildToTransform(dealCard.transform, GameModelChan.ListPlayerInGame.Find(p => p.mSide == dealCard.sideEnd).cuiPlayer.transform, 0.5f);
            }
        );
        #endregion
        yield return new WaitForSeconds(0.6f);

        currentStep = StepDealCard.DONE;
        Array.ForEach<GameObject>(listPickObject, obj => GameObject.Destroy(obj));
    }

    int GetNextGroupCard(int checkIndex)
    {
        checkIndex++;
        if (checkIndex > 4)
            checkIndex = 0;
        if (checkIndex == Array.FindIndex<GameObject>(listPickObject, obj => obj == pickAfterDealcard.gameObject))
            checkIndex = GetNextGroupCard(checkIndex++);
        return checkIndex;
    }

    void MoveAllChildToCenter(Transform parent,float time)
    {
        for(int i = 0; i < parent.childCount ;i++)
        {
            Transform tran = parent.GetChild(i);
            iTween.MoveTo(tran.gameObject, iTween.Hash("islocal", true, "time", time, "position", new Vector3(0f,0f,tran.localPosition.z)));
        }
    }

    void MoveAllChildToTransform(Transform tranFrom, Transform tranTo, float time)
    {
        while (tranFrom.childCount > 0)
        {
            Transform tran = tranFrom.GetChild(0);
            iTween.MoveTo(tran.gameObject, iTween.Hash("islocal", true, "time", time, "position", new Vector3(0f, 0f, (tranFrom.GetChild(0).gameObject == pickAfterDealcard.gameObject ? -10f : -5f))));
            tran.parent = tranTo;
        }
    }
}