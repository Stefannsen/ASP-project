using Laborator8App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Laborator8App.Models
{
    public class GroupRole
    {
        [Key]
        public string GroupRoleId { get; set; }
        public string GroupRoleName { get; set; }
        public virtual ICollection<GroupUser> GroupUsers { get; set; }
    }
}