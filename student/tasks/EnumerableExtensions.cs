using System.ComponentModel;
using System.Data;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace tasks;

/// <summary>
/// Here you should implement various extension methods for IEnumerable<T>.
/// </summary>
public static class EnumerableExtensions
{
    public static TResult Fold<TSource, TAccumulate, TResult>(
        this IEnumerable<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> accumulator,
        Func<TAccumulate, TResult> resultSelector)
    {
        TAccumulate state = seed;
        using IEnumerator<TSource> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            state = accumulator(state, enumerator.Current);
        }

        return resultSelector(state);
    }

    public static (double,double,double ,double ) ComputeStatistics(this int[] a)
    {
        double max = 0;
        double min = 0;
        double  average = 0;
        double standardDeviation = 0;


        max = a.Fold(
            -999,
            (CurrentMax,value)=>Math.Max(CurrentMax,value),
            (max) =>
            {
                return max;
            }
        );

        min = a.Fold(
            999,
            (CurrentMax,value)=>Math.Min(CurrentMax,value),
            (max) =>
            {
                return max;
            }
        );
        average = a.Fold(
            0,
            (CurrentMax,value)=>CurrentMax + value,
            (max) =>
            {
                return max / a.Count();
            }
        );
        standardDeviation = a.Fold(
            0.0,
            (CurrentMax,value)=>(CurrentMax + value - average) * (CurrentMax + value - average),
            (x) =>
            {
             return Math.Sqrt(x);   
            }
        );
        
        return (max,min,average,standardDeviation);
    }

        public static (int start, int end, int value) LongestSequence(this IEnumerable<int> sequence)
    {
        return sequence.Fold(
            seed: (
                Start: 0,
                End: 0,
                Value: sequence.First(),
                CurrentStart: 0,
                CurrentEnd: 0,
                CurrentValue: sequence.First()
            ),
            (acc, elem) =>
            {
                if (elem == acc.CurrentValue)
                {
                    var length = acc.End - acc.Start + 1;
                    var currentLength = acc.CurrentEnd - acc.CurrentStart + 1;

                    if (currentLength > length)
                    {
                        acc.Start = acc.CurrentStart;
                        acc.End = acc.CurrentEnd;
                        acc.Value = acc.CurrentValue;
                    }
                }
                else
                {
                    acc.CurrentStart = acc.CurrentEnd;
                    acc.CurrentValue = elem;
                }

                acc.CurrentEnd++;

                return acc;
            },
            resultSelector: acc => (
                start: acc.Start,
                end: acc.End,
                value: acc.Value
            )
        );
    }

    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> a,int size)
    {
        using var enumerator = a.GetEnumerator();
        
        while (enumerator.MoveNext())
        {
            var batch = new List<T>(capacity: size)
            {
                enumerator.Current
            };

            for (var i = 1; i < size && enumerator.MoveNext(); i++)
            {
                batch.Add(enumerator.Current);
            } 

            yield return batch; 
        }
    }
    public static IEnumerable<IEnumerable<T>> SlidingWindow<T>(this IEnumerable<T> a,int size)
    {
        if(size < 1)
        {
            throw new ArgumentException("Window must be at least 1");
        }

        using var iterator = a.GetEnumerator();
        var SlidingWindow = new List<T>();

        int current = 0;
        while (iterator.MoveNext())
        {
            
            if(current < size)
            {
                SlidingWindow.Add(iterator.Current);
                current++;
                continue;
            }
            yield return SlidingWindow;
            SlidingWindow.Add(iterator.Current);
            SlidingWindow.RemoveAt(0);
        }      
        if(current == size){
        yield return SlidingWindow;
        }
    }
    public static IEnumerable<IEnumerable<int>> FindSlidingWindowsWithRisingSum(this int[] a)
    {   
        int PrevSum = int.MaxValue;
        foreach(var i in a.SlidingWindow(5))
        {
            
            int sum = i.Sum();
            if(PrevSum < sum)
            {
                yield return i;
            }
            PrevSum = sum;
        }
    }
    public static IEnumerable<IEnumerable<int>> FindSlidingWindowsWithDuplicates(this int[] a)
    {   
        foreach(var i in a.SlidingWindow(5))
        {
            bool hasDuplicate = i.Distinct().Count() < i.Count(); 
            if(hasDuplicate)
            {
                yield return i;
            }  
        }
    }
    public static IEnumerable<string> FindMostCommonTrigrams(this string text)
    {
        if(text.Length < 3)
        {
            return new List<string>();
        } 

        var counts = text
        .Where(char.IsLetter)
        .Select(char.ToLowerInvariant)
        .SlidingWindow(3)
        .Select(window=>new string(window.ToArray()))
        .GroupBy( trigram => trigram)
        .Select(group => new
        {
            Trigram = group.Key,
            Count = group.Count()
        }).ToList();
        if (counts.Count == 0) return Enumerable.Empty<string>();
    
        int maxCount = counts.Max(item => item.Count);

        return counts.Where(item => item.Count == maxCount)
                     .Select(item =>item.Trigram);

    }
}



