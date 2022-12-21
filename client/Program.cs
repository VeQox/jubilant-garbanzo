namespace Client
{
  public class Program
  {
    public static void Main()
    {
      Client client = new();
      client.Start(new Uri("ws://localhost:8080"));
    }
  }
}