using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Lab08
{
	interface IMultiSeqOperation
	{        
		string Info { get; }
	}

	static class SequenceHelpers
    {
        public static IEnumerable FirstNSeqElem(int N, IEnumerable enumeration)
        {
            int curr = 0;
            var enumar = enumeration.GetEnumerator();
            while(enumar.MoveNext && curr < N)
            {
                yield return enumar.Current;
                curr++;
            }
        }

        public static  IEnumerable<string> SeqFormat(IEnumerable enumeration, string format = "{0}")
        {
            foreach(var item in enumeration)
            {
                yield return string.Format(format,item);
            }
        }
        public static void PrintSequence(IEnumerable enumeration, int limit = 15)
        {
                if (enumeration is ISequence sequence)
                    Console.WriteLine(sequence.Name);

                int index = 0;

                foreach(var item in FirstNSeqElem(limit, enumeration))
                {
                    // Dwie pierwsze spacje to wcięcie; {0,3} wypisuje indeks w polu
                    // o szerokości co najmniej 3 znaków (wyrównanie do prawej), a {1} to wartość elementu.
                    Console.WriteLine("  {0,3}: {1}", index, item);
                    index++;
                } 
        }   
    }   
    
}
