using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using Team1.Model;

namespace Team1.Infrastructure.Dtos
{
    public class TaskDto : TaskBase
    {
        [Display(Name ="Due Date")]
        public string DueDate { get { return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DueMonth)} {DueDay.ToString().PadLeft(2, '0')}"; } }
        public string TaskCategoryString { get { return TaskCategoryId.GetDisplayName(); } }

        public string TaskMemberTypesName
        {
            get
            {
                if (TaskMemberTypes == null || TaskMemberTypes.Count == 0)
                    return null;

                return string.Join(", ", TaskMemberTypes.Select(s => s.MemberTypeString).OrderBy(o=>o));
            }
        }

        [Display(Name = "Responsible Member")]
        [Required(ErrorMessage = "Responsible member is required.")]
        public List<TaskMemberTypeDto> TaskMemberTypes { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public bool IsUpdated { get; set; }
    }
}
