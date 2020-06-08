using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Team1.Model
{
    public class TaskCategoryBase
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public Enums.TaskCategoryEnum TaskCategoryId { get; set; }

        [StringLength(Constants.FieldSizes.NameLength)]
        public string Name { get; set; }
    }

    public class TaskCategory : TaskCategoryBase
    {
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
