using CourseClaimer.Wisedu.Shared.Dto;
using CourseClaimer.Wisedu.Shared.Models.Runtime;

namespace CourseClaimer.Wisedu.Shared.Extensions
{
    public static class ProgramExtensions
    {
        public static List<RowDto> AllRows { get; set; } = [];
        public static List<Entity> Entities { get; set; } = [];
    }
}
