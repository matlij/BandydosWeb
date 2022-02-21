using SportPlanner.ModelsDto;
using System.Diagnostics;

namespace BandydosWeb.Data
{
    public class EventService
    {
        private static readonly HttpClient _httpClient;

        static EventService()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(BandydosApiConstants.BaseUri)
            };
            _httpClient.DefaultRequestHeaders.Add("x-functions-key", "OlEfvbvK6YHrBpQa8tIycbz49AR/mgasjZryR3f96TdyXb/ZsEWwaA==");
        }

        public async Task<IEnumerable<EventDto>> GetEventsAsync()
        {
            var uri = new UriBuilder(BandydosApiConstants.BaseUri)
            {
                Path = BandydosApiConstants.EventUri,
            };

            var response = await _httpClient.GetAsync(uri.Uri);
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Failed to get events. Status code: {response.StatusCode}");
                return new List<EventDto>();
            }

            return await response.Content.ReadFromJsonAsync<IEnumerable<EventDto>>() ?? new List<EventDto>();
        }
    }
}