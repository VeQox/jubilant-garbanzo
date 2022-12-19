using System.Net.WebSockets;

ClientWebSocket ws = new ClientWebSocket();
await ws.ConnectAsync(
    new Uri("ws://localhost:8080"),
    new CancellationToken());

Thread inputThread = new(() =>
{
  while (true)
  {
    if (Console.KeyAvailable)
    {
      var key = Console.ReadKey(true);
      ws.SendAsync(new ArraySegment<byte>(ConvertToByteArray(key.Key.ToString())), WebSocketMessageType.Text, true, CancellationToken.None);
    }
  }
});

inputThread.Start();

var buffer = new byte[1024];
var result = ConvertToString(buffer, await ws.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationToken()));
Console.WriteLine(result);

int width = int.Parse(result.Split(";")[0]);
int height = int.Parse(result.Split(";")[1]);

Console.SetWindowSize(width, height);
Console.SetBufferSize(width, height);
Console.CursorVisible = false;

while (true)
{
  result = ConvertToString(buffer, await ws.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationToken()));
  string[] positions = result.Split(";");
  Console.Clear();
  foreach (string position in positions)
  {
    try
    {
      int x = int.Parse(position.Split("|")[0]);
      int y = int.Parse(position.Split("|")[1]);
      Console.SetCursorPosition(x, y);
      Console.Write("x");
    }
    catch
    {

    }
  }
  //await ws.SendAsync(new ArraySegment<byte>(ConvertToByteArray("")), WebSocketMessageType.Text, true, CancellationToken.None);
}

byte[] ConvertToByteArray(string message)
{
  List<byte> encodedMessage = new();
  foreach (char ch in message)
    encodedMessage.Add(((byte)ch));
  return encodedMessage.ToArray();
}

string ConvertToString(byte[] buffer, WebSocketReceiveResult result)
{
  string parsedString = "";
  for (int i = 0; i < result.Count; i++)
  {
    parsedString += ((char)buffer[i]);
  }
  return parsedString;
}