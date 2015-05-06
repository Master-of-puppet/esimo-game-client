using UnityEngine;
using System.Collections;

public class TongQuan : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        LoadPanelTongQuan();
    }

    void Start()
    {
        //this.gameObject.GetComponent<UITable>().Reposition();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RePositionPanelTongQuan(){

    }

    private void LoadPanelTongQuan()
    {

        for (int i = 0; i < 4; i++)
        {
            GameObject panel = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Giaidau/PanelTongQuanPrefabs"));
            panel.name = i.ToString() + "a PanelTongQuanPrefabs";
            panel.transform.parent = this.transform;
            panel.transform.localScale = Vector3.one;
            panel.transform.position = Vector3.zero;

            for (int j = 0; j < 6; j++)
            {
                GameObject BanChoi = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Giaidau/BanChoiReviewPrefabs"));
                BanChoi.name = j.ToString() + "Ban Choi";
                BanChoi.transform.parent = panel.transform.FindChild("Grid Tween").transform;
                BanChoi.transform.localScale = Vector3.one;
            }
        }
        //this.GetComponent<UITable>().Reposition();
        
    }
}
