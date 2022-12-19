import { IncomingMessage } from "http"
import { WebSocket, RawData, WebSocketServer } from "ws"

const port = 8080;
const width = 100;
const height = 40;

const wss: WebSocketServer = new WebSocket.Server({ port });

interface Connection {
    ws: WebSocket,
    ms: number,
    lastPackage: number,
    x: number;
    y: number;
};

let connections: Connection[] = [];

wss.on("listening", () => {
    console.log(`Server is listening on port ${port}`);
});

wss.on("connection", (ws: WebSocket, request: IncomingMessage) => {
    let connection: Connection = {
        ws: ws,
        ms: 0,
        lastPackage: 0,
        x: 0,
        y: 0,
    };
    ws.send(`${width};${height}`);
    connections.push(connection);
    console.log(`${connections.length} connections`);
    ws.on("message", (msg: RawData) => {
        if (msg.toString().length == 0)
            connection.ms = (Date.now() - connection.lastPackage) / 2;
        else {
            switch (msg.toString()) {
                case "LeftArrow":
                    if (connection.x != 0)
                        connection.x--;
                    break;
                case "UpArrow":
                    if (connection.y != 0)
                        connection.y--;
                    break;
                case "RightArrow":
                    if (connection.x != width - 1)
                        connection.x++;
                    break;
                case "DownArrow":
                    if (connection.y != height - 1)
                        connection.y++;
                    break;
            }
        }
    });

    ws.on("close", (code: number) => {
        connections = connections.filter((connection) => connection.ws != ws);
    });
});

setInterval(() => {
    let positions = "";
    for (let connection of connections) {
        positions += `${connection.x}|${connection.y};`;
    }
    for (let connection of connections) {
        connection.ws.send(positions);
    }
    //console.log(`avg ${(avgMS / connections.length).toFixed(2)} ms \n${connections.length} connections`);
}, 50);