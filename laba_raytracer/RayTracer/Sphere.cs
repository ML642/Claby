using Program_helper;


class Sphere : IHittable{

    public Vector3 Center{get;set;}
    public float Radius{get;set;}

    
    public Material Mat { get; set; }
    public Sphere(Vector3 сenter,float Radius,Material mat){
        this.Radius = Radius;
        this.Center = сenter;
        this.Mat = mat;
    }
    public bool Hit(Ray ray,float tMin, float tMax, out HitInfo hitInfo){
        Vector3 oc = ray.Origin - Center;
        hitInfo = default;

        hitInfo.Mat = Mat;

        float a = ray.Direction.LengthSquared();
        float halfB = Vector3.Dot(oc, ray.Direction);
        float c = oc.LengthSquared() - Radius * Radius;

        float discriminant = halfB * halfB - a * c;

        if(discriminant<0)return false;

        float sqrtD = MathF.Sqrt(discriminant);
        float root = (-halfB - sqrtD) / a;
        if(root < tMin || root > tMax){
            root = (-halfB + sqrtD) / a;
            if (root < tMin || root > tMax)
                return false;
        }
        hitInfo.T = root;
        hitInfo.Position = ray.At(root);
        Vector3 outwardNormal = (hitInfo.Position - Center) / Radius;

        hitInfo.SetFaceNormal(ray,outwardNormal);
        return true;
    }

}