namespace Postech.Payments.Api.Configuration;

public class DatabaseOptions
{
    public const string SectionName = "ConnectionStrings";
    
    public string DefaultConnection { get; set; } = string.Empty;
}