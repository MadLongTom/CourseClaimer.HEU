using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseClaimer.HEU.Shared.Dto;
using CourseClaimer.HEU.Shared.Models.Runtime;

namespace CourseClaimer.HEU.Shared.Extensions
{
    public static class ProgramExtensions
    {
        public static List<RowDto> AllRows { get; set; } = [];
        public static List<Entity> Entities { get; set; } = [];
    }
}
