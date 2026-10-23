using System.Numerics;
using System.Security.Cryptography.X509Certificates;

public class Vector2
{   
    double x;
    double y;
    public double X
    {
        get => x; set
        {
            x = value;
        }}
    public double Y
    {
        get => y; set
        {
            y = value;
        }
    }

    public Vector2(double  x,double y)
    {
        this.x = x;
        this.y = y;
    }
    
    public double  Length()
    {
        return Math.Sqrt(this.x * this.x + this.y*this.y);
    }
    public void Move(double dx,double dy)
    {
        this.x += dx;
        this.y += dy; 
    }
    public void Move(double d)
    {
        this.x +=d;
        this.y +=d;
    }

    public static Vector2 operator+(Vector2 first,Vector2 second)
    {
        return new Vector2(first.x+second.x,first.y+second.y);
    }
    public static Vector2 operator+(Vector2 first,float qx)
    {
        return new Vector2(first.x * qx ,first.y * qx);
    }
    
    public double  this[int z]
    {
        get
        {
            if(z == 0)
            return this.x;
            else if(z == 1)
            {
                return this.y;
            }
            else 
            {
                throw new IndexOutOfRangeException();
            }
        }
        set
        {
            if(z == 0)
            {
                this.x = value;
            }
            else if(z == 1)
            {
                 this.y = value;
            }
            else 
            {
                throw new  IndexOutOfRangeException();
            }
        }
    }

    public void Deconstruct(out double x , out double y)
    {
        x = this.x;
        y = this.y;
    }
    ~Vector2()
    {
        Console.WriteLine("THE END");
    }
}

