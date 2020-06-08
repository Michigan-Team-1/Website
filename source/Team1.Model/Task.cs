using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Team1.Model.Constants;

namespace Team1.Model
{
    public class TaskBase
    {
        [Key]
        public int TaskId { get; set; }

        [Display(Name ="Category")]
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public Enums.TaskCategoryEnum TaskCategoryId { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = ErrorMessages.RequiredField), StringLength(Constants.FieldSizes.DescriptionLength, ErrorMessage = ErrorMessages.StringLengthMax)]
        public string Description { get; set; }

        [Display(Name = "Due Month")]
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public byte DueMonth { get; set; }

        [Display(Name = "Due Day")]
        [Required(ErrorMessage = ErrorMessages.RequiredField)]
        public byte DueDay { get; set; }
    }

    public class Task : TaskBase
    {
        public OwnedTypes.AuditFields AuditFields { get; set; }

        public ICollection<TaskMemberType> TaskMemberTypes { get; set; }

        public TaskCategory TaskCategory { get; set; }
    }
}
