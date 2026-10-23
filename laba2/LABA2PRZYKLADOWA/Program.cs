using System.Runtime.CompilerServices;

public interface IMovable
{
    public string move();
}

public abstract class Vehicle{
    protected string Brand{get;set;}
    protected int speed{get;set;}

    public  Vehicle(int speed,string Brand)
    {
       this.Brand = Brand;
       this.speed = speed;  
    }

    public abstract double calculate();
    public abstract double calculate(double discount);
    public abstract override string ToString();

    
}

class Car : Vehicle, IMovable
{
    public int FuelConsumption{get;set;}
    public Car(int speed,string Brand,int FuelConsumption) : base(speed, Brand)
    {
        this.FuelConsumption = FuelConsumption; 
    }
    public override string ToString()
    {
        return $"THIS CAR CONSUMES {FuelConsumption}";
    }
    public override double calculate()
    {
        return (double)speed*FuelConsumption;
    }
    public override double calculate(double discount)
    {
        return this.calculate() * (1 - discount);
    }

    public string move()
    {
        return "THE CAR HAS MOVED: cool probably";
    }
    public static Car operator+(Car first,Car second)
    {
        return new Car(first.speed + second.speed,first.Brand,first.FuelConsumption);
    }
}
public class Bike:Vehicle,IMovable
{
    int cost;
    public Bike(int i,string j,int cost) : base(i, j)
    {
        this.cost = cost;
    }
    public override double calculate()
    {
        return this.cost;
    }

    public override double calculate(double discount)
    {
        return this.calculate()*(1-discount);
    }
    public override string ToString()
    {
        return "BIKE";
    }
    public string move()
    {
        return "THE BIKE MOVED";
    }
}