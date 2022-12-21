import { IncomingMessage } from "http"
import { WebSocket, RawData, WebSocketServer } from "ws"
import { Tail, Coord } from "./client"
import Client from "./client";

const port = 8080;
const width = 100;
const height = 40;

const wss: WebSocketServer = new WebSocket.Server({ port });

let clients: Client[] = [];
let updateClients: boolean = true;

wss.on("listening", () => {
    console.log(`Server is listening on port ${port}`);
});

wss.on("connection", (ws: WebSocket, request: IncomingMessage) => {
    let tail: Tail = new Tail(new Coord(Math.round(Math.random() * (width - 1)), Math.round(Math.random() * (height - 1))), 10);
    let client: Client = new Client(tail, ws, 'X', clients.length)
    ws.send(`${width};${height};${client.id}`);
    clients.push(client);
    console.log(`${clients.length} connections`);
    ws.on("message", (msg: RawData) => {
        let { X, Y } = client.Coords.peek()!;
        let keyCode : number = Number(msg.toString())
        switch (keyCode) {
            case 37:
                if (X != 0)
                    client.Coords.add(new Coord(X - 1, Y))
                break;
            case 38:
                if (Y != 0)
                    client.Coords.add(new Coord(X, Y - 1))
                break;
            case 39:
                if (X < width)
                    client.Coords.add(new Coord(X + 1, Y));
                break;
            case 40:
                if (Y < height)
                    client.Coords.add(new Coord(X, Y + 1));
                break;
        }
        updateClients = true;

        clients.filter(_client => _client != client).forEach(_client => {
            if(client.intersects(_client)) client.respawn(width, height);
        })
    });

    ws.on("close", (code: number) => {
        clients = clients.filter((connection : Client) => connection != client);
        console.log(`${clients.length} connections`)
    });
});

let lastJsonString = "";
setInterval(() => {
    let jsonString = JSON.stringify(clients, (key:string, value:any) => {
        if(key === "ws") return undefined;
        if(key === "length") return undefined;
        return value;
    });
    if(lastJsonString == jsonString) return;
    for (let client of clients) {
        client.ws.send(jsonString);
    }
    lastJsonString = jsonString;

    //console.log(`avg ${ (avgMS / connections.length).toFixed(2) } ms \n${ connections.length } connections`);
}, 50);