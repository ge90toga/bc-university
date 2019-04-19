using BCUniversity.Domain.StudentAggregate;
using BCUniversity.Infrastructure.DataModel;
using BCUniversity.Infrastructure.DataModel.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace BCUniversity.Infrastructure.Common
{
    public class UniversityContext: DbContext
    {
        private readonly string _connectionString;
        public DbSet<SubjectDataModel> Subjects { get; set; }
        public DbSet<LectureDataModel> Lectures { get; set; }
        public DbSet<LectureScheduleDataModel> LectureSchedules { get; set; }
        public DbSet<TheatreDataModel> Theatres { get; set; }
        public DbSet<StudentDataModel> Students { get; set; }
        
        public UniversityContext(IOptions<DbSettings> settings)
        {
            _connectionString = settings.Value.ConnectionString;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubjectStudentLink>()
                .HasKey(x => new { x.SubjectId, x.StudentId });  
            modelBuilder.Entity<SubjectStudentLink>()
                .HasOne(x => x.Subject)
                .WithMany(x => x.StudentLinks)
                .HasForeignKey(x => x.SubjectId);  
            modelBuilder.Entity<SubjectStudentLink>()
                .HasOne(x => x.Student)
                .WithMany(x => x.SubjectLinks)
                .HasForeignKey(x => x.StudentId);

            modelBuilder.Entity<LectureScheduleDataModel>()
                .HasOne(x => x.Lecture)
                .WithMany(x => x.LectureSchedules)
                .HasForeignKey(x => x.LectureId);
            
            modelBuilder.Entity<LectureScheduleDataModel>()
                .HasOne(x => x.Theatre)
                .WithMany(x => x.LectureSchedules)
                .HasForeignKey(x => x.TheatreId);
        }
    }
}