using UnityEngine;
using System.Collections;

public enum RechargeCard
{
    [AttributeRechargeCard("Thẻ cào MOBIPHONE", "VMS", @"^([a-zA-Z0-9]{15})$", @"^([0-9]{12})$",
        "Seri không hợp lệ. Seri phải từ 15 ký tự số!", "Mã thẻ không hợp lệ. Mã thẻ phải từ 12 ký tự số!"
        )]
    MOBIFONE,
    [AttributeRechargeCard("Thẻ cào VIETTEL", "VIETTEL", @"^([a-zA-Z0-9]{11})$", @"^([0-9]{13})$",
        "Seri không hợp lệ. Seri phải từ 11 ký tự số!", "Mã thẻ không hợp lệ. Mã thẻ phải từ 13 ký tự số!"
        )]
    VIETTEL,
    [AttributeRechargeCard("Thẻ cào VINAPHONE", "VNP", @"^([a-zA-Z0-9]{9})$", @"^([0-9]{14})$",
        "Seri không hợp lệ. Seri phải từ 9 ký tự số!", "Mã thẻ không hợp lệ. Mã thẻ phải từ 14 ký tự số!"
        )]
    VINAPHONE,
    [AttributeRechargeCard("Thẻ cào VCOIN", "VCOIN", @"^([a-zA-Z0-9]{9})$", @"^([0-9]{14})$",
       "Seri không hợp lệ. Seri phải từ 9 ký tự số!", "Mã thẻ không hợp lệ. Mã thẻ phải từ 14 ký tự số!"
       )]
    VCOIN,
    [AttributeRechargeCard("Thẻ cào GATE", "GATE", @"^([a-zA-Z0-9]{9})$", @"^([0-9]{14})$",
       "Seri không hợp lệ. Seri phải từ 9 ký tự số!", "Mã thẻ không hợp lệ. Mã thẻ phải từ 14 ký tự số!"
       )]
    GATE,
    none,

}
public class PanelRechargeCard : MonoBehaviour
{
    #region Unity Editor
    public UIInput serial, cardCode;
    public CUIHandle btnClose, btnSubmit;
    public GameObject parent;
    public UILabel title;
    #endregion
    [HideInInspector]
    private RechargeModel model;

    public RechargeModel Model
    {
        get { return model; }
        set
        {
            if (null == value || model == value)
            {
                return;
            }
            model = value;
			title.text = "Nạp chip qua thẻ cào " + model.Provider;
        }
    }
    void Awake()
    {
        CUIHandle.AddClick(btnClose, parent.GetComponent<RechargePopup>().OnClickClose);
        CUIHandle.AddClick(btnSubmit, OnClickButtonSubmit);
    }
    void Start()
    {
    }
    void OnDestroy()
    {
        CUIHandle.AddClick(btnClose, parent.GetComponent<RechargePopup>().OnClickClose);
        CUIHandle.AddClick(btnSubmit, OnClickButtonSubmit);
    }
    bool flag = false;
    private void OnClickButtonSubmit(GameObject targetObject)
    {
        if (flag) return;

        if (serial.value.Length == 0)
            NotificationView.ShowMessage("Seri thẻ cào không hợp lệ. Bạn chưa nhập seri thẻ cào!");
        else if (cardCode.value.Length == 0)
            NotificationView.ShowMessage("Mã thẻ cào không hợp lệ. Bạn chưa nhập Mã thẻ cào!");
        else
        {
            flag = true;
            Execute();
        }
    }



    public void Execute()
    {
        WaitingView.Show("Chờ xử lý");
        AttributeRechargeCard attribute = Utility.EnumUtility.GetAttribute<AttributeRechargeCard>(model.ETypeCard);
        ServerWeb.StartThread(ServerWeb.URL_SEND_RECHARGE, new object[] { "username", GameManager.Instance.mInfo.username, "txtSoSeri", serial.value, "txtSoPin", 
            cardCode.value, "select_method", attribute.text_id }, ProcessAfterRecharge);
        flag = false;
    }

    void ProcessAfterRecharge(bool isDone, WWW response, IDictionary json)
    {
        if (isDone && json.Contains("code"))
        {
            if (json.Contains("code") && json["code"].ToString() == "1")
            {
                serial.value = "";
                cardCode.value = "";
            }
            else
            {
                if (string.IsNullOrEmpty(json["message"].ToString()))
                    NotificationView.ShowMessage("Có lỗi trong quá trình nạp thẻ. Bạn vui lòng thử lại!");
                else
                    NotificationView.ShowMessage(json["message"].ToString());
                WaitingView.Hide();
            }
        }
        else
        {
            NotificationView.ShowMessage("Thông tin thẻ nạp không hợp lệ!");
            WaitingView.Hide();
        }
            

        flag = false;
        //WaitingView.Hide();
    }
    

}
[System.AttributeUsage(System.AttributeTargets.Field)]
public class AttributeRechargeCard : System.Attribute
{
    public string FORMAT_SERIAL;
    public string FORMAT_CODE;
    public string name;
    public string text_id;
    public string errorSeri;
    public string errorCode;

    public AttributeRechargeCard(string name, string id, string serial, string code, string errorSeri, string errorCode)
    {
        this.name = name;
        this.text_id = id;
        this.FORMAT_CODE = code;
        this.FORMAT_SERIAL = serial;
        this.errorSeri = errorSeri;
        this.errorCode = errorCode;
    }
}