using Laborator8App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Laborator5App.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Numele activitatii este obligatoriu")]
        public string ActivityName { get; set; }

        [Required(ErrorMessage = "Descrierea activitatii este obligatorie")]
        [DataType(DataType.MultilineText)]
        public string ActivityDescription { get; set; }

        [Required(ErrorMessage = "Data activitatii este obligatorie")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public int Id { get; set; }
        public virtual Group Group { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }


    }
}