using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface ISimilarComparable
{
    Dictionary<string, object> ToDictionary();
    bool Compare(Dictionary<string, object> dicCompare);
}

