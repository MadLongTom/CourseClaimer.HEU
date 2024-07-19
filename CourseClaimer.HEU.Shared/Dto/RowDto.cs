using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseClaimer.HEU.Shared.Dto
{
    public class RowDto
    {
        [DisplayName("课程号")]
        public string KCH { get; set; }
        [DisplayName("课程名")]
        public string KCM { get; set; }
        [DisplayName("类别")]
        public string XGXKLB { get; set; }
    }
}
