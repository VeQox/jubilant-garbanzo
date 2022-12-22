import { WebSocket } from "ws";
export default class Client {
  public Char: string;
  public id: number;
  public score: number;
  public Coords: Tail;
  public ws: WebSocket;

  public constructor(tail: Tail, ws: WebSocket, char: string, id: number) {
    this.Coords = tail;
    this.ws = ws;
    this.Char = char;
    this.id = id;
    this.score = 0;
  }

  public intersects(o: Client): boolean {
    let intersects = false;
    o.Coords.Coords.forEach(coord => {
      if (this.Coords.intersects(coord)) intersects = true;
    })
    if (intersects) o.score += o.Coords.length;
    return intersects;
  }

  public respawn(width: number, height: number) {
    this.Coords = new Tail(new Coord(Math.round(Math.random() * (width - 1)), Math.round(Math.random() * (height - 1))), 1);
  }
}

export class Tail {
  Coords: Coord[] = [];
  length: number = 10;

  constructor(coord: Coord, length: number) {
    this.length = length > 0 ? length : this.length;
    this.add(coord)
  }

  public add(coord: Coord) {
    this.Coords.splice(0, 0, coord);
    if (this.Coords.length > this.length) {
      this.pop();
    }
  }
  public peek(): Coord | undefined {
    return this.Coords.at(0);
  }
  public pop() {
    this.Coords.splice(this.Coords.length - 1, 1);
  }

  public intersects(coord: Coord): boolean {
    for (let { X, Y } of this.Coords) {
      if (coord.X === X && coord.Y === Y) return true;
    }
    return false;
  }
}

export class Coord {
  public X: number;
  public Y: number;

  constructor(x: number, y: number) {
    this.X = x;
    this.Y = y;
  }
}