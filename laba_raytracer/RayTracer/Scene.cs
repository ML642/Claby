using System.Collections.Generic;

 

 public class Scene : IHittable{
    public List<IHittable> Objects{get;} = new List<IHittable>();

    public void Add(IHittable obj) => Objects.Add(obj);

    public bool Hit(Ray ray, float tMin, float tMax, out HitInfo hitInfo)
    {
        hitInfo = default;
        bool hitAnything = false;
        float closest = tMax;

        foreach (IHittable obj in Objects)
        {
            if (obj.Hit(ray, tMin, closest, out HitInfo candidate))
            {
                hitAnything = true;
                closest = candidate.T;
                hitInfo = candidate;
            }
        }

        return hitAnything;
 }
 }