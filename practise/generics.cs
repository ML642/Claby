using System.Diagnostics.Contracts;

public interface IShape
{
    double Area();
}
public class Box<T>
{
    T Value;

    public Box(T arg){
        this.Value = arg; 
    }

    public override string ToString()
    {   
        return Value?.ToString() ?? "null";
    }

    public M myfunc<M>(M ty)
    {
        return ty;
    }
    public (T1,T2) Create<T1,T2>(T1 first,T2 second) where T1 : new()
    {
        return (first,second);
    }

    public static T3 FindLarger<T3>(T3 first,T3 second) where T3 : IShape
    {
        if(first.Area() < second.Area())
        {
            return first;
        }
        else
        {
            return second;
        }
    }


    public interface IStorage<T>
    {
        void Save(T value);
        T Load();
    }

    public class IStor : IStorage<int>
    {
        public void Save(int value)
        {
            
        }
        public int Load()
        {
            return 3;
        }
    }


    public interface ICopyable<TSelf>
    {
        TSelf Copy();
    }


    public class Person : ICopyable<Person>
        {
            public string Name { get; }

            public Person(string name)
            {
                Name = name;
            }

            public Person Copy()
            {
                return new Person(Name);
            }
        }
}