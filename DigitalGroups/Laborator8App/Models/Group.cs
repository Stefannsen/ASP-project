using Laborator8App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Laborator5App.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele grupului este obligatoriu")]
        [StringLength(100, ErrorMessage = "Numele grupului nu poate avea mai mult de 20 caractere")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descrierea grupului este obligatorie")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<GroupUser> GroupUsers { get; set; }

        public IEnumerable<SelectListItem> Categ { get; set; }
    }
}