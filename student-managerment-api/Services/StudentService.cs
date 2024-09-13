using Microsoft.EntityFrameworkCore;
using my_app.Context;
using my_app.Models;
using my_app.Models.EF;
using Nest;
using System.Linq;
using System.Net;

namespace my_app.Services
{
    public interface IStudentService
    {
        Task<StudentResponse> GetAllAsync(Paging pagesize);
        Task<StudentResponse> GetElasticAllAsync(Paging pagesize);
        Task<Student> GetAsync(int studentID);
        Task<Student> ElasticGetAsync(int studentID);
        Task<Student> CreateAsync(StudentCreate student);
        Task<Student> UpdateAsync(StudentUpdate student);
        Task<HttpStatusCode> DeleteAsync(int studentID);
    }

    public class StudentService : IStudentService
    {
        private readonly myappContext _context;
        private readonly ISenderService _senderService;
        private readonly IElasticClient _elasticClient;

        public StudentService(myappContext context, ISenderService senderService, IElasticClient elasticClient)
        {
            _context = context;
            _senderService = senderService;
            _elasticClient = elasticClient;
        }

        public async Task<Student> CreateAsync(StudentCreate student)
        {
            var std = new Student
            {
                Name = student.Name,
                Address = student.Address,
                Class = student.Class,
                CreatedAt = DateTime.Now,
                Dob = student.Dob
            };

            await _context.Students.AddAsync(std);
            await _context.SaveChangesAsync();
            await _senderService.PutNotification(std, "create");

            return std;
        }

        public async Task<HttpStatusCode> DeleteAsync(int studentID)
        {
            var std = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentID);

            if (std == null)
                return HttpStatusCode.NotFound;

            _context.Students.Remove(std);
            await _context.SaveChangesAsync();
            await _senderService.PutNotification(std, "delete");

            return HttpStatusCode.OK;
        }

        public async Task<StudentResponse> GetAllAsync(Paging pagesize)
        {
            var stds = await _context.Students.Skip(pagesize.PageSize * (pagesize.PageNum - 1)).Take(pagesize.PageSize).ToListAsync();
            var count = await _context.Students.CountAsync();

            return new StudentResponse
            {
                Students = stds,
                TotalStudent = count
            };
        }

        public async Task<StudentResponse> GetElasticAllAsync(Paging pagesize)
        {
            var stds = await _elasticClient.SearchAsync<Student>(s => s
            .Index("students")
            .From((pagesize.PageNum - 1) * pagesize.PageSize)
            .Size(pagesize.PageSize)
            .Query(q => q
                .MatchAll()
                )
            );

            var countResponse = await _elasticClient.CountAsync<Student>(c => c.Index("students"));

            return new StudentResponse
            {
                Students = stds.Documents.ToList(),
                TotalStudent = (int)countResponse.Count
            };
        }

        public async Task<Student> GetAsync(int studentID)
        {
            var std = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentID);

            if (std == null)
                return new Student();

            return std;
        }

        public async Task<Student> ElasticGetAsync(int studentID)
        {
            var searchResponse = await _elasticClient.GetAsync<Student>(studentID, g => g.Index("students"));
            
            if (!searchResponse.IsValid)
                return new Student();

            return searchResponse.Source;
        }

        public async Task<Student> UpdateAsync(StudentUpdate student)
        {
            var std = await _context.Students.FirstOrDefaultAsync(s => s.Id == student.Id);

            if (std == null)
                return new Student();

            std.Id = student.Id;
            std.Name = student.Name;
            std.Address = student.Address;
            std.Class = student.Class;
            std.Dob = student.Dob;
            std.UpdatedAt = DateTime.Now;

            _context.Students.Update(std);
            await _context.SaveChangesAsync();
            await _senderService.PutNotification(std, "update");

            return std;
        }
    }
}
