
using System.ComponentModel;
using Program_helper;

public class EmissiveMaterial : Material
{
    
    Vector3 color;

    public Vector3 Color{get=>color;set{color = value;}}

    public EmissiveMaterial(Vector3 Color)
    {
        this.color = Color;
    }

    public override bool Scatter(in Ray rIn, ref HitInfo hitINfo,out Vector3 attenuation,out Ray scattered)
    {
        Vector3 attenuation1 = new Vector3();
        attenuation = attenuation1;

        Ray scattered1 = new Ray();
        scattered = scattered1;

        return false;
    }


    public override Vector3 Emit()
    {
        return color;
    }

}
