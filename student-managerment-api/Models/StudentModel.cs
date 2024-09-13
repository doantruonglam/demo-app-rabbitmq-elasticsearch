using my_app.Models.EF;

namespace my_app.Models
{
    public class StudentResponse
    {
        public List<Student> Students { get; set; }
        public int TotalStudent { get; set; }
    }

    public class StudentCreate
    {
        public string? Name { get; set; }
        public DateTime? Dob { get; set; }
        public string? Address { get; set; }
        public string? Class { get; set; }
    }

    public class StudentUpdate : StudentCreate
    {
        public int Id { get; set; }
    }

    public class Paging
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; }
    }

}
