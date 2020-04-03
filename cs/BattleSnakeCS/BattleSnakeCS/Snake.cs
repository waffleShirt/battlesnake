using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json.Linq;

namespace BattleSnakeCS
{
    public class Snake
    {
        protected string mID;
        protected string mName;
        protected int mHealth = 0;
        protected List<Point> mBody = null;
        protected string mShout;

        public Snake()
        {
            mID = null;
            mName = null;
            mHealth = 0;
            mBody = null;
            mShout = null;
        }

        public Snake(string id, string name, int health, List<Point> body, string shout)
        {
            mID = id;
            mName = name;
            mHealth = health;
            mBody = body;
            mShout = shout;
        }

        public void DeserialiseSnake(JObject payload)
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
        }
    }
}
