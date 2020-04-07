//using System;
//using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using System;

namespace BattleSnakeCS.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BattleSnakeController : ControllerBase
    {
        // GET: api/BattleSnake
        [HttpGet]
        public string Get()
        {
            return "Started Toms Shitty battlesnake game";
        }

        // POST: BattleSnake/start
        [HttpPost("start")]
        public ContentResult Start()
        {
            Debug.WriteLine("Start called");

            JObject reqBody = ParseRequestBody(Request);

            // Create a new game
            BattleSnakeGame newGame = new BattleSnakeGame(reqBody);
            Program.battleSnakeGames.Add(newGame);

            ContentResult result = new ContentResult();
            result.StatusCode = 200;
            result.ContentType = "application/json";
            result.Content = newGame.GetPlayerSnake().GetSnakePersonalisationJSON();

            return result;
        }

        // Post: BattleSnake/move
        [HttpPost("move")]
        public ContentResult Move()
        {
            Debug.WriteLine("Move called");

            JObject reqBody = ParseRequestBody(Request);

            // Find the matching game by the ID received and update it
            ContentResult result = new ContentResult();
            result.StatusCode = 500;

            string gameID = (string)reqBody["game"]["id"];

            foreach (BattleSnakeGame game in Program.battleSnakeGames)
            {
                if (game.GetGameID() == gameID)
                {
                    // Return the players move
                    string nextMove = game.CompleteTurn(reqBody);

                    result.StatusCode = 200;
                    result.ContentType = "application/json";
                    result.Content = nextMove;

                    break;
                }
            }

            return result;
        }

        // GPOST: BattleSnake/end
        [HttpPost("end")]
        public ContentResult End()
        {
            Debug.WriteLine("End called");

            JObject reqBody = ParseRequestBody(Request);

            // Find the matching game by the ID received and delete it
            string gameID = (string)reqBody["game"]["id"];
            BattleSnakeGame gameToDelete = null;

            foreach (BattleSnakeGame game in Program.battleSnakeGames)
            {
                if (game.GetGameID() == gameID)
                {
                    gameToDelete = game;
                    break;
                }
            }

            if (gameToDelete != null)
            {
                Program.battleSnakeGames.Remove(gameToDelete);
            }
            else
            {
                // TODO: We have a fail
            }

            // Doesn't really matter as the server ignores the request
            ContentResult result = new ContentResult();
            result.StatusCode = 200;
            return result;
        }

        // POST: BattleSnake/ping
        [HttpPost("ping")]
        public ContentResult Ping()
        {
            // Must return 200. Content is for debug purposes. 
            ContentResult result = new ContentResult();
            result.StatusCode = 200;
            result.ContentType = "application/json";
            result.Content = "{ \"ping result\":\"success\"}";

            return result;
        }

        // POST: BattleSnake/setsnakeparams
        [HttpPost("setsnakeparams")]
        public ContentResult SetSnakeParams()
        {
            Debug.WriteLine("Set snake params called");

            JObject reqBody = ParseRequestBody(Request);

            BattleSnakeGame.ProtoypeSnake.SetPersonalisation(reqBody);
            WritePersonalisationToFile(); 

            // Content is for debug purposes. 
            ContentResult result = new ContentResult();
            result.StatusCode = 200;
            result.ContentType = "application/json";
            result.Content = "{ \"Set snake params\":\"Success\"}";

            return result;
        }

        // GET: BattleSnake/getsnakeparams
        [HttpGet("getsnakeparams")]
        public ContentResult GetSnakeParams()
        {
            // Content is for debug purposes. 
            ContentResult result = new ContentResult();
            result.StatusCode = 200;
            result.ContentType = "application/json";
            result.Content = BattleSnakeGame.ProtoypeSnake.GetSnakePersonalisationJSON();

            return result;
        }

        private JObject ParseRequestBody(HttpRequest req)
        {
            // Read the request body
            string requestBody = null;
            using (var reader = new StreamReader(req.Body))
            {
                requestBody = reader.ReadToEnd();
            }

            return JObject.Parse(requestBody);
        }

        private void WritePersonalisationToFile()
        {
            string json = BattleSnakeGame.ProtoypeSnake.GetSnakePersonalisationJSON(); 
            string path = Environment.CurrentDirectory;
            System.IO.File.WriteAllText(Path.Combine(path, "test.json"), json);
        }

        private void ReadPersonalisationFromFile()
        {
            string path = Environment.CurrentDirectory;
            using (StreamReader r = new StreamReader(Path.Combine(path, "test.json")))
            {
                string json = r.ReadToEnd();
                BattleSnakeGame.ProtoypeSnake.SetPersonalisation(JObject.Parse(json));
            }
        }
    }
}
