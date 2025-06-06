namespace LibDtoRemote.Dto;

public class LogEntryDto
{
  public DateTime Timestamp { get; set; }
  public string? Ip { get; set; } 
  public string? Level {get; set; } 
  public string? Message {get; set; } 
  public string? ApplicationName { get; set;} 
  public string? MachineName { get; set; }
  public string? ProcessId { get; set; } 
  public string? Architecture { get; set; }
  public string? Version { get; set; }
  public string? UserName { get; set; } 
  public string? UserDomain { get; set; }
  public string? ThreadId { get; set; }
  public string? Exception { get; set; }
  
  public object? Source {get; set;}
  public string? StackTrace { get; set; }
}
