using SQLite;

namespace Assignment.Models;

public class BarCode
{
    [PrimaryKey, AutoIncrement]
    public int Id {get; set;}
    [NotNull]
    public string? Code {get; set;}
}