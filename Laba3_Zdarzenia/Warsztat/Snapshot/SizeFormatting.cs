
using System.Globalization;
public static class Wrt
{
    public static string[] Units = ["B","KB","MB","GB","TB"]; 
    public static string ToHumanSize(this long bytes)
    {
        double size = bytes;
        int unit = 0;
        
        
        while(bytes >= 1024)
        {
            bytes /= 1024;
            unit++;    
        }
        

        return unit == 0 ? 
        string.Create(CultureInfo.InvariantCulture, $"{bytes} B")
        :
        string.Create(CultureInfo.InvariantCulture,
            $"{size:0.0} {Units[unit]}");
    }
}