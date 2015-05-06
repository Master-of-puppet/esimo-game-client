using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ListCuocUView : MonoBehaviour
{
    public UITable listCuocU;
    public UILabel lblName;
    public CUIHandle btnXuong;
    public UISlider timer;

    [HideInInspector]
    public List<CuocItemView> listItem = new List<CuocItemView>();
    
    void Awake()
    {
        CUIHandle.AddClick(btnXuong, OnClickXuong);
    }
   
    void Destroy()
    {
        CUIHandle.RemoveClick(btnXuong, OnClickXuong);
    }

    public void OnClickXuong(GameObject go)
    {
        List<int> cuoc = new List<int>();
        listItem.FindAll(item => item.cuoc.countClick > 0).ForEach(item => { for (int i = 0; i < item.cuoc.countClick; i++) cuoc.Add(item.cuoc.id); });
		
		if(cuoc.Count == 0)
		{
			NotificationView.ShowMessage("Bạn vui lòng lựa chọn cước sắc trước!", 2f);
			return;	
		}
		
		GameModelChan.ShowAvatarPlayer();
        GameManager.Server.DoRequestPluginGame(Utility.SetEsObject(Fields.GAMEPLAY.PLAY, new object[] { 
            Fields.ACTION, "xuong", 
            "cuoc", cuoc.ToArray()
        }));
        GameObject.Destroy(gameObject);
    }

    void Start()
    {
        AddListCuocU();
    }

    public void listCuocXuong()
    {
        lblName.text = "";

        listItem.FindAll(i => i.cuoc.countClick > 0).ForEach(i => i.SetChildDisable());

        List<CuocUXuong> listXuong = (from item in listItem.FindAll(i => i.cuoc.countClick > 0) select item.cuoc).ToList<CuocUXuong>();
        listXuong.Sort((x1, x2) => x1.point.CompareTo(x2.point));
        listXuong.ForEach(item => lblName.text += (item.max > 2 && item.countClick > 1 ? item.countClick + " " : "") + item.name + (listXuong.FindLast(i => i.id > 0).id == item.id ? "" : " - "));
        lblName.text = lblName.processedText.Replace("\n", "\n\n");
    }
	
    void AddListCuocU()
	{
		listItem.Clear();
        //if (GameManager.Instance.ListCuocU.Count == 0)
        //{
        //    CuocUXuong[] cuoc = new CuocUXuong[19];
        //    cuoc[0] = new CuocUXuong(1, "Xuông", 1, 3, 1, new int[] { 2, 3 });
        //    cuoc[1] = new CuocUXuong(2, "Chì", 2, 3, 1, null);
        //    cuoc[2] = new CuocUXuong(3, "Thông", 3, 6, 1, null);
        //    cuoc[3] = new CuocUXuong(4, "Thiên Ù", 4, 3, 1, new int[] { 2, 3 });
        //    cuoc[4] = new CuocUXuong(5, "Địa Ù", 3, 222, 1, null);
        //    cuoc[5] = new CuocUXuong(6, "Bạch Thủ", 2, 7, 1, null);
        //    cuoc[6] = new CuocUXuong(7, "Thiên Khai", 6, 4, 1, null);
        //    cuoc[7] = new CuocUXuong(8, "Bòn", 8, 0, 1, null);
        //    cuoc[8] = new CuocUXuong(9, "Ù Bòn", 4, 0, 1, null);
        //    cuoc[9] = new CuocUXuong(10, "Có Chíu", 5, 0, 4, null);
        //    cuoc[10] = new CuocUXuong(11, "Chíu Ù", 2, 0, 1, null);
        //    cuoc[11] = new CuocUXuong(12, "Nhà lầu xe hơi hoa rơi của phật ", 5, 0, 2, null);
        //    cuoc[12] = new CuocUXuong(13, "Tôm", 3, 0, 4, null);
        //    cuoc[13] = new CuocUXuong(14, "Bạch Thủ Chi", 10, 0, 1, null);
        //    cuoc[14] = new CuocUXuong(15, "Bạch Định", 3, 0, 1, null);
        //    cuoc[15] = new CuocUXuong(16, "Tám Đỏ", 4, 0, 1, null);
        //    cuoc[16] = new CuocUXuong(17, "Thập Thành", 5, 0, 1, null);
        //    cuoc[17] = new CuocUXuong(18, "Kim Tứ Chi", 10, 0, 1, null);
        //    cuoc[18] = new CuocUXuong(19, "Hoa Rơi Cửa Phật", 15, 0, 1, null);
        //    for (int i = 0; i < cuoc.Length; i++)
        //    {
        //        CreateListCuocU(cuoc[i]);
        //    }
        //}
        //else
        foreach (CuocUXuong cuoc in GameManager.Instance.ListCuocU)
        {
            CreateListCuocU(cuoc);
        }
        listCuocU.Reposition();
		listCuocU.transform.parent.GetComponent<UIScrollView> ().UpdatePosition ();
	}

	void CreateListCuocU (CuocUXuong cuocu)
	{
		GameObject obj = (GameObject)GameObject.Instantiate (Resources.Load ("Prefabs/Gameplay/CuocU/CuocUItemPrefab"));
        obj.name = string.Format("{0:0000}", cuocu.id);
		obj.transform.parent = listCuocU.transform;
		obj.transform.localPosition = new Vector3 (0f, 0f, -1f);
		obj.transform.localScale = Vector3.one;
		CuocItemView item = obj.GetComponent<CuocItemView> ();
		item.setData (cuocu,this);
		listItem.Add (item);
        Utility.AddCollider(obj);
	}

    static ListCuocUView _instance;
    public static ListCuocUView Create(float time,float timeCountdown)
    {
        if (_instance == null)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CuocU/ListCuocUPrefab"));
            obj.name = "__ListCuocU";
            obj.transform.position = new Vector3(3, 68, -129f);
            _instance = obj.GetComponent<ListCuocUView>();
			GameModelChan.HideAvatarPlayer(false);
        }
        _instance.StartTimer(time,timeCountdown);
        return _instance;
    }

    public static ListCuocUView CreateTest()
    {
        if (_instance == null)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CuocU/ListCuocUPrefab"));
            obj.name = "__ListCuocU";
            obj.transform.position = new Vector3(3, 68, -129f);
            _instance = obj.GetComponent<ListCuocUView>();
        }
        _instance.AddListCuocU();
        return _instance;
    }
    
    public static void Close()
    {
        if (_instance != null)
            GameObject.Destroy(_instance.gameObject);
    }

    public static bool IsShowing
    {
        get { return _instance != null; }
    }

    float startTimer;
    float valueTimer;
    void StartTimer(float time, float timeCountdown)
    {
        startTimer = time;
        valueTimer = timeCountdown;
        UpdateTime();
    }

    void UpdateTime()
    {
        valueTimer -= Time.deltaTime;
        timer.value = valueTimer / startTimer;
        timer.ForceUpdate();
        if (valueTimer > 0)
            Invoke("UpdateTime", Time.deltaTime);
        else
            CancelInvoke("UpdateTime");
    }
}
