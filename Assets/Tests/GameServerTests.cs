using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GameServerTests
    {
        [Test]
        public void InitialGameState()
        {
            Await(async () => {
                var server = new GameServer();
                var response = await server.SendAsync(new GameStateRequest());
                Assert.That(response.IsSuccess);
                Assert.That(response.GameState.Units.Count == 1);
            });
        }

        [Test]
        public void MovementDestinationsOneStep()
        {
            Await(async () => {
                var server = new GameServer();
                var initialResponse = await server.SendAsync(new GameStateRequest());
                var game = initialResponse.GameState;
                game.Units[0].Movement = 1;
                var moveResponse = await server.SendAsync(new PossibleMovementRequest{
                    GameState = initialResponse.GameState,
                    UnitIndex = 0,
                });
                Assert.That(moveResponse.IsSuccess);
                Assert.That(moveResponse.Destinations.Count, Is.EqualTo(9));
            });
        }

        [Test]
        public void MovementDestinationsThreeStep()
        {
            Await(async () => {
                var server = new GameServer();
                var initialResponse = await server.SendAsync(new GameStateRequest());
                var game = initialResponse.GameState;
                game.Units[0].Movement = 3;
                var moveResponse = await server.SendAsync(new PossibleMovementRequest{
                    GameState = initialResponse.GameState,
                    UnitIndex = 0,
                });
                Assert.That(moveResponse.IsSuccess);
                Assert.That(moveResponse.Destinations.Count, Is.EqualTo(35));
            });
        }

        [Test]
        public void MovementValidationBabyStep()
        {
            Await(async () => {
                var moveResponse = await MoveUnit(new Vector(0,1));
                Assert.That(moveResponse.IsSuccess);
                Assert.That(moveResponse.GameState.Units[0].Pos, Is.EqualTo(new Vector(4,5)));
            });
        }

        [Test]
        public void MovementValidationRejectIfTooLongStride()
        {
            Await(async () => {
                var moveResponse = await MoveUnit(new Vector(2, 0));
                Assert.That(moveResponse.IsSuccess == false);
            });
        }

        [Test]
        public void MovementValidationRejectIfOutSideMap()
        {
            Await(async () => {
                var moveResponse = await MoveUnit(
                    new Vector(-1,0),
                    new Vector(-2,0),
                    new Vector(-3,0),
                    new Vector(-4,0),
                    new Vector(-5,0)
                );
                Assert.That(moveResponse.IsSuccess == false);
            });
        }

        [Test]
        public void MovementValidationRejectIfContinueMovingAfterCrossingShore()
        {
            Await(async () => {
                var moveResponse = await MoveUnit(
                    new Vector(1,0),
                    new Vector(2,0),
                    new Vector(3,0)
                );
                Assert.That(moveResponse.IsSuccess == false);
            });
        }

        [Test]
        public void MovementValidationCanCrossShore()
        {
            Await(async () => {
                var moveResponse = await MoveUnit(new Vector(1,0));
                Assert.That(moveResponse.IsSuccess);
            });
        }
        
        // Testing utility method that creates a server, creates a standardized 9x9
        // map with right-hand side water. Applies movement to the single unit and
        // returns the UnitMoveResponse for validation. 
        public async Task<UnitMoveResponse> MoveUnit(params Vector[] offsets) {
            var server = new GameServer();
            var initialResponse = await server.SendAsync(new GameStateRequest());
            var unit = initialResponse.GameState.Units[0];
            return await server.SendAsync(new UnitMoveRequest{
                GameState = initialResponse.GameState,
                UnitIndex = 0,
                Path = offsets.Select(offset => unit.Pos + offset).ToArray()
            });
        }

        // Utility that allow us to run async code in the non-async [Test] methods
        private static void Await(Func<Task> createTask) {
            Task.Run(async () => await createTask()).GetAwaiter().GetResult();
        }
    }
}
