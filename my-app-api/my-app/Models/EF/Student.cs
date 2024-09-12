using System;
using System.Collections.Generic;

namespace my_app.Models.EF
{
    public partial class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Dob { get; set; }
        public string? Address { get; set; }
        public string? Class { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
