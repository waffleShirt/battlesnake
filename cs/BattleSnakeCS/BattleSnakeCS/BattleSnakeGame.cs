using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BattleSnakeCS
{
    public class BattleSnakeGame
    {
        private int count = 0; 

        // Game Variables
        private string mGameID = null;
        private int mTurn = 0;
        private Board mBoard = null;
        private PlayerSnake mPlayer = null;

        public BattleSnakeGame(JObject payload)
        {
            mGameID = (string)payload["game"]["id"];
            mTurn = (int)payload["turn"];

            mBoard = new Board(payload);

            mPlayer = new PlayerSnake(Program.ProtoypeSnake, payload); 
        }

        public string GetGameID()
        {
            return mGameID; 
        }
            
        public PlayerSnake GetPlayerSnake()
        {
            return mPlayer; 
        }

        public string CompleteTurn(JObject payload)
        {
            // Update the turn number
            mTurn = (int)payload["turn"];

            // Update food coords, opponents, and your snake
            mBoard.UpdateBoard(payload); 
            UpdatePlayerSnake(payload);

            PlayerSnake.Direction moveDirection = mPlayer.GetNextMove(mBoard); 

            switch (moveDirection)
            {
                case PlayerSnake.Direction.Up:
                    return "{ \"move\":\"up\", " +
                            " \"shout\": \"I am shouting\"}";
                case PlayerSnake.Direction.Down:
                    return "{ \"move\":\"down\", " +
                            " \"shout\": \"I am shouting\"}";
                case PlayerSnake.Direction.Left:
                    return "{ \"move\":\"left\", " +
                            " \"shout\": \"I am shouting\"}";
                case PlayerSnake.Direction.Right:
                    return "{ \"move\":\"right\", " +
                            " \"shout\": \"I am shouting\"}";
                default:
                    return "{ \"move\":\"none\", " +
                            " \"shout\": \"An error has occurred\"}";
            }
            //// Hardcode next move for now
            //count += 1;

            //if (count % 4 == 1)
            //{
            //    return "{ \"move\":\"left\", " +
            //            " \"shout\": \"I am shouting\"}";
            //}
            //else if (count % 4 == 2)
            //{
            //    return "{ \"move\":\"down\", " +
            //            " \"shout\": \"I am shouting\"}";
            //}
            //else if (count % 4 == 3)
            //{
            //    return "{ \"move\":\"right\", " +
            //            " \"shout\": \"I am shouting\"}";
            //}
            //else
            //{
            //    return "{ \"move\":\"up\", " +
            //            " \"shout\": \"I am shouting\"}";
            //}
        }

        public void EndGame(JObject payload)
        {
            // TODO: Do we even need to do any teardown on the game when it's done?
            // Update the turn number
            mTurn = (int)payload["turn"];

            // Update food coords, opponents, and your snake....is it even necessary though?
            mBoard.UpdateBoard(payload); 
            UpdatePlayerSnake(payload);

            // And then what......
        }

        /// <summary>
        /// Decide on how to make your move!
        /// </summary>
        public void DecideMove()
        {

        }

        private void UpdatePlayerSnake(JObject payload)
        {
            mPlayer.DeserialisePlayerSnake(payload); 
        }
    }
}
