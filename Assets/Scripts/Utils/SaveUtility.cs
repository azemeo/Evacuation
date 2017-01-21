using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;


/// <summary>
/// A collection of utility Methods useful for saving and loading data.
/// </summary>
public class SaveUtility
{

    #region Data Type Conversion
    public static List<string> csvToStringList(string input, string delimiter)
    {
        if (String.IsNullOrEmpty(input)) return new List<string>();

        List<string> retList = new List<string>(input.Split(delimiter[0]));

        return retList;
    }

    public static List<string> csvToStringList(string input)
    {
        List<string> retList = new List<string>(input.Split(','));

        return retList;
    }

    public static string ListToCsv<T> (IList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return "";
        }

        string retString = list[0].ToString();
        for (int i = 1; i < list.Count; i++)
        {
            retString += "," + list[i].ToString();
        }

        return retString;
    }

    public static Vector2Int csvToVector2Int(string input)
    {
        input = input.Replace("(", "").Replace(")", "");
        if (input == "") return new Vector2Int();

        List<string> valueList = csvToStringList(input);
        Vector2Int retVal = new Vector2Int(int.Parse(valueList[0]), int.Parse(valueList[1]));

        return retVal;
    }

    public static Vector2 csvToVector2(string input)
    {
        input = input.Replace("(", "").Replace(")", "");
        if (input == "") return new Vector2();

        List<string> valueList = csvToStringList(input);
        Vector2 retVal = new Vector2(float.Parse(valueList[0]), float.Parse(valueList[1]));

        return retVal;
    }

    public static Vector3Int csvToVector3Int(string input)
    {
        input = input.Replace("(", "").Replace(")", "");
        if (input == "") return new Vector3Int();

        List<string> valueList = csvToStringList(input);
        Vector3Int retVal = new Vector3Int(int.Parse(valueList[0]), int.Parse(valueList[1]), int.Parse(valueList[2]));

        return retVal;
    }

    public static Vector3 csvToVector3(string input, string delimiter = ",")
    {
        input = input.Replace("(", "").Replace(")", "");
        if (input == "") return new Vector3();

        List<string> valueList = csvToStringList(input, delimiter);
        Vector3 retVal;

        if (valueList.Count == 2)
            retVal = new Vector3(float.Parse(valueList[0]), float.Parse(valueList[1]));
        else if (valueList.Count == 3)
            retVal = new Vector3(float.Parse(valueList[0]), float.Parse(valueList[1]), float.Parse(valueList[2]));
        else
            retVal = new Vector3();

        return retVal;
    }

    public static Vector3 csvToVector3(string input, float zDepth)
    {
        input = input.Replace("(", "").Replace(")", "");
        if (input == "") return new Vector3();

        List<string> valueList = csvToStringList(input);
        Vector3 retVal;

        if (valueList.Count == 2)
            retVal = new Vector3(float.Parse(valueList[0]), float.Parse(valueList[1]), zDepth);
        else
            retVal = new Vector3(0, 0, zDepth);

        return retVal;
    }

    public static string Vector3ToCsv(Vector3 input, string delimiter = ",")
    {
        string retVal = input.ToString();
        if(delimiter != ",")
        {
            retVal.Replace(',', delimiter[0]);
        }
        return retVal;
    }

    public static Rect csvToRect(string input, Rect defaultValue)
    {
        if (input == "") return defaultValue;

        List<string> valueList = csvToStringList(input);
        Rect retVal = new Rect(float.Parse(valueList[0]), float.Parse(valueList[1]), float.Parse(valueList[2]), float.Parse(valueList[3]));

        return retVal;
    }

    public static Color csvRGBAToColor(string input)
    {
        if (input == "") return new Color(1, 1, 1, 1);

        List<string> valueList = csvToStringList(input);
        float r = stringToFloat(valueList[0]);
        float g = stringToFloat(valueList[1]);
        float b = stringToFloat(valueList[2]);
        float a = stringToFloat(valueList[3]);

        if (r > 1) { r = r / 255; }
        if (g > 1) { g = g / 255; }
        if (b > 1) { b = b / 255; }
        if (a > 1) { a = a / 255; }

        return new Color(r, g, b, a);
    }

    public static Color csvRGBAToColor(string input, Color defaultValue)
    {
        if (input == "") return defaultValue;

        return csvRGBAToColor(input);
    }

    public static bool stringToBool(string input)
    {
        return stringToBool(input, false);
    }

    public static bool stringToBool(string input, bool defaultVal)
    {
        if (input.ToLower() == "true" || input == "1" || input.ToLower() == "yes")
        {
            return true;
        }
        else if (input.ToLower() == "false" || input == "0" || input.ToLower() == "no")
        {
            return false;
        }
        else
        {
            return defaultVal;
        }
    }

    public static float stringToFloat(string input, int digits = -1)
    {
        return stringToFloat(input, 0, digits);
    }

    public static float stringToFloat(string input, float defaultVal, int digits = -1)
    {
        input = input.Replace(",", "").Replace(" ", "");
        try
        {
            float retVal = float.Parse(input);
            if (digits >= 0)
            {
                retVal = (float)Math.Round(retVal, digits);
            }
            return retVal;
        }
        catch
        {
            return defaultVal;
        }
    }

    public static int stringToInt(string input)
    {
		return stringToInt(input, 0);
    }

	public static int stringToInt(string input, int defaultVal)
	{
        input = input.Replace(",", "").Replace(" ", "");
		try
		{
			return int.Parse(input);
		}
		catch
		{
			return defaultVal;
		}
	}

    public static DateTime stringToDateTime(string input)
    {
        return DateTime.Parse(input);
    }

    public static bool TryParseInt64(string input, out long output)
    {
        try
        {
            output = Int64.Parse(input);
            return true;
        }
        catch
        {
            output = 0;
            return false;
        }
    }

    public static long ParseInt64(string input)
    {
        try
        {
            return Int64.Parse(input);
        }
        catch
        {
            return 0;
        }
    }

    public static bool TryParseDouble(string input, out double output)
    {
        try
        {
            output = Double.Parse(input);
            return true;
        }
        catch
        {
            output = 0.0;
            return false;
        }
    }

    public static T stringToEnum<T>(string value, T defaultEnum)
    {
		try
		{
	        value = value.Replace(" ", "").Trim();  //.Replace("_", "")
	        object retval = Enum.Parse(typeof(T), value, true);

	        if (retval != null)
	            return (T)retval;
	        else
	        return defaultEnum;
		}
		catch
		{
			return defaultEnum;
		}
    }

    public static byte[] intToByteArray(int input)
    {
        return BitConverter.GetBytes(input);
    }

    public static int byteArrayToInt(byte[] input)
    {
        return BitConverter.ToInt32(input, 0);
    }

    public static byte[] stringToByteArray(string input)
    {
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        return encoding.GetBytes(input);
    }

    public static string byteArrayToString(byte[] input)
    {
        return System.Text.ASCIIEncoding.ASCII.GetString(input);
    }

    #endregion

    #region Saving and Loading

	public static bool SaveFile(string fileText, string path)
	{
		try
		{
			FileStream file = File.Open(path, FileMode.Create);
			StreamWriter writer = new StreamWriter(file);
			writer.WriteLine(fileText);
			writer.Dispose();
			file.Dispose();

			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool SaveFile(byte[] bytes, string path)
	{
		try
		{
			File.WriteAllBytes(path, bytes);
			return true;
		}
		catch
		{
			return false;
		}
	}

#if UNITY_EDITOR
    public static string SaveFileAs(string fileText, string windowTitle = "Save File", string directory = "", string fileName = "", string extension = "txt")
    {
        if (string.IsNullOrEmpty(directory))
        {
            directory = Application.dataPath;
        }
        string path = UnityEditor.EditorUtility.SaveFilePanel(windowTitle, directory, fileName, extension);
        if (SaveFile(fileText, path))
        {
            return path;
        }
        return "";
    }
#endif

    public static string LoadFile(string path)
	{
        if (FileExists(path))
        {
            FileStream file = File.Open(path, FileMode.Open);
            StreamReader reader = new StreamReader(file);
            string fileText = reader.ReadToEnd();
            reader.Dispose();
            file.Dispose();

            return fileText;
        }
        return null;
	}

    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }

    #endregion

    public static string getParentFolder(string folder)
    {
        return folder.Substring(0, folder.LastIndexOf("/"));
    }

    static public string CreateMD5Hash(string input)
    {
        // Use input string to calculate MD5 hash
        MD5 md5 = MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        // Convert the byte array to hexadecimal string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("X2"));
            // To force the hex string to lower-case letters instead of
            // upper-case, use he following line instead:
            // sb.Append(hashBytes[i].ToString("x2"));
        }
        return sb.ToString();
    }

}
