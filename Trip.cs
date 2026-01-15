public class Trip
{
    public DateTime StartedAt { get; set; }
    public DateTime EndedAt { get; set; }
    public string StartStation { get; set; }
    public string EndStation { get; set; }
    public string UserType { get; set; } // member / casual
}
