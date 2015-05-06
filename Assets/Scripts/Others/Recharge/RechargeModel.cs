using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


public class RechargeModel
{
    string type;
    RechargeCard eTypeCard;
    string codeValue;

    public string CodeValue
    {
        get { return codeValue; }
        set { codeValue = value; }
    }
    public RechargeCard ETypeCard
    {
        get { return eTypeCard; }
        set { eTypeCard = value; }
    }
    public string Type
    {
        get { return type; }
        set { type = value; }
    }
    string code;
    public string Code
    {
        get { return code; }
        set { code = value; }
    }
    string value;

    public string Value
    {
        get { return this.value; }
        set { this.value = value; }
    }
    string provider;

    public string Provider
    {
        get { return provider; }
        set { provider = value; }
    }
    string template;

    public string Template
    {
        get { return template; }
        set { template = value; }
    }
    string imagesUrl;
    public string ImagesUrl
    {
        get { return imagesUrl; }
        set { imagesUrl = value; }
    }
	public RechargeModel(Hashtable obj)
	{
		ParseFromHasble (obj);
	}
    public void ParseFromJson(string json)
    {
        IDictionary dic = (IDictionary)JSON.JsonDecode(json);
        this.type = dic["type"].ToString();
        this.code = dic["code"].ToString();
        this.codeValue = dic["code_value"].ToString();
        this.value= dic["value"].ToString();
        this.provider = dic["provider"].ToString();
        this.eTypeCard = ConvertTypeToEnum(this.provider);
        this.imagesUrl = ServerWeb.URL_BASE + dic["image"].ToString();
        if (dic["template"] !=null)
            this.template = dic["template"].ToString();
        
    }
	public void ParseFromHasble(Hashtable obj)
	{
		this.type = obj["type"].ToString();
		this.code = obj["code"].ToString();
		this.value= obj["value"].ToString();
        this.codeValue = obj["code_value"].ToString();
		this.provider = obj["provider"].ToString();
		this.eTypeCard = ConvertTypeToEnum(this.provider);
        this.imagesUrl = ServerWeb.URL_BASE + obj["image"].ToString();
        if (obj["template"] !=null)
            this.template = obj["template"].ToString();
	}
    public RechargeCard ConvertTypeToEnum(string name)
    {
		MemberInfo[] memberInfos = typeof(RechargeCard).GetMembers(BindingFlags.Public | BindingFlags.Static);
        for (int i = 0; i < memberInfos.Length; i++)
        {
            if (name.Equals(memberInfos[i].Name.ToLower()))
            {
                return (RechargeCard)Enum.Parse(typeof(RechargeCard), memberInfos[i].Name);
            }
        }
        return RechargeCard.none;
    }
}
