using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibEntitiesRemote.Entities.Client.Enum;
using Microsoft.EntityFrameworkCore;

namespace LibEntitiesRemote.Entities.Client;

[Index(nameof(Ip), IsUnique = true)]
public class Client(string ip, string ipLocal, string port)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [MaxLength(45)]
    public string Ip { get; init; } = ip;

    [MaxLength(15)]
    public string IpLocal { get; set; } = ipLocal;

    [MaxLength(5)]
    public string Port { get; set; } = port;

    public int AttemptsConnection { get; set; }
    
    [EnumDataType(typeof(ConnectionStates))]
    public ConnectionStates StateClient { get; set; }

    [MaxLength(8)] 
    public int? TimeoutReceive { get; set; }

    [MaxLength(8)]
    public int? TimeoutSend { get; set; }

    public DateTimeOffset PrimaryConnection { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset DateConnected { get; set; } = DateTimeOffset.UtcNow;
    
}