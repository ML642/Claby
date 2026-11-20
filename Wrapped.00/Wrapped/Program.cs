using System.Globalization;
using Wrapped;

// Format numbers and dates the same way on every machine
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

var library = MusicLibrary.Load(Path.Combine(AppContext.BaseDirectory, "Data"));

Console.WriteLine($"Loaded {library.Artists.Count} artists, {library.Tracks.Count} tracks and {library.Plays.Count} plays.");
Console.WriteLine($"First play: {library.Plays[0]}");

static int Count(IEnumerable<Play> plays, Func<Play, bool> predicate)
{
    int count = 0;

    foreach (Play play in plays)
    {
        if (predicate(play))
            count++;
    }

    return count;
}

Action<string, int> print = (label, value)=>
{
    Console.WriteLine($"{label,-30}{value,6}");
};

var plays = library.Plays;

print("All plays", Count(plays, play => true));
print("Skips", Count(plays, Filters.IsSkip));
print("Weekend plays", Count(plays, Filters.IsWeekend));
print("Night plays", Count(plays, Filters.AtNight));

print("Weekend nights",
    Count(plays, Filters.All(Filters.IsWeekend, Filters.AtNight)));

print("Night streams",
    Count(plays, Filters.All(
        Filters.AtNight,
        Filters.Not(Filters.IsSkip))));

print("Longer than 6 minutes",
    Count(plays, Filters.LongerThan(TimeSpan.FromMinutes(6))));

print("Ada & The Lambdas",
    Count(plays, Filters.ByArtist(library, "Ada & The Lambdas")));

print("Plays at 7 a.m.",
    Count(plays, play => play.Timestamp.Hour == 7));