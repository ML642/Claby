using Program_helper;

public class Plane: IHittable
{
    public Vector3 Point { get; set; }
    public Vector3 Normal { get; set; }
    public Material Mat { get; set; }
    public Plane(Vector3 point,Vector3 normal,Material mat){
        this.Point = point;
        this.Normal = normal.Normalize();
        this.Mat = mat;
    }

    public bool Hit(Ray ray,float tMin,float tMax,out HitInfo hitInfo){
        hitInfo = default;

        float denom = Vector3.Dot(Normal,ray.Direction);

        if(MathF.Abs(denom) < 1e-6f){
            return false;
        }

        float t = Vector3.Dot(Point - ray.Origin,Normal) / denom;
        if(t <= tMin || t >= tMax ){
            return false;
        }
        hitInfo.T = t;
        hitInfo.Position = ray.At(t);
        hitInfo.Mat = Mat;
        hitInfo.SetFaceNormal(ray,Normal);
        return true;
    }

}
