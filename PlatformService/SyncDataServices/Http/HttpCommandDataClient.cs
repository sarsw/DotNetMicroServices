using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _cfg;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration cfg)
        {
            _httpClient = httpClient;
            _cfg = cfg;
        }
        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            // create a payload
            var httpContent = new StringContent(JsonSerializer.Serialize(plat), Encoding.UTF8, "application/json");

            // post async request
            var response = await _httpClient.PostAsync($"{_cfg["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Sync posted to cmd service OK");
            }
            else
            {
                Console.WriteLine("Sync posted to cmd service FAILED!!");
            }
        }
    }
}