namespace OpenMind.CRM.Application.Dtos;

public class GoogleEmailDto
{
    public string Id { get; set; } = string.Empty;
    public string ThreadId { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsRead { get; set; }
}

public class GoogleCalendarEventDto
{
    public string Id { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public string Location { get; set; } = string.Empty;
    public List<string> Attendees { get; set; } = new();
}