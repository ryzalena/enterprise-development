namespace Domain.Entities;

public class Specialization
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}