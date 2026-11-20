using System.Text;

namespace tasks;

/// <summary>
/// Here you should implement PascalToSnakeCase and SnakeToPascalCase string extension methods.
/// </summary>
public static class StringExtensions
{
   public static string PascalToSnakeCase(this string? s)
    {
        if(s == null)return "";
        StringBuilder a = new StringBuilder();
        string temp = "";
        for(int i  = 0 ; i < s.Length; i++)
        {   
            
            if('A' <= s[i] && 'Z'>= s[i])
            {

                a.Append(temp.ToLowerInvariant());
                if(i>0)a.Append('_');
                temp = char.ToLowerInvariant(s[i]).ToString();
            }
            else
            {
                temp += char.ToLowerInvariant(s[i]);
            }
        }
        
        a.Append(temp);
        Console.WriteLine("adfsdafasdfasd");
        return a.ToString();
    }
    
    
    public static string SnakeToPascalCase(this string? s)
    {
        if(s == null)return "";
        StringBuilder a = new StringBuilder();
        string temp = "";
        for(int i  = 0 ; i < s.Length; i++)
        {   
            if(s[i] == '_')
            {   
                a.Append(temp);
                temp = "";
            }
            else
            {   
                if(temp == "") temp += char.ToUpperInvariant(s[i]);
                else{
                     temp += s[i];
                }
            }
        }
        a.Append(temp);
        return a.ToString();
    }
       
}
