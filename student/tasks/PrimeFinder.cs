namespace tasks;

/// <summary>
/// Here you should implement the Sieve of Eratosthenes algorithm to find all prime numbers up to a given upper bound.
/// </summary>

public static class PrimeFinder
{   
    public static IEnumerable<int> SieveOfEratosthenes(int upperBound)
    {   
        bool[] composite = new bool[upperBound + 1];

        for(int i = 2 ; i <= upperBound ; i++)
        {
            if(composite[i] == false){
                yield return i;
           
                int j = i + i;
                while( j <= upperBound)
                {
                   
                    composite[j] = true; 
                    j=j+i;
                }
            }
        }
       yield break;
    }
}