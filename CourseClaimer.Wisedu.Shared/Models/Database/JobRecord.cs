using System.ComponentModel.DataAnnotations;

namespace CourseClaimer.Wisedu.Shared.Models.Database;

public class JobRecord
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime ExecuteTime { get; set; }=DateTime.Now;
    public string JobName { get; set; }
    public string Message { get; set; }
}