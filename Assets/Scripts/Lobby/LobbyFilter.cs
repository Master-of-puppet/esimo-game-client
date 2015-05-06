using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LobbyFilter : MonoBehaviour
{

    #region Unity Editor
    public CUIHandle btnClose, btnCancel;
    public UIInput txtLobby;
    public UITable tableBetValue;
    public UIToggle[] mainRule;
    public UIToggle[] numberUser;
    public UIToggle[] typePlay;
    public UIToggle[] betMoney;
    public GameObject btnUserOnline;
    #endregion

    [HideInInspector]
    [SerializeField]
    public event CallBackFunction FilterCallBack;
    public event CallBackFunction CancelFilterCallBack;
    private List<int> mainRuleIds = new List<int>();
    private List<int> numberUserIds = new List<int>();
    private List<int> betValue = new List<int>();
    bool? isPlaying = null;
    public bool isButtonCancelClick = false;
    [HideInInspector]
    [SerializeField]
    public List<Criteria> criterias = new List<Criteria>();

    void OnEnable()
    {
        gameObject.AddComponent<CUIPopup>().buttonClose = OnClickButtonClose;
    }
    void OnDisable() {
        if (gameObject.GetComponent<CUIPopup>() != null)
            GameObject.Destroy(gameObject.GetComponent<CUIPopup>());
    }
    void Awake()
    {
        CUIHandle.AddClick(btnClose, OnClickButtonClose);
        CUIHandle.AddClick(btnCancel, OnClickButtonCancel);
    }

    void OnDestroy()
    {
        CUIHandle.RemoveClick(btnClose, OnClickButtonClose);
        CUIHandle.RemoveClick(btnCancel, OnClickButtonCancel);
    }
    void Start()
    {
        
        for (int i = 0; i < betMoney.Length; i++)
        {
            if (i >((ChannelChan)GameManager.Instance.selectedChannel).bettingValues.Length - 1)
            {
                GameObject.Destroy(betMoney[i].gameObject);
                betMoney[i] = null;
            }
            else
            {
                if (((ChannelChan)GameManager.Instance.selectedChannel).bettingValues[i] == ChannelChan.CHANNEL_BILLIONAIRE_BETTING_OTHER_VALUE)
                {
                    GameObject.Destroy(betMoney[i].gameObject);
                    betMoney[i] = null;
                }
                else
                {
                    betMoney[i].name = Convert.ToString(((ChannelChan)GameManager.Instance.selectedChannel).bettingValues[i]);
                    betMoney[i].gameObject.transform.FindChild("Label").gameObject.GetComponent<UILabel>().text = string.Format("{0:0,0}", ((ChannelChan)GameManager.Instance.selectedChannel).bettingValues[i]).Replace(",", ".");
                    Utility.AddCollider(betMoney[i].gameObject);
                }
            }
        }
        tableBetValue.Reposition();
    }

    private void OnClickButtonClose(GameObject targetObject)
    {
        gameObject.SetActive(false);
        NGUITools.SetActive(btnUserOnline, true);
    }
    private void OnClickButtonCancel(GameObject targetObject)
    {
        if (CancelFilterCallBack != null)
            CancelFilterCallBack();
        Array.ForEach<UIToggle>(mainRule, m => m.value = false);
        Array.ForEach<UIToggle>(betMoney, m => { if (m != null) m.value = false; });
        Array.ForEach<UIToggle>(numberUser, m => m.value = false);
        Array.ForEach<UIToggle>(typePlay, m => m.value = false);
        txtLobby.value = "";
        mainRuleIds = new List<int>();
        numberUserIds = new List<int>();
        betValue = new List<int>();
        isPlaying = null;
    }
    public void OnTextSubmit()
    {
        if (enabled && !isButtonCancelClick)
        {
            BuildCriteria();
            if (FilterCallBack != null)
                FilterCallBack();
        }
    }
    public void MainRuleValueChange()
    {
        if (enabled && !isButtonCancelClick)
        {
            int i = (int)Char.GetNumericValue(UIToggle.current.name[0]);
            if (UIToggle.current.value)
            {
                if (!mainRuleIds.Contains(i))
                    mainRuleIds.Add(i);
            }
            else
            {
                if (mainRuleIds.Contains(i))
                    mainRuleIds.Remove(i);
            }
            BuildCriteria();
            if (FilterCallBack != null)
                FilterCallBack();

        }
    }
    public void NumberUserValueChange()
    {

        if (enabled && !isButtonCancelClick)
        {
            int i = (int)Char.GetNumericValue(UIToggle.current.name[0]);
            if (UIToggle.current.value)
            {
                if (!numberUserIds.Contains(i))
                    numberUserIds.Add(i);
            }
            else
            {
                if (numberUserIds.Contains(i))
                    numberUserIds.Remove(i);
            }
            BuildCriteria();
            if (FilterCallBack != null)
                FilterCallBack();

        }
    }
    public void BetMoneyValueChange()
    {
        if (enabled && !isButtonCancelClick)
        {
            int i = Convert.ToInt32(UIToggle.current.name);
            if (UIToggle.current.value)
            {
                if (!betValue.Contains(i))
                    betValue.Add(i);
            }
            else
            {
                if (betValue.Contains(i))
                    betValue.Remove(i);
            }
            BuildCriteria();
            if (FilterCallBack != null)
                FilterCallBack();
        }
    }
    public void TypeLobbyValueChange()
    {
        if (enabled && !isButtonCancelClick)
        {
            if (UIToggle.current.value)
                isPlaying = true;
            else
                isPlaying = null;
            BuildCriteria();
            if (FilterCallBack != null)
                FilterCallBack();

        }
    }

    public void BuildCriteria()
    {
        criterias.Clear();
        Dictionary<string, object> dict = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(txtLobby.value))
            try
            {
                int numberRoom = Convert.ToInt32(txtLobby.value.ToLower());
                dict.Add(Fields.LobbyFilter.KEY_ROOM_NUMBER, numberRoom);
            }
            catch (Exception ex)
            {
                dict.Add(Fields.LobbyFilter.KEY_NAME, txtLobby.value.ToLower());
            }
        if (betValue.Count !=0)
            dict.Add(Fields.LobbyFilter.KEY_MONEY, betValue);
        if (mainRuleIds.Count != 0)
            dict.Add(Fields.LobbyFilter.KEY_RULE, mainRuleIds);
        if (numberUserIds.Count != 0)
            dict.Add(Fields.LobbyFilter.KEY_USER, numberUserIds);
        if (isPlaying !=null)
            dict.Add(Fields.LobbyFilter.KEY_TYPE, isPlaying);
        criterias = Utility.ComparableCriteria.buildCriteria(dict);
    }
}
