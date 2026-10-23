using Program_helper;



public struct HitInfo{
    public Material Mat;
    public Vector3 Position;
    public Vector3 Normal;
    public float T;
    public bool FrontFace;
    
    public void SetFaceNormal(Ray r, Vector3 outwardNormal){
        float dot = Vector3.Dot(r.Direction,outwardNormal);
        if(dot < 0){
            FrontFace = true;
            Normal = outwardNormal;
        }
        else{
            FrontFace = false;
            Normal = outwardNormal * -1;
        }
    }
    
}