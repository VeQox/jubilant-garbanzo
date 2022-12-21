namespace Client
{
  public class Coord
  {
    public int X { get; set; }
    public int Y { get; set; }
  }
  public class Player
  {
    public char Char { get; set; }
    public Coord[]? Coords { get; set; }
  }
  public class ServerMessage
  {
    public Player[]? Players { get; set; }
  }

  public class ClientMessage
  {
    public ConsoleKey Key { get; set; }
  }
}