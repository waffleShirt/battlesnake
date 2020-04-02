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
        private Snake mYou = null;
        
        // Your snake personalisation parameters
        private string mColor = "#ff00ff";
        private string mHeadType = "bendr";
        private string mTailType = "pixel"; 

        public BattleSnakeGame(JObject payload)
        {
            mGameID = (string)payload["game"]["id"];
            mTurn = (int)payload["turn"];

            // Extract board parameters and create board
            int boardHeight = (int)payload["board"]["height"];
            int boardWidth = (int)payload["board"]["width"];

            List<Point> foodCoords = DeserialiseFood(payload);

            List<Snake> opponentSnakes = DeserialiseOpponents(payload); 

            mBoard = new Board(boardHeight, boardWidth, foodCoords, opponentSnakes);

            mYou = DeserialiseYourSnake(payload); 
        }

        public string GetSnakePersonalisationContent()
        {
            // TODO: Hardcoded, needs to use actual vars
            return "{ \"color\":\"#ff00ff\", " +
                "   \"headType\": \"tongue\", " +
                "   \"tailType\": \"fat-rattle\"}";
        }

        public string CompleteTurn(JObject payload)
        {
            // Update the turn number
            mTurn = (int)payload["turn"];

            // Update food coords, opponents, and your snake
            UpdateFood(payload);
            UpdateOpponents(payload);
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

            // Update food coords, opponents, and your snake
            UpdateFood(payload);
            UpdateOpponents(payload);
            UpdateYourSnake(payload);

            // And then what......
        }

        /// <summary>
        /// Decide on how to make your move!
        /// </summary>
        public void DecideMove()
        {

        }

        private void UpdateFood(JObject payload)
        {
            List<Point> newFoodCoords = DeserialiseFood(payload);
            mBoard.UpdateFood(newFoodCoords); 
        }

        private void UpdateOpponents(JObject payload)
        {
            List<Snake> opponents = DeserialiseOpponents(payload);
            mBoard.UpdateOpponents(opponents); 
        }

        private void UpdateYourSnake(JObject payload)
        {
            mYou = null;
            mYou = DeserialiseYourSnake(payload); 
        }

        private List<Point> DeserialiseFood(JObject payload)
        {
            List<Point> foodCoords = new List<Point>();
            JArray foodCoordsJSON = (JArray)payload["board"]["food"];
            foreach (var coord in foodCoordsJSON.Children())
            {
                int x = (int)coord["x"];
                int y = (int)coord["y"];
                foodCoords.Add(new Point(x, y));
            }

            return foodCoords; 
        }

        private List<Snake> DeserialiseOpponents(JObject payload)
        {
            List<Snake> opponentSnakes = new List<Snake>();
            JArray opponentSnakesJSON = (JArray)payload["board"]["snakes"];
            foreach (var snake in opponentSnakesJSON.Children())
            {
                string id = (string)snake["id"];
                string name = (string)snake["name"];
                int health = (int)snake["health"];

                List<Point> body = new List<Point>();
                JArray bodyJSON = (JArray)snake["body"];
                foreach (var bodyCoord in bodyJSON.Children())
                {
                    int x = (int)bodyCoord["x"];
                    int y = (int)bodyCoord["y"];
                    body.Add(new Point(x, y));
                }

                string shout = (string)snake["shout"];

                opponentSnakes.Add(new Snake(id, name, health, body, shout));
            }

            return opponentSnakes; 
        }

        private Snake DeserialiseYourSnake(JObject payload)
        {
            string yourSnakeID = (string)payload["you"]["id"];
            string yourSnakeName = (string)payload["you"]["name"];
            int yourSnakeHealth = (int)payload["you"]["health"];

            List<Point> yourSnakeBody = new List<Point>();
            JArray yourSnakeBodyJSON = (JArray)payload["you"]["body"];
            foreach (var bodyCoord in yourSnakeBodyJSON.Children())
            {
                int x = (int)bodyCoord["x"];
                int y = (int)bodyCoord["y"];
                yourSnakeBody.Add(new Point(x, y));
            }

            string yourSnakeShout = (string)payload["you"]["shout"];
            return new Snake(yourSnakeID, yourSnakeName, yourSnakeHealth, yourSnakeBody, yourSnakeShout);
        }

        private class Board
        {
            private int mBoardHeight = 1;
            private int mBoardWidth = 1;
            private List<Point> mFood = null;
            private List<Snake> mOpponents = null;

            public Board(int boardHeight, int boardWidth, List<Point> food, List<Snake> opponents)
            {
                mBoardHeight = boardHeight;
                mBoardWidth = boardWidth;
                mFood = food;
                mOpponents = opponents; 
            }

            public void SetBoardSize(int height, int width)
            {
                mBoardHeight = height;
                mBoardWidth = width; 
            }

            public void UpdateFood(List<Point> food)
            {
                mFood = null;
                mFood = food; 
            }

            public void UpdateOpponents(List<Snake> opponents)
            {
                mOpponents = opponents; 
            }
        }

        private class Snake
        {
            private string mID;
            private string mName; 
            private int mHealth = 0;
            private List<Point> mBody = null;
            private string mShout; 

            public Snake(string id, string name, int health, List<Point> body, string shout)
            {
                mID = id;
                mName = name;
                mHealth = health;
                mBody = body;
                mShout = shout; 
            }
        }

    }
}
