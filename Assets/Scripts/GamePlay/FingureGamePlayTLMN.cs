using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FingureGamePlayTLMN : MonoBehaviour
{
    /// <summary>
    /// Card bị kéo thả
    /// </summary>
    [HideInInspector]
    public GameObject dragObject;

    int dragFingerIndex = -1;
    Vector3 beginPosition;

    void OnDragCard(DragGesture gesture)
    {
        FingerGestures.Finger finger = gesture.Fingers[0];

        if (!GameModelTLMN.DealCardDone)
            return;

        if (gesture.Phase == ContinuousGesturePhase.Started)
        {
            dragObject = gesture.Selection;

            if (dragObject == null) return;
            if (dragObject.GetComponent<ECardTexture>() == null) return;
            if (gameObject.GetComponent<iTween>() && gameObject.GetComponent<iTween>().isRunning) return;
            //set depth to top
            dragObject.GetComponent<ECardTexture>().texture.depth = 15;
            beginPosition = dragObject.transform.localPosition;
            // Lưu lại ngón đang drag
            dragFingerIndex = finger.Index;
        }
        else if (finger.Index == dragFingerIndex)  //Xử lý drag. Đảm bảo chắc là đối tượng đang kéo là đối tượng bạn đang chọn.
        {
            if (finger.Index != 0) return;

            if (gesture.Phase == ContinuousGesturePhase.Updated)
            {
                dragObject.gameObject.transform.position = GetWorldPos(gesture.Position);
            }
            else
            {
                //Thiết lập lại khi kết thúc drag
                dragFingerIndex = -1;
                dragObject.transform.localPosition = beginPosition;

                GameObject obj = gesture.PickObject(gameObject.GetComponent<ScreenRaycaster>(), gesture.Position);

                if (obj == null) return;

                if (obj.name == "MainDesk")
                {
                    if (GameModelTLMN.CanDiscard(new List<ECard>(new ECard[] { dragObject.GetComponent<ECardTexture>().card })))
                    {
                        dragObject.gameObject.transform.position = GetWorldPos(gesture.Position);
                        GameModelTLMN.game.OnDiscard(new List<ECard>(new ECard[] { dragObject.GetComponent<ECardTexture>().card }));
                    }
                }
                else
                {
                    ECard dragCard = dragObject.GetComponent<ECardTexture>().card;
                    bool isChange = false;
                    if (obj.name == "Hand-Left")
                    {
                        GameModelTLMN.YourController.mCardHand.Remove(dragCard);
                        GameModelTLMN.YourController.mCardHand.Insert(0, dragCard);
                        isChange = true;
                    }
                    else if (obj.name == "Hand-Right")
                    {
                        GameModelTLMN.YourController.mCardHand.Remove(dragCard);
                        GameModelTLMN.YourController.mCardHand.Add(dragCard);
                        isChange = true;
                    }
                    else if (obj.GetComponent<ECardTexture>() != null)
                    {
                        int indexOf = GameModelTLMN.YourController.mCardHand.IndexOf(GameModelTLMN.YourController.mCardHand.Find(c => c == obj.GetComponent<ECardTexture>().card));
                        int indexOfDrag = GameModelTLMN.YourController.mCardHand.IndexOf(GameModelTLMN.YourController.mCardHand.Find(c => c == dragCard));

                        if (indexOfDrag > indexOf && indexOf + 1 < GameModelTLMN.YourController.mCardHand.Count)
                            indexOf++;

                        GameModelTLMN.YourController.mCardHand.Remove(dragCard);
                        GameModelTLMN.YourController.mCardHand.Insert(indexOf, dragCard);
                        isChange = true;
                    }

                    if (isChange)
                    {
                        GameModelTLMN.model.GetCardCollection();
                    }
                }
                GameModelTLMN.game.UpdateHand(GameModelTLMN.YourController.slotServer, 0.5f);
                //Debug.Log(obj.name);
            }
        }
    }

    //Chuyển từ Vector2 Screen sang Vector3 Camera
    public static Vector3 GetWorldPos(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        float t = -ray.origin.z / ray.direction.z;
        Vector3 pos = ray.GetPoint(t);
        pos.z = -1;
        return pos;
    }
}

