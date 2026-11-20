using Wrapped;
public static class Filters{
    public static readonly TimeSpan SkipThreshold = TimeSpan.FromSeconds(30);

    public static bool IsSkip(Play play)
    {   
        if( play.Duration > SkipThreshold)return false;
        return true;
    }
    public static bool IsWeekend(Play play)
    {
        if(play.Timestamp.DayOfWeek == DayOfWeek.Sunday ||
            play.Timestamp.DayOfWeek == DayOfWeek.Sunday)
        {
            return true;
        }
        return false;
    }
    public static readonly Func<Play, bool> AtNight =
    static play => play.Timestamp.Hour >= 22
                || play.Timestamp.Hour < 6;   

    
    public static Func<Play,bool> InMonth(int month)
    {
        return play=>play.Timestamp.Month == month;
    }

    public static Func<Play, bool> LongerThan(TimeSpan duration) => play => play.Duration > duration;

    public static Func<Play,bool> ByArtist(MusicLibrary library, string artistName)
    {
        var traksIds = new HashSet<int>();

        foreach(var artist in library.Artists)
        {
            if(artist.Name != artistName)
                continue;
             foreach (var track in library.Tracks)
        {
            if (track.ArtistId == artist.Id)
                traksIds.Add(track.Id);
        }    
        }
        return play => traksIds.Contains(play.TrackId) ;
    }
    

    public static Func<Play, bool> Not(Func<Play, bool> filter)
    {
        return play => !filter(play) ;
   }


    public static Func<Play, bool> All(params Func<Play, bool>[] filters)
    {
        return play => {
        bool result = true;
        foreach(var i in filters)
        {
            if(i(play) == false)
                {
                    result = false;
                }
        }
        return result;
        };
    }
    public static Func<Play, bool> Any(params Func<Play, bool>[] filters)
    {
        return play => {
           foreach(var i in filters)
        {
            if(i(play))
                {
                    return  true;
                }
        }
        return false;
        };
    }

};