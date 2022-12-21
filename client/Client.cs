using System.Net.WebSockets;
using System.Text.Json;
namespace Client
{
  public class Client
  {
    public int ID { get; }
    private ClientWebSocket WebSocket { get; set; }
    private Thread InputThread { get; set; }

    public Client()
    {
      ID = 0;
      WebSocket = new();
      InputThread = new(DetectKeys);
    }

    public async void Start(Uri uri)
    {
      if (!await Connect(uri)) return;
      SetConsoleWindow();
      InputThread.Start();

      while (WebSocket.State == WebSocketState.Open)
      {
        Update(await Receive());
      }
    }

    public void Update(ServerMessage? message)
    {
      if (message == null || message.Players == null) return;
      foreach (Player player in message.Players)
      {
        if (player.Coords == null) continue;
        foreach (Coord coord in player.Coords)
        {
          Console.SetCursorPosition(coord.X, coord.Y);
          Console.Write(player.Char);
        }
      }
    }

    public async Task<ServerMessage?> Receive()
    {
      byte[] buffer = new byte[1024];
      WebSocketReceiveResult response = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
      return JsonSerializer.Deserialize<ServerMessage>(ConvertToString(buffer, response));
    }

    public async void Send(ConsoleKey key)
    {
      await WebSocket.SendAsync(ConvertToBuffer(JsonSerializer.Serialize(key)), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private ArraySegment<byte> ConvertToBuffer(string message)
    {
      List<byte> encodedMessage = new();
      foreach (char ch in message)
        encodedMessage.Add(((byte)ch));
      return new ArraySegment<byte>(encodedMessage.ToArray());
    }

    private void DetectKeys()
    {
      while (WebSocket.State == WebSocketState.Open)
      {
        if (Console.KeyAvailable)
        {
          var key = Console.ReadKey(true);
          Send(key.Key);
        }
      }
    }

    private async Task<bool> Connect(Uri uri)
    {
      try
      {
        await WebSocket.ConnectAsync(uri, CancellationToken.None);
      }
      catch (Exception)
      {
        return false;
      }
      return true;
    }

    private async void SetConsoleWindow()
    {
      byte[] buffer = new byte[1024];
      string response = ConvertToString(buffer, await WebSocket.ReceiveAsync(buffer, CancellationToken.None));
      int[] dimensions = Array.ConvertAll(response.Split(";"), int.Parse);

      int width = dimensions[0];
      int height = dimensions[1];

      Console.SetWindowSize(width, height);
      Console.SetBufferSize(width, height);
      Console.CursorVisible = false;
    }

    private string ConvertToString(byte[] buffer, WebSocketReceiveResult result)
    {
      string parsedString = "";
      for (int i = 0; i < result.Count; i++)
      {
        parsedString += ((char)buffer[i]);
      }
      return parsedString;
    }
  }
}

