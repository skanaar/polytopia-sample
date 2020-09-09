using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

// This class describes the state of the game.

public class GameState {
    private Tile[,] tiles;
    public GameState(Tile[,] tiles, IEnumerable<Unit> units) {
        this.tiles = tiles;
        Units = new List<Unit>(units);
    }
    public Tile GetTile(int x, int y) {
        return (IsWithinBounds(x, y)) ? tiles[x,y] : Tile.Invalid;
    }
    public Tile GetTile(Vector p) {
        return GetTile(p.X, p.Y);
    }
    public IList<Unit> Units { get; set; }
    public int Width => tiles.GetLength(0);
    public int Height => tiles.GetLength(1);
    public bool IsWithinBounds(int i, int j) {
        return i >= 0 && j >= 0 && i < tiles.GetLength(0) && j < tiles.GetLength(1);
    }
    // utility for visualizing the world in the Console
    public string AsciiVisualization() {
        var unit = Units[0];
        var msg = new StringBuilder();
        for (var j = 0; j<Height; j++) {
            for (var i = 0; i<Width; i++) {
                if (Units.Any(e => e.Pos.X==i && e.Pos.Y==j)) {
                    msg.Append("@ ");
                } else {
                    msg.Append(tiles[i, j] == Tile.Ground ? "# " : "~ ");
                }
            }
            msg.Append("\n");
        }
        return msg.ToString();
    }
}

public enum Tile { Water, Ground, Invalid }

public struct Vector {
    public Vector(int x, int y) {
        X = x;
        Y = y;
    }
    public int X { get; }
    public int Y { get; }
    
    public static Vector operator +(Vector a, Vector b) {
        return new Vector(a.X + b.X, a.Y+b.Y);
    }
    
    public string ToString() => $"({X}, {Y})";
}

public static class VectorOperations {
    public static bool IsAdjacent(this Vector a, Vector b) {
        return Math.Abs(a.X - b.X) < 2 && Math.Abs(a.Y - b.Y) < 2;
    }
}

public class Unit {
    public int Id { get; set; }
    public Vector Pos { get; set; }
    public int Movement = 3;
}