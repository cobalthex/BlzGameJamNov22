using System.Text;

public static class StringExtensions
{
    private static StringBuilder s_stringBuilder = new StringBuilder();

    public static string FormatString(this string thisString, params object[] objects)
    {
        return string.Format(thisString, objects);
    }

    public static string ConcatString(this string thisString, params object[] objects)
    {
        s_stringBuilder.Length = 0;
        s_stringBuilder.Append(thisString);
        foreach (var obj in objects)
            s_stringBuilder.Append(obj);
        return s_stringBuilder.ToString();
    }

    public static string JoinString(this string thisString, params object[] objects)
    {
        s_stringBuilder.Length = 0;
        var count = objects.Length;
        for (int i = 0; i < count-1; i++)
        {
            s_stringBuilder.Append(objects[i].ToString());
            s_stringBuilder.Append(thisString);
        }
        s_stringBuilder.Append(objects[count-1].ToString());
        return s_stringBuilder.ToString();
    }
}