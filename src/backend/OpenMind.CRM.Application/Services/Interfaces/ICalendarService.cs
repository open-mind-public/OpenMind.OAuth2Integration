using OpenMind.CRM.Application.Dtos;

namespace OpenMind.CRM.Application.Services.Interfaces;

public interface ICalendarService
{
    string ProviderName { get; }
    Task<List<CalendarEventDto>> GetCalendarEventsAsync(int userId, DateTime? timeMin = null, DateTime? timeMax = null, int maxResults = 50);
    Task<CalendarEventDto?> GetEventByIdAsync(int userId, string eventId);
    Task<CalendarEventDto> CreateEventAsync(int userId, CalendarEventDto eventDto);
    Task<CalendarEventDto> UpdateEventAsync(int userId, string eventId, CalendarEventDto eventDto);
    Task<bool> DeleteEventAsync(int userId, string eventId);
}