using System.Net.WebSockets;
using System.Text.Json;
namespace Client
{
  public class Client
  {
    public int ID { get; private set; }
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
      if (!Connect(uri)) return;
      SetConsoleWindow();
      InputThread.Start();

      while (WebSocket.State == WebSocketState.Open)
      {
        Update(await Receive());
      }
    }

    public void Update(Player[]? players)
    {
      if (players == null) return;
      Console.Clear();
      Console.SetCursorPosition(0, 0);
      Console.Write(players.AsEnumerable().Where(player => player.Id == ID).First().Score);
      foreach (Player player in players)
      {
        if (player.Coords == null) continue;
        foreach (Coord coord in player.Coords.CoordsCoords)
        {
          Console.SetCursorPosition(coord.X, coord.Y);
          Console.Write(player.Char);
        }
      }
    }

    public async Task<Player[]?> Receive()
    {
      byte[] buffer = new byte[1024];
      WebSocketReceiveResult response = await WebSocket.ReceiveAsync(buffer, CancellationToken.None);
      string jsonString = ConvertToString(buffer, response);
      return Player.FromJson(jsonString);
    }

    public async void Send(ConsoleKey key)
    {
      string jsonString = JsonSerializer.Serialize(key);
      await WebSocket.SendAsync(ConvertToBuffer(jsonString), WebSocketMessageType.Text, true, CancellationToken.None);
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
        try
        {
          if (Console.KeyAvailable)
          {
            var key = Console.ReadKey(true);
            Send(key.Key);
          }
        }
        catch
        {

        }

      }
    }

    private bool Connect(Uri uri)
    {
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      try
      {
        WebSocket.ConnectAsync(uri, cancellationTokenSource.Token).Wait(cancellationTokenSource.Token);
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
      ID = dimensions[2];


      try
      {
        Console.SetWindowSize(width, height);
        Console.SetBufferSize(width, height);
        Console.CursorVisible = false;
      }
      catch
      {

      }
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

