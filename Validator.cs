using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;

public class Validator
{
    private Dictionary<string, string> reserveData;
    private delegate bool BaseValidator(string value);
    private Dictionary<string, BaseValidator> validators;

    public Validator()
    {
        reserveData = new Dictionary<string, string>(); 
        validators = new Dictionary<string, BaseValidator>();
        validators.Add("uuid", ValidateUUID);
        validators.Add("int", ValidateInt);
        validators.Add("email", ValidateEmail);
        validators.Add("date", ValidateDateTime);
    }
    public string FieldValidator(string fieldKey, string fieldValue, ref JToken token)
    {
        try
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
    private string Validate(ref JToken token, string reserve, string fieldKey, string fieldValue)
    {
        if (token is JObject obj)
        {
            string value = obj[fieldKey[1..]].ToString();
            if (validators.ContainsKey(fieldValue) && !validators[fieldValue](value))
            {
                throw new Exception($"Error: {value} is incorrect!");
            }
            obj.Remove(fieldKey[1..]);
            obj[$"{fieldKey}"] = fieldValue;

            if (reserve != String.Empty)
            {
                obj[$"{fieldKey}"] += $"-{ManageData(ref obj, reserve, value)}";
            }

            return value;
        }
        else if (token is JArray array)
        {
            foreach (var item in array)
            {
                Validate(ref token, reserve, fieldKey, fieldValue);
            }
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


