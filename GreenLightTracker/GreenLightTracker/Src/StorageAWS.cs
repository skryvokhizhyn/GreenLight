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
            string jsonString = JsonSerializer.Serialize(path);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("http://yourUrl", new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Bad response" + response.Content);
            }
        }

        public long GetGpsLocationCount()
        {
            return 0;
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