using Program_helper;
using Image1;

public class MainClass
{
    public static void Main()
    {
        Scene world = new Scene();
        Material material = new EmissiveMaterial(new Vector3(1,1,1));

        world.Add(new Sphere(new Vector3(0, 0, -1), 0.5f,material));
        world.Add(new Plane(
            new Vector3(0, -0.5f, 0),
            new Vector3(0, 1, 0),material));

        Camera cam = new Camera();
        cam.Position = new Vector3(0, 0, 1);
        cam.Target = new Vector3(0, 0, -1);

        Image image = cam.Render(world);
        Console.Write(image);
    }
}