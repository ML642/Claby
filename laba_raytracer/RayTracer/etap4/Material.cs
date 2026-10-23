
using System.Runtime.Serialization;
using Program_helper;
public abstract class Material
{
    public abstract bool Scatter(in Ray rIn, ref HitInfo hitINfo,out Vector3 
    attenuation,out Ray scattered);

    public virtual Vector3 Emit()
    {
        return new Vector3(0,0,0);
    }
}

