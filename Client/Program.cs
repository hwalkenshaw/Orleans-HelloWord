using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        public static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    await DoCLientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nException while trying to run client: {ex.Message}");
                Console.WriteLine("Make sure the silo that the client is trying to connect to is running");
                Console.WriteLine("Press any key to exit");

                Console.ReadKey();
                return 1;
            }
        }
        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;

            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");

            return client;

        }

        private async static Task DoCLientWork(IClusterClient client)
        {
            //Example of calling graions from the initialized client

            var friend = client.GetGrain<IHello>(0);

            var response = await friend.SayHello("Heeeey");

            Console.WriteLine($"\n\n{response}\n\n");
        }

    }
}
