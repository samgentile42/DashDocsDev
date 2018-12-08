using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DashDocs.Models
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}