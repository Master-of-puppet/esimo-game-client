using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using System.Collections;

/// <summary>
/// NGUYỄN VIỆT DŨNG
/// Class tiện ích để xử lý kiểm tra
/// </summary>
public static class Utility
{
    /// <summary>
    /// Check Input
    /// </summary>
    public static class Input
    {
        public const string DATE_TIME_FORMAT = "dd/MM/yyyy";
        public const string EMAIL_PATTERN =
                    @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
        public const string PHONE_NUMBER_PATTERN = @"^(0\d{9,10})$";
        public const string CMTND_NUMBER_PATTERN = @"^([0-9]{9})$";

        public static bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return Regex.IsMatch(email, EMAIL_PATTERN);
        }

        public static bool IsPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return false;

            return Regex.IsMatch(phone, PHONE_NUMBER_PATTERN);
        }

        public static bool IsBrithday(string brithday)
        {
            if (string.IsNullOrEmpty(brithday))
                return false;

            DateTime date;
            return DateTime.TryParseExact(brithday, DATE_TIME_FORMAT,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out date);
        }

        public static bool IsCMTND(string cmtnd)
        {
            if (string.IsNullOrEmpty(cmtnd))
                return false;

            return Regex.IsMatch(cmtnd, CMTND_NUMBER_PATTERN);
        }

        public static bool IsStringValid(string str, int min, int max)
        {
            return str.Length >= min && str.Length <= max;
        }
    }

    /// <summary>
    /// Convert data
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// Viết hoa chữ cái đầu
        /// </summary>
        public static string ToTitleCase(string str)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Định dạng kiểu ngày tháng trong game
        /// </summary>
        public static string TimeToString(DateTime d)
        {
            if (d.Ticks == 0)
                return "";

            return string.Format("{0:dd/MM/yyyy}", d);
        }

        /// <summary>
        /// Định dạng kiểu ngày tháng trong game
        /// </summary>
        public static string TimeToStringFull(DateTime d)
        {
            if (d.Ticks == 0)
                return "";

            return string.Format("{0:dd/MM/yyyy - hh:mm:ss}", d);
        }

        /// <summary>
        /// Định dạng kiểu tiền trong game (Eg: 10.000.000 Chip)
        /// </summary>
        public static string Chip(System.Object chip)
        {
            long money = 0;
            if (long.TryParse(chip.ToString(), out money) == false || money == 0)
                return "0";

            return string.Format("{0:#,##}", money).Replace(",", ".");
        }
        /// <summary>
        /// Định dạng kiểu tiền trong game (Eg: 10.000.000 Chip)
        /// </summary>
        public static string ChipToK(System.Object chip)
        {
            long money = 0;
            if (long.TryParse(chip.ToString(), out money) == false || money == 0)
                return "0";
            //if (money > 1000 && money < 1000000)
            //  return string.Format("{0:#,##}K", Mathf.Round(money / 1000));
            //else if (money >= 1000000)
            //  return string.Format("{0:#,##}M", Mathf.Round(money / 1000000));
            if (money > 1000)
                return string.Format("{0:#,##}K", Mathf.Round(money / 1000));

            return string.Format("{0:#,##}", money);
        }

        /// <param name="country_code">
        /// Việt Nam = "vi-VN"
        /// US = "en-US"
        /// </param>
        public static DateTime StringToTime(string date, string country_code)
        {
            IFormatProvider culture = new System.Globalization.CultureInfo(country_code, true);
            return DateTime.Parse(date, culture, System.Globalization.DateTimeStyles.AssumeLocal);
        }

        /// <summary>
        /// Tính tuổi
        /// </summary>
        public static int TimeToAge(DateTime time)
        {
            if (time.Ticks == 0)
                return 0;

            return DateTime.Now.Year - time.Year;
        }

        /// <summary>
        /// Định dạng time tin nhắn
        /// "Vừa xong", "Cách đây 2 phút", "Hôm nay", "Hôm qua"...
        /// </summary>
        public static string MessageTime(DateTime date)
        {
            DateTime now = DateTime.Now;
            TimeSpan time = now.Subtract(date);

            if (time.TotalMinutes < 1)
                return "Vừa xong";
            else if (time.TotalMinutes < 60)
                return string.Format("{0:##} phút trước", time.TotalMinutes);
            if (time.TotalDays <= 2 && now.DayOfYear == date.DayOfYear)
                return "Hôm nay";
            if (time.TotalDays <= 3 && now.DayOfYear - date.DayOfYear == 1)
                return "Hôm qua";
            return TimeToStringFull(date);
        }
    }

    /// <summary>
    /// Method for Enum
    /// </summary>
    public static class EnumUtility
    {
        /// <summary>
        /// Lấy thuộc tính mô tả của Enum
        /// </summary>
        public static string GetDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            System.ComponentModel.DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// Lấy các thông tin từ Attribute của Enum
        /// </summary>
        public static T GetAttribute<T>(Enum enumValue) where T : Attribute
        {
            T attribute;

            MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                attribute = (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
                return attribute;
            }
            return null;
        }

        /// <summary>
        /// Lấy enum từ mô tả của enum
        /// </summary>
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(System.ComponentModel.DescriptionAttribute)) as System.ComponentModel.DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
        }

        /// <summary>
        /// Lấy ra danh sách các enum
        /// </summary>
        public static string[] GetEnumName(Type typeEnum)
        {
            return Enum.GetNames(typeEnum);
        }
    }

    /// <summary>
    /// Auto Attach box collider for NGUI Object
    /// Use for Object OnEnable
    /// </summary>
    public static BoxCollider AddCollider(GameObject go)
    {
        if (go != null)
        {
            Collider col = go.GetComponent<Collider>();
            BoxCollider box = col as BoxCollider;

            if (box == null)
            {
                if (col != null)
                {
                    if (Application.isPlaying)
                        GameObject.Destroy(col);
                    else
                        GameObject.DestroyImmediate(col);
                }
                box = go.AddComponent<BoxCollider>();
            }

            int depth = NGUITools.CalculateNextDepth(go);

            Bounds b = NGUIMath.CalculateRelativeWidgetBounds(go.transform);
            box.isTrigger = true;
            box.center = b.center + Vector3.back * (depth * 0.25f);
            box.size = new Vector3(b.size.x, b.size.y, 0f);
            return box;
        }
        return null;
    }
    /// <summary>
    /// Get Width Height va ti? le man hinh de tinh toan
    /// Khi add mot component khong thuoc NGUI , nhung muon tinh toan no voi NGUI thi can get
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns>Vector3 , x = width , y = height , z = ratio (ti le)</returns>
    public static Vector3 GetWidthHeightScreenFollowNGUI(GameObject gameObject) 
    {
        int uiFactor = UniWebViewHelper.RunningOnRetinaIOS() ? 2 : 1;
        UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
        float ratio = ((float)mRoot.activeHeight / Screen.height) * uiFactor;
        int width = Mathf.FloorToInt(Screen.width * ratio / uiFactor);
        int height = Mathf.FloorToInt(Screen.height * ratio / uiFactor);
        return new Vector3(width, height,ratio);
    }

    /// <summary>
    /// Build EsObject from List
    /// </summary>
    static public Electrotank.Electroserver5.Api.EsObject SetEsObject(string command, params object[] param)
    {
        Electrotank.Electroserver5.Api.EsObject eso = new Electrotank.Electroserver5.Api.EsObject();

        if (!string.IsNullOrEmpty(command))
            eso.setString(Fields.COMMAND, command);

        if (param == null) return eso;

        for (int i = 0; i < param.Length; i += 2)
        {
            string key = (string)param[i];
            object obj = param[i + 1];

            if (obj.GetType() == typeof(Electrotank.Electroserver5.Api.EsObject))
                eso.setEsObject(key, (Electrotank.Electroserver5.Api.EsObject)obj);
            else if (obj.GetType() == typeof(string))
                eso.setString(key, (string)obj);
            else if (obj.GetType() == typeof(int))
                eso.setInteger(key, (int)obj);
            else if (obj.GetType() == typeof(bool))
                eso.setBoolean(key, (bool)obj);
            else if (obj.GetType() == typeof(long))
                eso.setLong(key, (long)obj);
            else if (obj is Electrotank.Electroserver5.Api.EsObject[])
                eso.setEsObjectArray(key, (Electrotank.Electroserver5.Api.EsObject[])obj);
            else if (obj is int[])
                eso.setIntegerArray(key, (int[])obj);
            else if (obj is string[])
                eso.setStringArray(key, (string[])obj);
            else
                Debug.LogError("-----> " + key + " Invalid");

            //else if (obj.GetType() == typeof(byte))
            //    eso.setByte(key, (byte)obj);
            //else if (obj.GetType() == typeof(char))
            //    eso.setChar(key, (char)obj);
            //else if (obj.GetType() == typeof(float))
            //    eso.setFloat(key, (float)obj);
            //else if (obj.GetType() == typeof(double))
            //    eso.setDouble(key, (double)obj);
            //else if (obj.GetType() == typeof(short))
            //    eso.setShort(key, (short)obj);
        }
        return eso;
    }

    public static void TranslateLocalY(GameObject go, float _value)
    {

        Vector3 oldVector = go.transform.localPosition;
        oldVector.y += _value;
        go.transform.localPosition = oldVector;
    }
    public static void AutoScrollChat(CUITextList textList)
    {
        if (textList.TotalLines * 16f < textList.paragraphHistory)
            TranslateLocalY(textList.gameObject, 16f);
    }
    public static string EncodeEmail(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            string[] arrEmail = email.Split('@');
            string str = arrEmail[0][0] + "" + arrEmail[0][1] + "" + arrEmail[0][2];
            string newEmail = str + "*******@" + arrEmail[1];
            return newEmail;
        }
        else
        {
            return "";
        }
    }
    public static string EndcodeNumber(string number)
    {
        if (!string.IsNullOrEmpty(number))
        {
            if (number.Length > 3)
            {
                string newNumber = "";
                for (int i = 0; i < number.Length; i++)
                {
                    if (i < 3)
                        newNumber += number[i];
                    else
                        newNumber += "*";
                }
                return newNumber;
            }
            else
            {
                return number;
            }
        }
        else
        {
            return "";
        }
    }
    public static string EndCodePhoneNumber(string number)
    {
        if (!string.IsNullOrEmpty(number))
        {
            string newNumber = "**********";
            for (int i = 3; i > 0; i--)
            {
                newNumber += number[number.Length - i];
            }
            return newNumber;
        }
        else
        {
            return "";
        }
    }

    public static class ComparableCriteria
    {
        public static Dictionary<string, object> mergeDictionary(Dictionary<string, object> criteria1, Dictionary<string, object> criteria2)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string key in criteria1.Keys)
            {
                result.Add(key, criteria1[key]);
            }
            foreach (string key in criteria2.Keys)
            {
                result.Add(key, criteria2[key]);
            }
            return result;
        }

        public static List<Criteria> buildCriteria(Dictionary<string, object> sample)
        {
            List<Criteria> result = new List<Criteria>();
            foreach (string key in sample.Keys)
            {
                if (sample[key] is Dictionary<string, object>)
                {
                    // de quy, ke cmn
                }
                else if (sample[key].GetType().IsGenericType && sample[key] is IEnumerable)
                {
                    
                    Type typeObject = sample[key].GetType().GetGenericArguments().Single();
                    if (typeObject == typeof(int))
                    {
                        List<int> subList = (List<int>)sample[key];
                        if (result.Count == 0)
                        {
                            for (int i = 0; i < subList.Count; i++)
                            {
                                Dictionary<string, object> dict = new Dictionary<string, object>();
                                dict.Add(key, subList[i]);
                                result.Add(new Criteria(dict));
                            }
                        }
                        else
                        {
                            List<Criteria> tmp = new List<Criteria>();
                            for (int i = 0; i < subList.Count; i++)
                            {
                                Dictionary<string, object> dict1 = new Dictionary<string, object>();
                                dict1.Add(key, subList[i]);

                                for (int j = 0; j < result.Count; j++)
                                {
                                    tmp.Add(new Criteria(mergeDictionary((Dictionary<string, object>)result[j].iDict, dict1)));
                                }
                            }
                            result = tmp;
                        }
                    }
                    else if (typeObject == typeof(string))
                    {
                        List<string> subList = (List<string>)sample[key];
                        if (result.Count == 0)
                        {
                            for (int i = 0; i < subList.Count; i++)
                            {
                                Dictionary<string, object> dict = new Dictionary<string, object>();
                                dict.Add(key, subList[i]);
                                result.Add(new Criteria(dict));
                            }
                        }
                        else
                        {
                            List<Criteria> tmp = new List<Criteria>();
                            for (int i = 0; i < subList.Count; i++)
                            {
                                Dictionary<string, object> dict1 = new Dictionary<string, object>();
                                dict1.Add(key, subList[i]);

                                for (int j = 0; j < result.Count; j++)
                                {
                                    tmp.Add(new Criteria(mergeDictionary((Dictionary<string, object>)result[j].iDict, dict1)));
                                }
                            }
                            result = tmp;
                        }
                    }
                    else if (typeObject == typeof(long))
                    {
                        List<long> subList = (List<long>)sample[key];
                        if (result.Count == 0)
                        {
                            for (int i = 0; i < subList.Count; i++)
                            {
                                Dictionary<string, object> dict = new Dictionary<string, object>();
                                dict.Add(key, subList[i]);
                                result.Add(new Criteria(dict));
                            }
                        }
                        else
                        {
                            List<Criteria> tmp = new List<Criteria>();
                            for (int i = 0; i < subList.Count; i++)
                            {
                                Dictionary<string, object> dict1 = new Dictionary<string, object>();
                                dict1.Add(key, subList[i]);

                                for (int j = 0; j < result.Count; j++)
                                {
                                    tmp.Add(new Criteria(mergeDictionary((Dictionary<string, object>)result[j].iDict, dict1)));
                                }
                            }
                            result = tmp;
                        }
                    }
                }
                else
                {
                    if (result.Count == 0)
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>();
                        dict.Add(key, sample[key]);
                        result.Add(new Criteria(dict));
                    }
                    else
                    {
                        for (int j = 0; j < result.Count; j++)
                        {
                            result[j].iDict.Add(key, sample[key]);
                        }
                    }
                }
            }
            return result;
        }

        public static List<Dictionary<string, object>> filterByCriterias(List<Dictionary<string, object>> sample, List<Dictionary<string, object>> criterias)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            for (int i = 0; i < sample.Count; i++)
            {
                for (int j = 0; j < criterias.Count; j++)
                {
                    if (IsMatchCriteria(sample[i], criterias[j]))
                    {
                        result.Add(sample[i]);
                    }
                }
            }
            return result;
        }

        public static bool IsMatchCriteria(Dictionary<string, object> obj, Dictionary<string, object> criteria)
        {
            foreach (string key in criteria.Keys)
            {
                if (criteria[key] is Dictionary<string, object>)
                {
                    if (obj[key] is Dictionary<string, object>)
                    {
                        bool isMatch = IsMatchCriteria((Dictionary<string, object>)obj[key], (Dictionary<string, object>)criteria[key]);
						if (!isMatch) return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (obj.ContainsKey(key))
                {

					if(obj[key].GetType() == typeof(int)){
						bool isMatch = (int)criteria[key] == (int)obj[key];
						if (!isMatch) return false;
					}else if(obj[key].GetType() == typeof(string)){
						bool isMatch = ((string)obj[key]).Contains((string)criteria[key])  ;
						if (!isMatch) return false;
					}else if(obj[key].GetType() == typeof(long)){
						bool isMatch = (long)criteria[key] == (long)obj[key];
						if (!isMatch) return false;
                    }
                    else if (obj[key].GetType() == typeof(bool)) {
						bool isMatch = (bool)criteria[key] == (!(bool)obj[key]);
						if (!isMatch) return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
