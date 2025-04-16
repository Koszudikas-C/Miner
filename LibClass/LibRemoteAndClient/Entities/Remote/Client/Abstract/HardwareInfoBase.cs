namespace LibRemoteAndClient.Entities.Remote.Client.Abstract;

public abstract class HardwareInfoBase
{
    public string Name { get; set; } = "Unknown";
    public string Status { get; set; } = "Unknown";
    public double UsagePercentage { get; set; } = 0;
    public double TotalCapacity { get; set; } = 0;
}
