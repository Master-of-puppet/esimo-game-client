using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Criteria : ISimilarComparable
{
    public Dictionary<string, object> iDict;
    public Criteria(Dictionary<string, object> dict)
    {
        iDict = dict;
    }
    public bool Compare(Dictionary<string, object> dicCompare)
    {
        return Utility.ComparableCriteria.IsMatchCriteria(iDict, dicCompare);
    }

    public Dictionary<string, object> ToDictionary()
    {
        return null;
    }

    public string toJSON()
    {
        return JSON.JsonEncode(this.iDict);
    }
}

