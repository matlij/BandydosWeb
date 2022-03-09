using SportPlanner.ModelsDto;
using SportPlanner.ModelsDto.Enums;
using System.Diagnostics;
using AutoMapper;
using System.Text.Json;

namespace BandydosWeb.Data;

public class EventService
{
    private static readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly ILogger<EventService> _logger;

    static EventService()
    {
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(BandydosApiConstants.BaseUri)
        };
        _httpClient.DefaultRequestHeaders.Add("x-functions-key", "OlEfvbvK6YHrBpQa8tIycbz49AR/mgasjZryR3f96TdyXb/ZsEWwaA==");
    }

    public EventService(IMapper mapper, ILogger<EventService> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<Event>> GetEventsAsync(bool sortedByDate = true)
    {
        var uri = new UriBuilder(BandydosApiConstants.BaseUri)
        {
            Path = BandydosApiConstants.EventUri,
        };

        var response = await _httpClient.GetAsync(uri.Uri);
        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"Failed to get events. Status code: {response.StatusCode}");
            return new List<Event>();
        }

        var events = await response.Content.ReadFromJsonAsync<IEnumerable<Event>>() ?? new List<Event>();
        return _mapper.Map<IEnumerable<Event>>(events.OrderBy(e => e.Date));
    }

    public async Task<bool> UpdateEventsAsync(Event @event)
    {
        var uri = new UriBuilder(BandydosApiConstants.BaseUri)
        {
            Path = BandydosApiConstants.EventUri,
        };

        var response = await _httpClient.PutAsJsonAsync(uri.Uri, @event);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Failed to update event with Date: " + @event.Date);
        }
        return true;
    }

    public async Task<bool> UpdateEventUserAsync(Guid eventId, EventUserDto eventUser)
    {
        var uri = new UriBuilder(BandydosApiConstants.BaseUri)
        {
            Path = $"{BandydosApiConstants.EventUri}/{eventId}/eventuser",
        };

        var response = await _httpClient.PutAsync(uri.Uri, new StringContent(JsonSerializer.Serialize(eventUser)));
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to update eventuser {eventUser.UserName} for event date {eventId}");
            return false;
        }
        return true;
    }
}

public class Event
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public int Attending
    {
        get
        {
            return Users.Count(u => u.UserReply == UserReply.Going);
        }
    }
    public int Invited
    {
        get
        {
            return Users.Count();
        }
    }

    public bool UserIsAttending
    {
        get
        {
            return User.UserReply == UserReply.Going;
        }
        set
        {
            User.UserReply = value ? UserReply.Going : UserReply.NotGoing;
        }
    }

    public EventUserDto User
    {
        get
        {
            return Users.Single(u => u.UserName == BandydosApiConstants.UserName);
        }
    }

    public IEnumerable<EventUserDto> Users { get; set; } = new List<EventUserDto>();
    public EventType EventType { get; set; }
    public AddressDto Address { get; set; }

}