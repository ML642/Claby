using System.Security.Cryptography.X509Certificates;

public interface Iscalable
{
    void Scale(double factor);

}

public abstract class Shape
{
    public string Name { get; }

    protected Shape(string name)
    {
        Name = name;
    }

    public abstract double Area();
    public virtual string Describe()
    {
        return "base case";
    }
}
class Circle : Shape,Iscalable
{
    public  double Radius{get;private set;}

    public Circle(string name,double radius) : base(name)
    {
        if(radius<=0)throw new ArgumentException("Value must be positive");

        this.Radius =  radius;
    }

    public override double Area()
    {
        return  Math.PI * Radius * Radius;
    }

    public override string Describe()
    {
        return $"this circle radius is {Radius}";
    }

    public void Scale(double factor)
    {
        if(factor<=0)throw new ArgumentException("Value must be positive");
        Radius = this.Radius * factor;
    }
    public override string ToString()
    {
        return "THIS";
    }
    public static Circle operator+(Circle one,Circle two)
    {
        return new Circle("name",one.Area()+two.Area());       
    }
    public double  this[int key]
    {
        get
        {
            return this.Area();
        }
        set
        {
            Scale(value);
        }
    }
    public void Deconstructor(out double  x,out double y)
    {
        x = this.Radius;
        y = this.Area();
    }
}
