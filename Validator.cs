using Newtonsoft.Json.Linq;

public class Validator
{
    private static Dictionary<string, string> reserveData = [];
    public string FieldValidator(string fieldData, ref JToken token)
    {
        try
        {
            string reserve = string.Empty;
            if (fieldData.Contains('-'))
            {
                string[] temp = fieldData.Split('-');
                fieldData = temp[0];
                reserve = temp[1];
            }

            switch (fieldData)
            {
                // id -> uuid
                case "id": return ValidateUUID(ref token, reserve, fieldData);
                case "created_at": return ValidateDate(ref token, reserve, fieldData);
                case "updated_at": return ValidateDate(ref token, reserve, fieldData);
                case "refresh_token": return ValidateRefreshToken(ref token);
                case "access_token": return ValidateAccessToken(ref token);
                default: return null;
            }
        }
        catch (Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(@"C:\Users\gbegl\source\repos\cs\TestProject1\TestProject1\errrror.txt", true))
            {
                sw.WriteLine(ex.Message);
                sw.WriteLine($"Data {token}");
                sw.WriteLine("--------------------------");
            }
            return null;
        }
    }
    private string ValidateDate(ref JToken token, string reserve, string fieldName)
    {
        if (token is JObject obj)
        {
            string key = reserve != String.Empty && reserveData.ContainsKey(reserve[2..]) ? reserve[2..] : fieldName;
            string date = obj[key].ToString();
            // check
            obj.Remove(key);
            obj[$"*{key}"] = fieldName;

            if (reserve != String.Empty)
            {
                obj[$"*{key}"] += $"-{ManageData(ref obj, reserve, date)}";
            }

            return date;
        }
        else if (token is JArray array)
        {
            foreach (var item in array)
            {
                ValidateUUID(ref token, reserve, fieldName);
            }
        }
        return null;
    }
    private string ValidateUUID(ref JToken token, string reserve, string fieldName)
    {
        if (token is JObject obj)
        {
            string key = reserve != String.Empty && reserveData.ContainsKey(reserve[2..]) ? reserve[2..] : fieldName;
            string date = obj[key].ToString();
            // check
            obj.Remove(key);
            obj[$"*{key}"] = fieldName;

            if (reserve != String.Empty)
            {
                obj[$"*{key}"] += $"-{ManageData(ref obj, reserve, date)}";
            }

            return date;
        }
        else if (token is JArray array)
        {
            foreach (var item in array)
            {
                ValidateUUID(ref token, reserve, fieldName);
            }
        }
        return null;
    }
    private string ValidateRefreshToken(ref JToken token)
    {
        if (token is JObject obj)
        {
            string refreshToken = (string)obj["refresh_token"];
            // check
            obj.Remove("refresh_token");
            obj["*refresh_token"] = "refresh_token";
            if (refreshToken != null)
                return $"Bearer {refreshToken}";
        }
        return null;
    }
    private string ValidateAccessToken(ref JToken token)
    {
        if (token is JObject obj)
        {
            string accessToken = (string)obj["access_token"];
            // check
            obj.Remove("access_token");
            obj["*access_token"] = "access_token";
            if (accessToken != null)
                return $"Bearer {accessToken}";
        }
        return null;
    }
    private string ManageData(ref JObject token, string key, string value)
    {
        if (key.StartsWith(">"))
        {
            reserveData[key[2..]] = value;
            return key;
        }
        if (reserveData[key[2..]] == value)
        {
            return key;
        }
        return String.Empty;
    }
}


