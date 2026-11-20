using System.ComponentModel;
using System.Numerics;

namespace Lab10;

/// <summary>
/// W tym etapie nie wolno używać funkcji Aggregate z LINQ.<br/>
/// Napisz metodę rozszerzającą Reduce&lt;TSource, TAccumulate&gt;
/// do interfejsu IEnumerable&lt;TSource&gt;, która będzie
/// wykonywać operację redukcji na sekwencji do pojedynczej wartości.<br/>
/// Metoda powinna przyjmować następujące parametry:<br/>
/// - TAccumulate seed: inicjalna wartość akumulatora,<br/>
/// - Func accumulator: funkcja, która realizuje logikę akumulacji,
///   przyjmującą dwie wartości typu TAccumulate i TSource,
///   zwracającą wartość typu TAccumulate.<br/>
/// Metoda powinna iterować przez elementy sekwencji source
/// i aktualizować wynikowy akumulator przy użyciu dostarczonej funkcji.<br/>
/// Przy użyciu metody Reduce zaimplementuj trzy metody
/// rozszerzające interfejs IEnumerable&lt;T&gt;,
/// przy ograniczeniu typu T do interfejsu INumber&lt;T&gt;:<br/>
/// - Sum - zwracającą sumę T sekwencji<br/>
/// - Min - zwracającą minimalny element T sekwencji<br/>
/// - Max - zwracającą maksymalny element T sekwencji<br/>
/// Wskazówki:<br/>
/// - T.CreateTruncating(double.NegativeInfinity)<br/>
/// - T.CreateTruncating(double.PositiveInfinity)<br/>
/// - T.Zero<br/>
/// - T.Min<br/>
/// - T.Max
/// </summary>
public static class EnumerableExtensions
{
    //TODO: Reduce<TSource, TAccumulator>

    public static T Reduce<T,S>(this IEnumerable<S> source, T acum, Func<T,S,T> a)
    {   
        foreach(var i in source)
        {
            acum = a(acum,i);
        }
        return acum;
    }


    public static T Sum<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        return source.Reduce(T.IsZero,(accum,num)=>num+accum);
    }

    public static T Max<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        return source.Reduce(T.CreateTruncating(double.PositiveInfinity),(accum,num)=>Math.Max(num,accum));
    }

    public static T Min<T>(this IEnumerable<T> source) where T : INumber<T>
    {
        // TODO: Min
        return source.Reduce(T.CreateTruncating(double.NegativeInfinity),(accum,num)=>Math.Min(num,accum));
    
    }
}