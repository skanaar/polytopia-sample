using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

// Every Request contain the entire GameState before the action
// Responses that modify the world also contain the resulting GameState
// Clients are then supposed to replace their GameState with the fresh copy
// sent with the Response.
// This approach mean that the server validating moves can be stateless
// which is makes it easier to scale the servers.
public class GameServer {

    // Get initial game state. Input specifies world size and map generation algorithm.
    public async Task<GameStateResponse> SendAsync(GameStateRequest req) {
        
        var unit = new Unit { Id = 0, Pos = new Vector(req.Size/2, req.Size/2) };

        var tiles = new Tile[req.Size, req.Size];
        if (req.MapType == MapType.HalfWaterHalfLand){
            for (var j = 0; j<req.Size; j++) {
                for (var i = 0; i<req.Size; i++) {
                    tiles[i,j] = i > req.Size/2 ? Tile.Water : Tile.Ground;
                }
            }
        }
        var game = new GameState(tiles, new []{ unit });
        UnityEngine.Debug.Log(game.AsciiVisualization());
        return new GameStateResponse { IsSuccess = true, GameState = game };
    }

    // Ask server what destinations we can move to. Returns a list of possible locations.
    public async Task<PossibleMovementResponse> SendAsync(PossibleMovementRequest req) {
        var game = req.GameState;
        var unit = game.Units[req.UnitIndex];
        var destinations = new List<Vector>{ unit.Pos };

        // start walking from the units' position and track destinations
        ExplorePossiblePaths(unit.Pos.X, unit.Pos.Y, unit.Movement);

        void ExplorePossiblePaths(int x, int y, int movementLeft) {
            if (movementLeft == 0) {
                // movement has been consumed, exit
                return;
            }
            movementLeft = movementLeft - 1;
            // recursively explore one step from the current position
            for(var i = Math.Max(0, x-1); i<=Math.Min(x+1, game.Width-1); i++) {
                for(var j = Math.Max(0, y-1); j<=Math.Min(y+1, game.Height-1); j++) {
                    // track destination
                    destinations.Add(new Vector(i, j));
                    // consume all movement if unit moved across a sea/land boundary
                    var didCrossShore = game.GetTile(x, y) != game.GetTile(i, j);
                    ExplorePossiblePaths(i, j, didCrossShore ? 0 : movementLeft);
                }
            }
        }
        
        return new PossibleMovementResponse {
            IsSuccess = true,
            Destinations = destinations.Distinct().ToList(), // remove duplicate destinations
        };
    }

    // Move a unit. If successful returns the new GameState after unit has moved
    public async Task<UnitMoveResponse> SendAsync(UnitMoveRequest req) {
        var unit = req.GameState.Units[req.UnitIndex];
        var unitPos = unit.Pos;
        var hasChangedTileType = false;

        foreach (Vector p in req.Path) {
            
            // unit cannot move after entering/exiting water
            if (hasChangedTileType) {
                UnityEngine.Debug.Log("hasChangedTileType");
                return UnitMoveResponse.Failure(req.GameState);
            }
            // unit can only move one step at a time
            if (!unitPos.IsAdjacent(p)) {
                UnityEngine.Debug.Log("unit can only move one step at a time");
                return UnitMoveResponse.Failure(req.GameState);
            }
            // unit cannot move outside of the world
            if (req.GameState.GetTile(p) == Tile.Invalid) {
                UnityEngine.Debug.Log("unit cannot move outside of the world");
                return UnitMoveResponse.Failure(req.GameState);
            }
            // remember if we crossed a water boundary
            if (req.GameState.GetTile(p) != req.GameState.GetTile(unitPos)) {
                hasChangedTileType = true;
            }
            // everything looked good! move unit
            unitPos = p;
        }
        // apply the verified destination to the unit
        unit.Pos = unitPos;
        
        // return success with a new GameState that describe the world
        return new UnitMoveResponse {
            IsSuccess = true,
            GameState = req.GameState
        };
    }
}