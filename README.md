# polytopia-sample

Every Request contain the entire **GameState** before the action.

Responses that modify the world also contain the resulting **GameState**.
Clients are then supposed to replace their **GameState** with the fresh copy sent with the **Response**.

This approach mean that the server validating moves can be stateless which is makes it easier to scale the servers.

## Domain objects

![](https://www.nomnoml.com/image.svg?source=%23%23.frame%3A%20direction%3Ddown%0A%5BGameState%5D-%3E%5B%3Ctable%3ETile%7CGround%7CWater%5D%0A%5BGameState%5D-%3E%5BUnit%7CMovement%3A%20Int%3BPos%3A%20Vector%5D%0A%5BGameServer%7C%7CSendAsync(Request)%5D%0A%5BGameServer%5D%20--%3E%20%5BPossibleMovementRequest%5D%0A%5BGameServer%5D%20--%3E%20%5BUnitMoveRequest%5D%0A%5BGameServer%5D%20--%3E%20%5BGameStateRequest%5D%0A%0A)

## Data flow

![](https://www.nomnoml.com/image.svg?source=%5B%3Cactor%3Estart%20game%5D%20GameStateRequest%20-%3E%20%5B%3Cstate%3EGenerate%20World%5D%0A%5B%3Cstate%3EGenerate%20World%5D%20GameStateResponse%20-%3E%20%5B%3Cactor%3Eselect%20unit%5D%0A%5B%3Cactor%3Eselect%20unit%5D%20PossibleMovementRequest%20-%3E%20%5B%3Cstate%3ECalculate%20movements%5D%0A%5B%3Cstate%3ECalculate%20movements%5D%20PossibleMovementResponse%20-%3E%20%5B%3Cactor%3Eselect%20destination%5D%0A%5B%3Cactor%3Eselect%20destination%5D%20UnitMoveRequest%20-%3E%20%5B%3Cstate%3E%20Validate%20movement%5D%0A%5B%3Cstate%3E%20Validate%20movement%5D%20UnitMoveResponse%20-%3E%20%5B%3Cactor%3E%20%F0%9F%91%8D%5D)
