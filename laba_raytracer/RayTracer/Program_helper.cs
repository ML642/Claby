
namespace Program_helper;



public struct Vector3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public static  Vector3 RandomInUnitSphere()
    {
    
        
        double x = System.Random.Shared.NextDouble() * 2 - 1  ;
        double y = System.Random.Shared.NextDouble() * 2 - 1  ;
        double z = System.Random.Shared.NextDouble() * 2 - 1  ;

        while(x*x + y*y + z*z > 1)
        {
             x = System.Random.Shared.NextDouble() * 2 - 1  ;
             y = System.Random.Shared.NextDouble() * 2 - 1  ;
             z = System.Random.Shared.NextDouble() * 2 - 1  ;
        }
        return new Vector3((float)x,(float)y,(float)z);
    }
    public bool NearZero(Vector3 v)
    {
        if (MathF.Abs(v.x) < 1e-8f || MathF.Abs(v.y) < 1e-8f || MathF.Abs(v.z) < 1e-8f)
        {
            return true;
        }
        return false;
    }

     
    public override string ToString()
    {
        return $"{x}, {y}, {z}";
    }
    public static Vector3 operator +(Vector3 u, Vector3 v)
    {
        return new Vector3(u.x + v.x, u.y + v.y, u.z + v.z);
    }

    public static Vector3 operator -(Vector3 u)
    {
        return new Vector3(-u.x, -u.y, -u.z);
    }

    public static Vector3 operator -(Vector3 u, Vector3 v)
    {
        return new Vector3(u.x - v.x, u.y - v.y, u.z - v.z);
    }

    public static Vector3 operator *(Vector3 first, Vector3 second)
    {
        return new Vector3(first.x * second.x, first.y * second.y, first.z * second.z);
    }

    public static Vector3 operator *(Vector3 v, float t)
    {
        return new Vector3(v.x * t, v.y * t, v.z * t);
    }
    public static Vector3 operator /(Vector3 v, float t)
    {
        if (t == 0) throw new DivideByZeroException();
        else
        {
            return new Vector3(v.x / t, v.y / t, v.z / t);
        }
    }

    public float Length()
    {
        return MathF.Sqrt(x * x + y * y + z * z);
    }
    public float LengthSquared()
    {
        return x * x + y * y + z * z;
    }

    public bool NearZero()
    {
        const float threshold = 1e-8f;
        return MathF.Abs(x) < threshold
            && MathF.Abs(y) < threshold
            && MathF.Abs(z) < threshold;
    }

    

    public static Vector3 RandomUnitVector()
    {
        while (true)
        {
            Vector3 sample = RandomInUnitSphere();
            if (!sample.NearZero())
            {
                return sample.Normalize();
            }
        }
    }   

    public static float Dot(Vector3 a, Vector3 b)
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    public static Vector3 Cross(Vector3 u, Vector3 v)
    {
        return new Vector3(
            u.y * v.z - u.z * v.y,
            u.z * v.x - u.x * v.z,
            u.x * v.y - u.y * v.x
        );
    }
    public Vector3 Normalize()
    {
        float length = this.Length();
        if(length == 0)
        {
            return new Vector3(0,0,0);
        }
        
        return new Vector3(
            x/length,
            y/length,
            z/length
        );
    }
    public static Vector3 operator*(float t,Vector3 v){
                return new Vector3(v.x * t, v.y * t, v.z * t);

    }

}

