using UnityEngine;
using Electrotank.Electroserver5.Api;

public class UserTournament : MonoBehaviour
{
    public UILabel lblFullName, lblLevel, lblTimeRegister, lblResult;
    public UITexture avatarUser;

    [HideInInspector]
    public int userId, level;
    [HideInInspector]
    public string userName, fullName, avatarUrl, timeRegister,result;

    public static UserTournament Create(EsObject es,Transform parent,GameObject prefab)
    {
        GameObject obj = NGUITools.AddChild(parent.gameObject, prefab);
        UserTournament tour = obj.GetComponent<UserTournament>();
        if (es.variableExists("userId"))
            tour.userId = es.getInteger("userId");
        obj.name = "userTournament" + tour.userId;
        if (es.variableExists("userName"))
            tour.userName = es.getString("userName");
        if (es.variableExists("timeRegister"))
            tour.timeRegister = es.getString("timeRegister");
        if (es.variableExists("fullName"))
            tour.fullName = es.getString("fullName");
        if (es.variableExists("level"))
            tour.level = es.getInteger("level");
        if (es.variableExists("avatar"))
            tour.avatarUrl = es.getString("avatar");
        if (es.variableExists("result"))
            tour.result = es.getString("result");
        tour.SetData();
        return tour;
    }

    public void SetData()
    {
        lblFullName.text = string.IsNullOrEmpty(fullName) ? userName : fullName;
        lblLevel.text = level.ToString();
        lblTimeRegister.text = timeRegister;
        lblResult.text = result;
        new AvatarCacheOrDownload(avatarUrl, delegate(Texture texture)
        {
            if (texture!=null)
            {
                avatarUser.mainTexture = texture;
            }
        }, true);
    }
}
