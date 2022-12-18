using System.Net.WebSockets;

ClientWebSocket ws = new ClientWebSocket();
await ws.ConnectAsync(
    new Uri("ws://localhost:8080")
    , new CancellationToken());

await ws.SendAsync(new ArraySegment<byte>(new byte[]{(byte)'H',(byte)'e',(byte)'l',(byte)'l',(byte)'o',}), WebSocketMessageType.Text, true, new CancellationToken());