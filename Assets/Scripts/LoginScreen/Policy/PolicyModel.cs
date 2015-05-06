using UnityEngine;
using System.Collections;

public class PolicyModel {
    public PolicyModel(Hashtable obj) 
    {
        policy = obj["policy"].ToString();
        message = obj["message"].ToString();
        code = obj["code"].ToString();
        createDate = obj["create_time"].ToString();
    }

    public PolicyModel()
    {
        // TODO: Complete member initialization
    }
    private string policy;

    public string Policy
    {
        get { return policy; }
        set { policy = value; }
    }
    private string message;

    public string Message
    {
        get { return message; }
        set { message = value; }
    }
    private string code;

    public string Code
    {
        get { return code; }
        set { code = value; }
    }
    private string createDate;

    public string CreateDate
    {
        get { return createDate; }
        set { createDate = value; }
    }
}
