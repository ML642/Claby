
using Image1;

using Program_helper;
public class Camera{
    private float _fow = 90;
    
    public int MaxDepth { get; set; } = 50;
    
    public float AspectRatio{
        get=>_aspectRatio;
        set{
            _aspectRatio = value;
            Recalculate();
        }
    }
    
    private float _aspectRatio = 16f/9f;
    
    
    
    private int _imageWidth = 1280;
    public int ImageWidth {
        get=>_imageWidth;
        set{
            _imageWidth = value;
            Recalculate();
        }
    }
    

    public Vector3 Position{
        get=>_position;
        set{
            _position = value;
            Recalculate();
        }
    }
    public Vector3 Target {
        get=>_target;
        set{
            _target = value;
            Recalculate();
        }
    }

    private Vector3 _position = new Vector3(0,0,0);
    private Vector3 _target = new Vector3(0,0,-1);

    private int _imageHeight;
    private Vector3 _pixelDu;
    private Vector3 _pixelDv;
    private Vector3 _viewportCorner;

    public float Fov{
        get=>_fow;
        set{
            _fow = value;
            Recalculate();
        }
    }
    public Camera(){
        Recalculate();
    }
    
    public void Recalculate(){
         _imageHeight = (int)(ImageWidth / AspectRatio);
        float theta = Fov * MathF.PI / 180.0f;
        float h = MathF.Tan(theta / 2.0f);
        float  distanceToViewport = (Position - Target).Length();
        float viewportHeight = 2.0f * h * distanceToViewport;
        float viewportWidth = viewportHeight * AspectRatio;

        Vector3 w = (Position - Target).Normalize();
        Vector3  u = Vector3.Cross(new Vector3(0, 1, 0), w).Normalize();
        Vector3  v = Vector3.Cross(w, u);

        Vector3 viewportU = viewportWidth * u;
        Vector3 viewportV = viewportHeight * -v;
        
        _pixelDu = viewportU / ImageWidth;
        _pixelDv = viewportV / _imageHeight;

        _viewportCorner = Position - (distanceToViewport * w) - viewportU / 2.0f - viewportV / 2.0f;
        _viewportCorner += (_pixelDu + _pixelDv) * 0.5f;
    
    }


    private Vector3 RayColor(Ray ray, int MaxDepth,IHittable world)
    {
        if(MaxDepth <= 0) return  new Vector3(0,0,0);



        if (!world.Hit(ray, 0.001f, float.PositiveInfinity, out HitInfo hitInfo))
        {
            return new Vector3(0, 0, 0);
        }

        Vector3 nowe_swiatlo  = hitInfo.Mat.Emit();

        if(hitInfo.Mat.Scatter(ray,ref hitInfo,out Vector3 attenuation,
        out Ray scattered))
        {
            return nowe_swiatlo + attenuation * RayColor(scattered,MaxDepth-1, world);
        }
        return nowe_swiatlo;

    }
    public Image Render(IHittable world){
    Image image = new Image(ImageWidth, _imageHeight);

    for (int j = 0; j < _imageHeight; j++)
    {
        for (int i = 0; i < ImageWidth; i++)
        {
            Vector3 pixelCenter =
                _viewportCorner + i * _pixelDu + j * _pixelDv;
            Vector3 rayDirection = pixelCenter - Position;
            Ray ray = new Ray(Position, rayDirection);

            image[i, j] = RayColor(ray, MaxDepth, world);
        }
    }

    return image;
}

}
