using System;
using Interpreter;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var model = ShitHelper.Model;
            var consumer = new CommonMessageHandler(model);
            ShitHelper.Handler = new ServerHandler();
            model.BasicConsume(StepNames.OrderAccepted, false, String.Empty, false, false, null, consumer);
            model.BasicConsume(StepNames.OrderForImplementation, false, String.Empty, false, false, null, consumer);
            model.BasicConsume(StepNames.OrderMerged, false, String.Empty, false, false, null, consumer);
            model.BasicConsume(StepNames.OrderSucces, false, String.Empty, false, false, null, consumer);
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
