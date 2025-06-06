namespace LibDtoRemote.Dto.ClientMine;

public class MiningStatsDto
{
    public string? HashMining { get; set; }
    public double HashRate { get; set; }
    public int AcceptedShares { get; set; }
    public int RejectedShares { get; set; }
    public string? Status { get; set; }
    public string? CoinType { get; set; }
    public double PowerConsumption { get; set; }
}
