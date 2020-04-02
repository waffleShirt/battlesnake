using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics; 

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

            // Read the request body
            string requestBody = null; 
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = reader.ReadToEnd();
            }

            JObject requestBodyJSON = JObject.Parse(requestBody);

            // Try creating a game
            Program.testGame = new BattleSnakeGame(requestBodyJSON); 

            if (Program.testGame != null)
            {
                // Send back the player settings for the game
                ContentResult result = new ContentResult();
                result.StatusCode = 200;
                result.ContentType = "application/json";
                result.Content = Program.testGame.GetSnakePersonalisationContent(); 

                return result;
            }
            else
            {
                ContentResult result = new ContentResult();
                result.StatusCode = 500;
                return result; 
            }
        }

        // Post: BattleSnake/move
        [HttpPost("move")]
        public ContentResult Move()
        {
            Debug.WriteLine("Move called"); 

            // Read the request body
            string requestBody = null;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = reader.ReadToEnd();
            }

            JObject requestBodyJSON = JObject.Parse(requestBody);

            // TODO: Need to find the matching game by the ID received

            // Return the players move
            string nextMove = Program.testGame.CompleteTurn(requestBodyJSON); 

            ContentResult result = new ContentResult();
            result.StatusCode = 200;
            result.ContentType = "application/json";
            result.Content = nextMove; 

            return result;
        }

        // GPOST: BattleSnake/end
        [HttpPost("end")]
        public ContentResult End()
        {
            Debug.WriteLine("End called");

            // Read the request body
            string requestBody = null;
            using (var reader = new StreamReader(Request.Body))
            {
                requestBody = reader.ReadToEnd();
            }

            JObject requestBodyJSON = JObject.Parse(requestBody);

            // TODO: Need to find the matching game by the ID received

            // Return the players move
            Program.testGame.EndGame(requestBodyJSON);

            ContentResult result = new ContentResult();
            result.StatusCode = 200;

            // Doesn't really matter as the server ignores the request
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
    }
}
