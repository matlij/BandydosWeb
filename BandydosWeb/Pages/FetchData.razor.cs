using BandydosWeb.Data;
using System.Linq;

namespace BandydosWeb.Pages;

public partial class FetchData
{
    private IEnumerable<Event> events = new List<Event>();
    private bool isBusy;

    private async Task SwitchUserAttending(Event @event)
    {
        isBusy = true;
        @event.UserIsAttending = !@event.UserIsAttending;
        var result = await EventService.UpdateEventUserAsync(@event.Id, @event.User);
        if (!result)
        {
            events = await EventService.GetEventsAsync();
        }
        else
        {
            events = events.Select(e => e.Id == @event.Id ? @event : e);
        }
        isBusy = false;
    }

    protected override async Task OnInitializedAsync()
    {
        events = await EventService.GetEventsAsync();
    }
}
