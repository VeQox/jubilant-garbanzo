namespace Client
{
  public class Program
  {
    public static void Main()
    {
      Client client = new();
      client.Start(new Uri("ws://172.17.221.110:8080"));
    }
  }
}