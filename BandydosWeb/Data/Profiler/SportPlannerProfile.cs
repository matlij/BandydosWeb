using SportPlanner.ModelsDto;
using AutoMapper;

namespace BandydosWeb.Data.Profiler;

public class SportPlannerProfile : Profile
{
    public SportPlannerProfile()
    {
        #region DtoToModel

        CreateMap<EventDto, Event>();

        #endregion DtoToModel
    }
}
