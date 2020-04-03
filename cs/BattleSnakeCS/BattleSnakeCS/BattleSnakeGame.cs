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
        private PlayerSnake mYou = null;

        public BattleSnakeGame(JObject payload)
        {
            mGameID = (string)payload["game"]["id"];
            mTurn = (int)payload["turn"];

            mBoard = new Board(payload);

            mYou = new PlayerSnake(payload); 
        }

        public BattleSnakeGame(JObject payload, PlayerSnake playerSnake)
        {
            mGameID = (string)payload["game"]["id"];
            mTurn = (int)payload["turn"];

            mBoard = new Board(payload);

            mYou = playerSnake;
            playerSnake.InitialisePlayerSnake(payload); 
        }

        public string CompleteTurn(JObject payload)
        {
            // Update the turn number
            mTurn = (int)payload["turn"];

            // Update food coords, opponents, and your snake
            mBoard.UpdateBoard(payload); 
            UpdateYourSnake(payload);

            // Hardcode next move for now
            count += 1;

            if (count % 4 == 1)
            {
                return "{ \"move\":\"left\", " +
                        " \"shout\": \"I am shouting\"}";
            }
            else if (count % 4 == 2)
            {
                return "{ \"move\":\"down\", " +
                        " \"shout\": \"I am shouting\"}";
            }
            else if (count % 4 == 3)
            {
                return "{ \"move\":\"right\", " +
                        " \"shout\": \"I am shouting\"}";
            }
            else
            {
                return "{ \"move\":\"up\", " +
                        " \"shout\": \"I am shouting\"}";
            }
        }

        public void EndGame(JObject payload)
        {
            // Update the turn number
            mTurn = (int)payload["turn"];

            // Update food coords, opponents, and your snake....is it even necessary though?
            mBoard.UpdateBoard(payload); 
            UpdateYourSnake(payload);

            // And then what......
        }

        /// <summary>
        /// Decide on how to make your move!
        /// </summary>
        public void DecideMove()
        {

        }

        private void UpdateYourSnake(JObject payload)
        {
            mYou.DeserialisePlayerSnake(payload); 
        }
    }
}
