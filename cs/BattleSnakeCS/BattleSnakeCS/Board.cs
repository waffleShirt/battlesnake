using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json.Linq;

namespace BattleSnakeCS
{
    public class Board
    {
        private int mBoardHeight = 1;
        private int mBoardWidth = 1;
        private List<Point> mFood = null;
        private List<Snake> mOpponents = null;

        public Board(JObject payload)
        {
            mFood = new List<Point>();
            mOpponents = new List<Snake>(); 

            mBoardHeight = (int)payload["board"]["height"];
            mBoardWidth = (int)payload["board"]["width"];

            DeserialiseFood(payload);
            DeserialiseOpponents(payload); 
        }

        public void UpdateBoard(JObject payload)
        {
            DeserialiseFood(payload);
            DeserialiseOpponents(payload);
        }

        public void DeserialiseOpponents(JObject payload)
        {
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

                mOpponents.Add(new Snake(id, name, health, body, shout));
            }
        }

        private void DeserialiseFood(JObject payload)
        {
            // Erase old food before updating
            mFood.Clear(); 

            // Get the updated food coords
            JArray foodCoordsJSON = (JArray)payload["board"]["food"];
            foreach (var coord in foodCoordsJSON.Children())
            {
                int x = (int)coord["x"];
                int y = (int)coord["y"];
                mFood.Add(new Point(x, y));
            }
        }

        public Point GetBoardSize()
        {
            return new Point(mBoardWidth, mBoardHeight); 
        }

        public List<Point> GetFood()
        {
            return mFood; 
        }

        public List<Snake> GetOpponents()
        {
            return mOpponents; 
        }
    }
}
