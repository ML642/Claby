using System.Globalization;

namespace Task2;
public readonly record struct Reading(string Sensor, double Value, DateTime At);
public record class Data(
    double HighestValue,                        // highest recorded value across all sensors
    double LowestTemp,                          // lowest recorded temperature
    string MostReadings,                        // name of the sensor with most readings
    TimeSpan TotalDuration,                     // difference between first and last recorded reading
    IList<string> UniqueSensors,                // list of all sensor names without duplicates
    IList<(string, double)> AverageReadings,    // average reading per sensor
    IList<(string, TimeSpan)> LargestGaps,      // a largest timespan between two consecutive readings (time order) per sensor
    IList<(Reading, Reading)> SignChanges       // two consecutive readings (time order) having two differnet signs (first < second) 
);

public class SensorLog {
    private readonly List<Reading> _readings = new();
    public void Add(Reading r) { _readings.Add(r); }
    public IReadOnlyList<Reading> All => _readings;

    public SensorLog LoadFromCsv(string path) {
        bool first = true;
        foreach (var line in File.ReadLines(path)) {
            if (first) { first = false; continue; } // assuming header            
            if (string.IsNullOrWhiteSpace(line)) {
                continue;
            }
            var parts = line.Split(',');
            Add(new Reading(parts[0].Trim(), double.Parse(parts[1], CultureInfo.InvariantCulture), DateTime.Parse(parts[2], CultureInfo.InvariantCulture)));
        }
        return this;
    }
}

// =============================================================
// TODO 1
// =============================================================

public static class T1 {
    public static IEnumerable<Reading> InRange(IEnumerable<Reading> source, double min, double max) {
        foreach(var i in source)
        {
            if(i.Value >= min && i.Value <= max)
            {
                yield return source;
            }
        }
    }
}

// =============================================================
// TODO 2 
// =============================================================

public static class T2 {
    public static IEnumerable<Reading> Repeat(string sensor, double value, int count, DateTime start) {
        for(int i = 0; i < count ; i++)
        {
            yield return new Reading(sensor,value,start);
            start = start.AddMinutes(2);
            value += 0.2;
        }
    }
}

// =============================================================
// TODO 3
// =============================================================

public class ReadingBySensorThenValue : Comparer<Reading> {
    public override int Compare(Reading x, Reading y) {
        int result = x.Sensor.CompareTo(y.Sensor);

        if(!result)return result;

        result = x.Value.CompareTo(y.Value);

        if(!result)return result;

        return  x.At.CompareTo(y.At);
    }

}


// =============================================================
// TODO 4 
// =============================================================

public static class T4 {

    private double HighestValue(IEnumerable<Reading> source)
    {
        int max = -99;
        foreach(var i in source)
        {
            max = Math.Max(i.Value,max);
        }
        return max;
    }

    private double LowestTemp(IEnumerable<Reading> source)
    {
        int min = 999;
        foreach(var i in source)
        {
            min = Math.Min(min,i.Value);
        }
        return min;
    }
    private async Task<string> MostReadings(IEnumerable<Reading> source)
    {
        string name;
        int max = -99;
        IDictionary<string,int> a = new IDictionary<string,int>();
        foreach (var i in source)
        {
            if (!a.Contains(i.Sensor))
            {
                a.Add(i.Sensor,1);
            }
            else
            {
                a[i.Sensor] += 1;
                if (a[i.Sensor] > max)
                {
                    name = i.Sensor;
                    max = a[i.Sensor];
                }
            }
        }
    }

    private TimeSpan TotalDuration(IEnumerable<Reading> source)
    {
        Reading last = source[source.Count()-1];

        return source[0].At - last.At;
    }
    private IList<string> UniqueSensors(IEnumerable<Reading> source)
    {
        HashSet<string> names;
        foreach(var i in source)
        {
            names.Add(i.Sensor);
        }

        IList<string> answer = new IList<string>();
        foreach(var i in names)
        {
            answer.Add(i);
        }
        return answer;
    }
    
    public static Data Summarize(IEnumerable<Reading> source) {
        
    }
}

// =============================================================
// TODO 5 
// =============================================================

public static class T5 {
    public static string Classify(IReadOnlyList<Reading> readings) {
        throw new NotImplementedException();
    }
}