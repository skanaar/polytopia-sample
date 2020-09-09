# polytopia-sample

The server design is based on the concept of a stateless server where the entire game state is sent on every request. This means that the servers are very easy to scale with no shared data.

It is also possible to store the entire game session as a series of commands, and validate the game in the end when high-scores are send instead of validating it on every move.

If the servers must be stateful (for matchmaking, for example) then the map state can be stored as a simple hash of the entire state. Validating that client and server states match is a matter of comparing that hash.

## Request/Response

Every Request contain the entire **GameState** before the action.

Responses that modify the world also contain the resulting **GameState**.
Clients are then supposed to replace their **GameState** with the fresh copy sent with the **Response**.

## Domain objects

![](https://www.nomnoml.com/image.svg?source=%23%23.frame%3A%20direction%3Ddown%0A%5BGameState%5D-%3E%5B%3Ctable%3ETile%7CGround%7CWater%5D%0A%5BGameState%5D-%3E%5BUnit%7CMovement%3A%20Int%3BPos%3A%20Vector%5D%0A%5BGameServer%7C%7CSendAsync(Request)%5D%0A%5BGameServer%5D%20--%3E%20%5BPossibleMovementRequest%5D%0A%5BGameServer%5D%20--%3E%20%5BUnitMoveRequest%5D%0A%5BGameServer%5D%20--%3E%20%5BGameStateRequest%5D%0A%0A)

## Data flow

![](https://www.nomnoml.com/image.svg?source=%5B%3Cactor%3Estart%20game%5D%20GameStateRequest%20-%3E%20%5B%3Cstate%3EGenerate%20World%5D%0A%5B%3Cstate%3EGenerate%20World%5D%20GameStateResponse%20-%3E%20%5B%3Cactor%3Eselect%20unit%5D%0A%5B%3Cactor%3Eselect%20unit%5D%20PossibleMovementRequest%20-%3E%20%5B%3Cstate%3ECalculate%20movements%5D%0A%5B%3Cstate%3ECalculate%20movements%5D%20PossibleMovementResponse%20-%3E%20%5B%3Cactor%3Eselect%20destination%5D%0A%5B%3Cactor%3Eselect%20destination%5D%20UnitMoveRequest%20-%3E%20%5B%3Cstate%3E%20Validate%20movement%5D%0A%5B%3Cstate%3E%20Validate%20movement%5D%20UnitMoveResponse%20-%3E%20%5B%3Cactor%3E%20%F0%9F%91%8D%5D)
