using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing; 

namespace BattleSnakeCS
{
    public class PlayerSnake : Snake
    {
        private string mColor = "#ff00ff";
        private string mHeadType = "bendr";
        private string mTailType = "pixel";

        public PlayerSnake()
        {

        }

        public PlayerSnake(JObject payload)
        {
            InitialisePlayerSnake(payload);  
        }

        public void SetPersonalisation(JObject payload)
        {
            mColor = (string)payload["color"];
            mHeadType = (string)payload["head-type"];
            mTailType = (string)payload["tail-type"];
        }

        public void InitialisePlayerSnake(JObject payload)
        {
            mBody = new List<Point>();

            mID = (string)payload["you"]["id"];
            mName = (string)payload["you"]["name"];
            mHealth = (int)payload["you"]["health"];

            DeserialisePlayerSnake(payload);
        }

        public void DeserialisePlayerSnake(JObject payload)
        {
            // Update health
            mHealth = (int)payload["you"]["health"];

            // Erase old body and update with new coords
            mBody.Clear(); 

            JArray snakeBodyJSON = (JArray)payload["you"]["body"];
            foreach (var bodyCoord in snakeBodyJSON.Children())
            {
                int x = (int)bodyCoord["x"];
                int y = (int)bodyCoord["y"];
                mBody.Add(new Point(x, y));
            }
        }

        public string GetSnakePersonalisationContent()
        {
            JObject personalisationContentJSON =
                new JObject(
                    new JProperty("color", mColor),
                    new JProperty("headType", mHeadType),
                    new JProperty("tailType", mTailType));

            return personalisationContentJSON.ToString(Newtonsoft.Json.Formatting.None); 
        }
    }
}
