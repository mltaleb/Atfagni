using SQLite;
namespace Atfagni.Mobile.Models.Local;

public class LocalCity
{
    [PrimaryKey] public int Id { get; set; }
    public string Name { get; set; }
}