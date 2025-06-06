namespace LibDtoClient.Dto.ClientMine.Abstract;

public class HardwareInfoBaseDto
{
    public string? Name { get; set; }
    public string? Status { get; set; }
    public double UsagePercentage { get; set; } 
    public double TotalCapacity { get; set; }
}
