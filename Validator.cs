using Newtonsoft.Json.Linq;
using System.Net.Mail;

public class Validator
{
    private Dictionary<string, string> reserveData;
    private delegate bool BaseValidator(string value);
    private Dictionary<string, BaseValidator> validators;

    public Validator()
    {
        reserveData = new Dictionary<string, string>();
        validators = new Dictionary<string, BaseValidator>()
        {
            {"uuid", ValidateUUID },
            {"int", ValidateInt },
            {"email", ValidateEmail },
            {"date", ValidateDateTime },
        };
    }
    public string FieldValidator(string fieldKey, string fieldValue, ref JToken token)
    {
        string reserve = string.Empty;
        if (fieldValue.Contains('-'))
        {
            string[] temp = fieldValue.Split('-');
            fieldValue = temp[0];
            reserve = temp[1];
        }

        return Validate(ref token, reserve, fieldKey, fieldValue);
    }
    private string Validate(ref JToken token, string reserve, string fieldKey, string fieldValue)
    {
        if (token is JObject obj)
        {
            string cleanFieldKey = fieldKey[1..];
            if (!obj.ContainsKey(cleanFieldKey))
            {
                return null;
            }
            string value = obj[cleanFieldKey].ToString();
            if (validators.ContainsKey(fieldValue) && !validators[fieldValue](value))
            {
                throw new Exception($"Error: {value} is incorrect!");
            }
            obj.Remove(cleanFieldKey);
            obj[$"{fieldKey}"] = fieldValue;

            if (reserve != String.Empty)
            {
                obj[fieldKey] += $"-{ManageData(ref obj, reserve, value)}";
            }

            return value;
        }
        else if (token is JArray array)
        {
            ValidateJArray(ref array, reserve, fieldKey, fieldValue);
        }
        return null;
    }

    private void ValidateJArray(ref JArray array, string reserve, string fieldKey, string fieldValue)
    {
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i] is JObject obj)
            {
                string cleanFieldKey = fieldKey[1..];
                if (!obj.ContainsKey(cleanFieldKey))
                {
                    return;
                }
                string value = obj[cleanFieldKey].ToString();
                if (validators.ContainsKey(fieldValue) && !validators[fieldValue](value))
                {
                    throw new Exception($"Error: {value} is incorrect!");
                }
                obj.Remove(cleanFieldKey);
                obj[$"{fieldKey}"] = fieldValue;

                if (reserve != String.Empty)
                {
                    obj[fieldKey] += $"-{ManageData(ref obj, reserve, value)}";
                }
            }
        }
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
    private bool ValidateUUID(string uuid)
    {
        return Guid.TryParse(uuid, out _);
    }
    private bool ValidateInt(string intgr)
    {
        return int.TryParse(intgr, out _);
    }
    private bool ValidateDateTime(string date)
    {
        return DateTime.TryParse(date, out _);
    }
    private bool ValidateEmail(string email)
    {
        try
        {
            var email_ = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}


