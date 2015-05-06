using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Component để xử lý các object có thể kéo thả
/// </summary>
public class CUIDraggableObject : MonoBehaviour
{
    #region Unity Editor
    public EType type = EType.CHANGE_AVATAR;
    #endregion

    public enum EType
    {
        CHANGE_AVATAR
    }

    private bool mIsDragging;
    private Vector3 oldPosition = Vector3.zero;
    private GameObject oldObject;
    void OnDrag(Vector2 delta)
    {
        if (GameManager.CurrentScene==ESceneName.GameplayChan)
        {
            if (GameModelChan.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt) == null)
                return;
        }
        else if (GameManager.CurrentScene==ESceneName.GameplayTLMN)
        {
            if (GameModelTLMN.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt) == null)
                return;
        }
        else if (GameManager.CurrentScene==ESceneName.GameplayPhom)
        {
            if (GameModelPhom.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt) == null)
                return;
        }
        
        if (enabled && UICamera.currentTouchID > -2)
        {
            if (!mIsDragging)
            {
                mIsDragging = true;

                Vector3 pos = transform.localPosition;

                if (type == EType.CHANGE_AVATAR)
                    pos.z = -2;
                else
                    pos.z = 0f;

                transform.localPosition = pos;
				transform.parent.FindChild("Background").gameObject.SetActive(true);
            }
            else{
                Collider col = UICamera.lastHit.collider;
                if (col.gameObject.GetComponent<CUIDraggableObject>() != null)
                {
                    if (oldObject == null)
                    {
                        col.gameObject.transform.parent.FindChild("Background").gameObject.SetActive(true);
                        oldObject = col.gameObject;
                    }
                    else
                    {
                        oldObject.transform.parent.FindChild("Background").gameObject.SetActive(false);
                        if (oldObject.GetComponent<UIContainerAnonymous>().valueInt != col.gameObject.GetComponent<UIContainerAnonymous>().valueInt)
                        {
                            oldObject.transform.parent.FindChild("Background").gameObject.SetActive(false);
                            col.gameObject.transform.parent.FindChild("Background").gameObject.SetActive(true);
                            oldObject = col.gameObject;
                        }
                        else
                            oldObject.transform.parent.FindChild("Background").gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (oldObject != null && oldObject.transform.parent.FindChild("Background").gameObject.activeSelf)
                    {
                        oldObject.transform.parent.FindChild("Background").gameObject.SetActive(false);
                        oldObject = null;
                    }
                }
                transform.localPosition += new Vector3(delta.x * 960 / Screen.width, delta.y * 640/ Screen.height, 0f);
			}
        }
    }
    bool isShowDialog = false;
    void OnPress(bool isPressed)
    {
        if (enabled)
        {
            mIsDragging = false;
            Collider col = collider;
            if (col != null) col.enabled = !isPressed;
            if (!isPressed)
                Drop();
            else
                oldPosition = transform.localPosition;
        }
    }

    void Drop()
    {
        if (GameManager.CurrentScene == ESceneName.GameplayChan)
        {
            if (GameModelChan.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt) == null)
                return;
        }
        else if (GameManager.CurrentScene == ESceneName.GameplayTLMN)
        {
            if (GameModelTLMN.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt) == null)
                return;
        }
        else if (GameManager.CurrentScene == ESceneName.GameplayPhom)
        {
            if (GameModelPhom.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt) == null)
                return;
        }

        transform.localPosition = oldPosition;
        transform.parent.FindChild("Background").gameObject.SetActive(false);
        oldObject = null;
        Collider col = UICamera.lastHit.collider;
        if (col.gameObject.GetComponent<CUIDraggableObject>() == null) return;

        string userName = "";
        if (GameManager.CurrentScene == ESceneName.GameplayChan)
            userName = GameModelChan.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt).username;
        else if (GameManager.CurrentScene == ESceneName.GameplayTLMN)
            userName = GameModelTLMN.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt).username;
        else if (GameManager.CurrentScene == ESceneName.GameplayPhom)
            userName = GameModelPhom.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt).username;

        if (type == EType.CHANGE_AVATAR)
        {
			col.transform.parent.FindChild("Background").gameObject.SetActive(false);
            GameManager.Server.DoRequestPluginGame(Utility.SetEsObject("changePlayerSlot", new object[] { 
                "slotId", col.gameObject.GetComponent<UIContainerAnonymous>().valueInt,
                //"player", GameModelChan.GetPlayer(gameObject.GetComponent<UIContainerAnonymous>().valueInt).username
                "player", userName
            }));
           
        }
    }
}
