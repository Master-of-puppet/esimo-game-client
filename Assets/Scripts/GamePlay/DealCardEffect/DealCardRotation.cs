using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Thực hiện việc quay bài sau khi chia xong
/// </summary>
public class DealCardRotation : MonoBehaviour
{
    public ESide sideStart;
    public ESide sideEnd;
    public DealCardRotation next;

    public float speedDealCard;
    public Transform focus;
    public bool isComplete = true;

    Vector3 toPotision = Vector3.zero;
    bool isEnableRotate = false;

    /// <summary>
    /// Thông báo bắt đầu xoay bài (nêu rõ là sẽ quay đến vị trí nào)
    /// </summary>
    /// <param name="sideEnd">Vị trí muốn đến</param>
    public void StartRotate(ESide sideEnd)
    {
        isEnableRotate = true;
        this.sideEnd = sideEnd;
        toPotision = GetListRotation.Find(d => d.sideStart == sideEnd).transform.localPosition;
        speedDealCard = 200f;

        isComplete = this.sideStart == this.sideEnd ? true : false;
    }

	void Update () 
    {
        if (isEnableRotate && sideStart != sideEnd)
        {
            if (toPotision == Vector3.zero)
                toPotision = GetListRotation.Find(d => d.sideStart == sideEnd).transform.localPosition;

            gameObject.transform.RotateAround(focus.localPosition, Vector3.forward, speedDealCard * Time.deltaTime);
		    gameObject.transform.localRotation = new Quaternion(0f,0f,0f, 1f);

            if (Vector3.SqrMagnitude(toPotision - gameObject.transform.localPosition) < 50f)
            {
                isEnableRotate = false;
                toPotision = Vector3.zero;
                isComplete = true;
            }
        }
	}
	
    public static List<DealCardRotation> GetListRotation
    {
        get
        {
            return (
                from trans in GameModelChan.game.mPlaymat.locationRotation 
                where trans.gameObject.GetComponent<DealCardRotation>() != null
                select trans.gameObject.GetComponent<DealCardRotation>()
            ).ToList<DealCardRotation>();
        }
    }

    public DealCardRotation GetNextAvalible
    {
        get
        {
            DealCardRotation isNext = this.next;
            while (isNext.transform.childCount == 0)
            {
                isNext = isNext.next;

                if (isNext == this)
                    break;
            }
            return isNext;
        }
    }
}