import {IncomingMessage} from "http"
import { WebSocket, RawData, WebSocketServer} from "ws"

const port = 8080;

const wss : WebSocketServer = new WebSocket.Server({port});

wss.on("listening", () => {
    console.log(`Server is listening on port ${port}`);
});

wss.on("connection", (ws : WebSocket, request : IncomingMessage) => {
    console.log("Hello there");
    ws.on("message", (msg : RawData)=> {
        console.log(msg.toString());
    });

    ws.on("close", (code : number) => {
        console.log("Bye bye sucka");
    });
});