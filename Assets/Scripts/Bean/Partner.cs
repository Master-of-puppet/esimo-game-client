using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class Partner
{
    public Partner(Hashtable obj)
    {
        if (obj["id"] !=null)
            this.id = obj["id"].ToString();
        if (obj["code_identifier"] != null)
            this.codeIdentifier = obj["code_identifier"].ToString();
        if (obj["name"] !=null)
            this.name = obj["name"].ToString();
        if (obj["address"] != null)
            this.address = obj["address"].ToString();
        if (obj["tel"] !=null)
            this.tel = obj["tel"].ToString();
        if (obj["mobile"] !=null)
            this.mobile = obj["mobile"].ToString();
        if (obj["email"] != null)
            this.email = obj["email"].ToString();
        if (obj["website"] != null)
            this.website = obj["website"].ToString();
    }
    private string id;

    public string Id
    {
        get { return id; }
    }
    private string codeIdentifier;

    public string CodeIdentifier
    {
        get { return codeIdentifier; }
    }
    private string name;

    public string Name
    {
        get { return name; }
    }
    private string address;

    public string Address
    {
        get { return address; }
    }
    private string tel;

    public string Tel
    {
        get { return tel; }
    }
    private string mobile;

    public string Mobile
    {
        get { return mobile; }
    }
    private string email;

    public string Email
    {
        get { return email; }
    }
    private string website;

    public string Website
    {
        get { return website; }
    }
}

