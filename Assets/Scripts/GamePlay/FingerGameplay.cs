using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerGameplay : MonoBehaviour
{
    /// <summary>
    /// Card bị kéo thả
    /// </summary>
    [HideInInspector]
    GameObject dragObject;
    [HideInInspector]
    ChanCard lastPickCard;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if((GameModelChan.CurrentState == GameModelChan.EGameState.playerFullLaying || GameModelChan.MiniState == GameModelChan.EGameStateMini.wait_player_xuong) && GameModelChan.game.isViewCompleted && GameModelChan.ListGameObjectNoc.Count > 0){
                GameModelChan.DestroyViewNoc();
                GameModelChan.game.deck.transform.FindChild("2. Card").gameObject.SetActive(true);
                GameModelChan.game.isViewCompleted = !GameModelChan.game.isViewCompleted;
            }
        }
    }
    public Vector3 GetWorldPos(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        float t = -ray.origin.z / ray.direction.z;
        Vector3 pos = ray.GetPoint(t);
        pos.z = -1;
        return pos;
    }

    //void OnDragCardChan(DragGesture gesture)
    //{
    //    if (gesture.Phase == ContinuousGesturePhase.Ended)
    //    {
    //        if (lastPickCard != null)
    //            GameModel.game.CardClick(lastPickCard);
    //    }
    //    if (gesture.Selection == dragObject) return;
    //    dragObject = gesture.Selection;
    //    if (dragObject == null) return;

    //    if (dragObject.GetComponent<CardTexture>() == null) return;

    //    lastPickCard = dragObject.GetComponent<CardTexture>().card;

    //    GameModel.game.UpdateHand(dragObject);
    //}
}
