using System.Text;
using Program_helper;
namespace Image1;

using Vector3 = Program_helper.Vector3;

public class Image
{
	public int height { get; init; }
	public int width
	{
		get;
		init;
	}
	private Vector3[,] Pixels;

	public Image(int width, int height) {
		this.height = height;
		this.width = width;

		this.Pixels = new Vector3[width, height];
	}
	public Vector3 this[int x, int y]
	{
		get
		{
			return this.Pixels[x,y];
		}
		set
		{
			this.Pixels[x,y] = value;
		}
	}

	public override string ToString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("P3");
		sb.AppendLine($"{width} {height}");
		sb.AppendLine("255");

		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
				Vector3 pixel = Pixels[j, i];
				sb.AppendLine($"{(int)(pixel.x*255)} {(int)(pixel.y*255)} {(int)(pixel.z * 255)}");
			}
		}
		return sb.ToString();
	}
}