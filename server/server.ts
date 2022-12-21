import { IncomingMessage } from "http"
import { WebSocket, RawData, WebSocketServer } from "ws"
import { Tail, Coord } from "./client"
import Client from "./client";

const port = 8080;
const width = 100;
const height = 40;

const wss: WebSocketServer = new WebSocket.Server({ port });

let clients: Client[] = [];
let updateClients: boolean = false;

wss.on("listening", () => {
    console.log(`Server is listening on port ${port}`);
});

wss.on("connection", (ws: WebSocket, request: IncomingMessage) => {
    console.log(`${clients.length} connections`);
    let tail: Tail = new Tail(new Coord(Math.round(Math.random() * (width - 1)), Math.round(Math.random() * (height - 1))), 10);
    let client: Client = new Client(tail, ws, 'X', clients.length)
    ws.send(`${width};${height}`);
    clients.push(client);
    ws.on("message", (msg: RawData) => {
        console.log(`${client.id}:${msg.toString()}`)
        let { X, Y } = client.Coords.peek()!;
        switch (msg.toString()) {
            case "LeftArrow":
                if (X != 0)
                    client.Coords.add(new Coord(X - 1, Y))
                break;
            case "UpArrow":
                if (Y != 0)
                    client.Coords.add(new Coord(X, Y - 1))
                break;
            case "RightArrow":
                if (X < width)
                    client.Coords.add(new Coord(X + 1, Y));
                break;
            case "DownArrow":
                if (Y < height)
                    client.Coords.add(new Coord(X, Y + 1));
                break;
        }
        updateClients = true;
    });

    ws.on("close", (code: number) => {
        clients = clients.filter((connection) => connection.ws != ws);
        console.log(`${clients.length} connections`)
    });
});

setInterval(() => {
    if (!updateClients) return;
    for (let client of clients) {
        client.ws.send(JSON.stringify(clients));
    }
    updateClients = false;
    //console.log(`avg ${ (avgMS / connections.length).toFixed(2) } ms \n${ connections.length } connections`);
}, 50);