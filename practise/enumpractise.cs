using System.Security.Cryptography.X509Certificates;

public enum stat
{
    Created,
    Paid,
    Shipped,
    Delivered
}

namespace practise
{
    public class practise
    {
        
    public class Player
{
    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public string Name { get; }
    public Position CurrentPosition { get; set; }

    public Player(string name, int x, int y)
    {
        Name = name;
        CurrentPosition = new Position(x, y);
        }
    }
    Player player = new Player("Martsin", 10, 20);
    Player.Position position = new Player.Position(10, 20);
}

}