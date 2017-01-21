using UnityEngine;
using System.Collections;

public static class Toolbox {
    public const string GameScene = "Game";

    public static string FormatTime(int seconds)
    {
        string timeString = "";

        if (seconds > 86400)
        {
            int d = Mathf.FloorToInt(seconds / 86400);
            seconds -= (d * 86400);
            timeString += d.ToString() + "d";

            if (seconds == 0)
                return timeString;
        }
        if (seconds > 3600)
        {
            int h = Mathf.FloorToInt(seconds / 3600);
            seconds -= (h * 3600);
            timeString += h.ToString() + "h";

            if (seconds == 0)
                return timeString;
        }

        if (seconds > 60)
        {
            int m = Mathf.FloorToInt(seconds / 60);
            seconds -= (m * 60);
            timeString += m.ToString() + "m";
            if (seconds == 0)
                return timeString;
        }

        timeString += seconds.ToString() + "s";

        return timeString;
    }

    /// <summary>
    /// The reverse of FormatTime(). Takes a formatted time string (#d#h#m#s) and returns the time in seconds it represents.
    /// </summary>
    /// <param name="timeString">A string representing a time in the format (#d#h#m#s)</param>
    /// <returns>Time in Seconds</returns>
    public static int GetTimeFromString(string timeString)
    {
        int timeInSeconds = 0;
        if (timeString.Contains("d"))
        {
            int dayIndex = timeString.IndexOf("d");
            string days = timeString.Substring(0, dayIndex);
            timeInSeconds += SaveUtility.stringToInt(days) * 86400;
            timeString = timeString.Remove(0, dayIndex + 1);
        }
        if (timeString.Contains("h"))
        {
            int hourIndex = timeString.IndexOf("h");
            string hours = timeString.Substring(0, hourIndex);
            timeInSeconds += SaveUtility.stringToInt(hours) * 3600;
            timeString = timeString.Remove(0, hourIndex + 1);
        }
        if (timeString.Contains("m"))
        {
            int minIndex = timeString.IndexOf("m");
            string mins = timeString.Substring(0, minIndex);
            timeInSeconds += SaveUtility.stringToInt(mins) * 60;
            timeString = timeString.Remove(0, minIndex + 1);
        }
        if (timeString.Contains("s"))
        {
            int secIndex = timeString.IndexOf("s");
            string secs = timeString.Substring(0, secIndex);
            timeInSeconds += SaveUtility.stringToInt(secs);
        }
        else if(!string.IsNullOrEmpty(timeString))
        {
            timeInSeconds += SaveUtility.stringToInt(timeString);
        }
        return timeInSeconds;
    }

    public static void SetActiveChildren(GameObject gameObject, bool value)
    {
        foreach(Transform t in gameObject.transform)
        {
            t.gameObject.SetActive(value);
        }
    }

    public static int GetCompleteCost(float timeRemaining)
    {
        int[] ranges = new int[] { 60, 3600, 86400, 604800 };
        int[] gems = new int[] { 1, 20, 260, 1000 };

        return convertResourceToClovers(Mathf.FloorToInt(timeRemaining), ranges, gems);
    }

    public static int GetResourceCost(int resourceAmount)
    {
        int[] ranges = new int[] { 100, 1000, 10000, 100000, 1000000, 10000000 };
        int[] gems = new int[] { 1, 5, 25, 125, 600, 3000 };

        return convertResourceToClovers(resourceAmount, ranges, gems);
    }

    private static int convertResourceToClovers(int amount, int[] ranges, int[] gems)
    {
        //**TODO**
        //The following code is taken from here: http://forum.supercell.net/showthread.php/23028-Gem-calculation-formulas
        // (translated from javascript to c# of course), to give a reasonable sense on how time relates to gem costs during the demo.
        // We should change this formula (or at least the numbers used) for the final game, as well as not hard-coding the values here. -EK

        if (ranges == null || gems == null) return 0;

        if (amount <= 0) return 0;
        else if (amount <= ranges[0]) return (gems[0]);

        //find the bracket that the time remaining currently lies in
        for (int i = 1; i < ranges.Length - 1; i++)
        {
            if (amount <= ranges[i])
            {
                //cost of the tier just below ours + cost of the remaining time in the current tier (scaled within that tier)
                return (Mathf.FloorToInt((amount - ranges[i - 1]) / ((ranges[i] - ranges[i - 1]) / (gems[i] - gems[i - 1])) + gems[i - 1]));
            }
        }

        //if we get here, we are buying something past the final tier, so calculate it based on the same scale used in the final tier.
        return (Mathf.FloorToInt((amount - ranges[ranges.Length - 2]) / ((ranges[ranges.Length - 1] - ranges[ranges.Length - 2]) / (gems[gems.Length - 1] - gems[gems.Length - 2])) + gems[gems.Length - 2]));
    }

    public static T GetOrCreateComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component as T;
    }

    public static bool IsLayerInMask(GameObject gameObject, LayerMask mask)
    {
        if (((1 << gameObject.layer) & mask) != 0)
        {
            return true;
        }
        return false;
    }
}
