﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootstrapBlazor.Components;

namespace CourseClaimer.HEU.Shared.Models.Database
{
    public class ClaimRecord
    {
        [Key]
        [AutoGenerateColumn(IsVisibleWhenAdd = false,IsVisibleWhenEdit = false)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [DisplayName("用户")]
        public string UserName { get; set; }
        [DisplayName("选课时间")]
        public DateTime ClaimTime { get; set; } = DateTime.Now;
        [DisplayName("课程名")]
        public string Course { get; set; }
        [DisplayName("是否成功")]
        public bool IsSuccess { get; set; }
    }
}
