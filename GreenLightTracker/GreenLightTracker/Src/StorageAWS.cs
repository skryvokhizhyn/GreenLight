using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System;

namespace GreenLightTracker.Src
{
    class StorageAWS : StorageInterface
    {
        public async void Store(Guid uuid, ICollection<GpsLocation> path)
        {
            var pathDto = new PathAWSDTO
            {
                route_id = uuid.ToString(),
                ts = Java.Lang.JavaSystem.CurrentTimeMillis().ToString(),
                route = JsonSerializer.Serialize(path)
            };

            string jsonString = JsonSerializer.Serialize<PathAWSDTO>(pathDto);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://2qf6oc5nc2.execute-api.eu-central-1.amazonaws.com/prod/collect", 
                    new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Bad response" + response.Content);
            }
        }

        public ICollection<GpsLocation> GetAllGpsLocations()
        {
            return null;
        }

        public void Close()
        {

        }
    }
}