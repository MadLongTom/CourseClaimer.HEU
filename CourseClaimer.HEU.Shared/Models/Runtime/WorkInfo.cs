using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseClaimer.HEU.Shared.Models.Runtime
{
    public class WorkInfo
    {
        public Entity Entity { get; set; }
        public Task task { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; } = new();
    }
}
