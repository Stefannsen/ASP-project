using Laborator5App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Laborator8App.Models
{
    public class GroupUser
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Key, Column(Order = 1)]
        public string ApplicationUsers_Id { get; set; }


        public virtual Group Group { get; set; }
        public virtual ApplicationUser ApplicationUsers { get; set; }

        public string GroupRoleId { get; set; }
        public virtual GroupRole GroupRole { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

    }


}