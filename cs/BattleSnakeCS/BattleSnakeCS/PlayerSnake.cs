using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace BattleSnakeCS
{
    public class PlayerSnake : Snake
    {
        public struct Personalisation
        {
            public string mColor;
            public string mHeadType;
            public string mTailType;

            public Personalisation(string color, string headType, string tailType)
            {
                mColor = color;  //"#ff00ff";
                mHeadType = headType;  //"bendr";
                mTailType = tailType;  //"pixel";
            }
        }

        private Personalisation mPersonalisation; 

        public PlayerSnake()
        {
            // Defaults for personalisation
            mPersonalisation = new Personalisation("#ff00ff", "bendr", "pixel"); 
        }

        public PlayerSnake(PlayerSnake other, JObject payload)
        {
            InitialisePlayerSnake(payload);
            mPersonalisation = other.GetPersonalisation(); 
        }

        public Personalisation GetPersonalisation()
        {
            return mPersonalisation; 
        }

        public void SetPersonalisation(JObject payload)
        {
            mPersonalisation.mColor = (string)payload["color"];
            mPersonalisation.mHeadType = (string)payload["headType"];
            mPersonalisation.mTailType = (string)payload["tailType"];
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

        public string GetSnakePersonalisationJSON()
        {
            JObject personalisationContentJSON =
                new JObject(
                    new JProperty("color", mPersonalisation.mColor),
                    new JProperty("headType", mPersonalisation.mHeadType),
                    new JProperty("tailType", mPersonalisation.mTailType));

            return personalisationContentJSON.ToString(Newtonsoft.Json.Formatting.None); 
        }

        public void WritePersonalisationToFile(string filename)
        {
            string path = Path.Combine(Environment.CurrentDirectory, filename);
            File.WriteAllText(path, GetSnakePersonalisationJSON());
        }

        public void ReadPersonalisationFromFile(string filename)
        {
            string path = Path.Combine(Environment.CurrentDirectory, filename);
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                SetPersonalisation(JObject.Parse(json));
            }
        }
    }
}
