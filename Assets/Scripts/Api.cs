using System.Collections.Generic;

// These are the Request and Response objects that would be serialized
// and sent over the network to a (maybe external) server

// Every Request contain the entire GameState before the action
// Responses that modify the world also contain the resulting GameState
// This approach mean that the server validating moves can be stateless
// which is makes it easier to scale the servers.

public class UnitMoveRequest {
    public GameState GameState { get; set; }
    public int UnitIndex { get; set; }
    public Vector[] Path { get; set; }
}

public class UnitMoveResponse {
    public bool IsSuccess { get; set; }
    public GameState GameState { get; set; }
    
    public static UnitMoveResponse Failure(GameState state) {
        return new UnitMoveResponse { IsSuccess = false, GameState = state };
    }
}

public class PossibleMovementRequest {
    public GameState GameState { get; set; }
    public int UnitIndex { get; set; }
}

public class PossibleMovementResponse {
    public bool IsSuccess { get; set; }
    public IList<Vector> Destinations { get; set; }
}

public enum MapType { HalfWaterHalfLand }

public class GameStateRequest {
    public MapType MapType { get; set; } = MapType.HalfWaterHalfLand;
    public int Size { get; set; } = 9;
}

public class GameStateResponse {
    public bool IsSuccess { get; set; }
    public GameState GameState { get; set; }
}
