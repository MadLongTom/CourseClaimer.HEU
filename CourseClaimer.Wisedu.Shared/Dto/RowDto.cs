using System.ComponentModel;

#pragma warning  disable CS8618
namespace CourseClaimer.Wisedu.Shared.Dto
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
