using Program_helper;

public struct Ray
{   
    public Vector3 Origin { get; init; }

    public Vector3 Direction { get; init; }

    public Ray(Vector3 origin , Vector3 Direction)
    {
        this.Origin = origin;
        this.Direction = Direction;
    }

    public Vector3 At(float t)
    {
        return this.Direction * t + this.Origin;
    }


}