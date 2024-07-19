using BootstrapBlazor.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseClaimer.HEU.Shared.Models.Database
{
    public class Customer
    {
        [Key]
        [AutoGenerateColumn(IsVisibleWhenAdd = false, IsVisibleWhenEdit = false)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [DisplayName("用户名")]
        public string UserName { get; set; }
        [DisplayName("密码")]
        public string Password { get; set; }
        [DisplayName("类别")]
        public string Categories { get; set; } = string.Empty;
        [DisplayName("课程")]
        public string Course { get; set; } = string.Empty;
        [DisplayName("是否完成")]
        public bool IsFinished { get; set; }
    }
}
