using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BattleSnakeCS
{
    public class Program
    {
        public static List<BattleSnakeGame> battleSnakeGames = new List<BattleSnakeGame>();
        public static PlayerSnake ProtoypeSnake = new PlayerSnake();

        public static void Main(string[] args)
        {
            ProtoypeSnake.ReadPersonalisationFromFile("personalisation.json");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
