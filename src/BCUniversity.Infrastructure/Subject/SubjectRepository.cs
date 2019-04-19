using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCUniversity.Domain.SubjectAggregate;
using BCUniversity.Infrastructure.Common;
using BCUniversity.Infrastructure.DataModel;
using BCUniversity.Infrastructure.DataModel.Relationships;
using BCUniversity.Service.Subject;
using Microsoft.EntityFrameworkCore;

namespace BCUniversity.Infrastructure.Subject
{
    public class SubjectRepository: RepositoryBase<Domain.SubjectAggregate.Subject>, ISubjectRepository
    {
        public SubjectRepository(UniversityContext dbContext) : base(dbContext)
        {
        }

        public override async Task Save(Domain.SubjectAggregate.Subject subject)
        {
            SubjectDataModel subjectDataModel;
            if (!string.IsNullOrWhiteSpace(subject.Id))
            {
                subjectDataModel = await _dbContext.Subjects.SingleOrDefaultAsync(x => x.Id == subject.Id);
            }
            else
            {
                subjectDataModel = new SubjectDataModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = subject.Name
                };
            }

            var lecturesDataModelById = subjectDataModel.Lectures.ToDictionary(x => x.Id);

            var lectures = GetLectureDataModels(subject, lecturesDataModelById);

            subjectDataModel.Lectures = lectures;
            subjectDataModel.StudentLinks = subject.StudentEnrolments.Select(x => new SubjectStudentLink()
            {
                StudentId = x.StudentId,
                SubjectId = subject.Id
            }).ToList();

            _dbContext.Update(subjectDataModel);

            await _dbContext.SaveChangesAsync();
        }

        public override async Task<Domain.SubjectAggregate.Subject> GetById(string id)
        {
            var dataModel = await _dbContext.Subjects.SingleOrDefaultAsync(x => x.Id == id);

            if (dataModel == null)
            {
                return null;
            }
            
            var lectures = dataModel.Lectures.Select(l =>
            {
                var lectureSchedules = l.LectureTheatreLinks.Select(x =>
                    new LectureSchedule(new TheatreReference(x.TheatreId, x.Theatre.Name, x.Theatre.Capacity),
                        x.DayOfWeek, x.StartHour, x.EndHour)).ToList();
                
                return new Lecture(l.Name, lectureSchedules);
            }).ToList();

            var studentEnrolments =
                dataModel.StudentLinks.Select(x => new StudentEnrolment(x.Student.Id, x.Student.Name)).ToList();

            var subject = new Domain.SubjectAggregate.Subject(dataModel.Name, lectures, studentEnrolments);

            return subject;
        }
        
        private static List<LectureDataModel> GetLectureDataModels(
            Domain.SubjectAggregate.Subject subject,
            Dictionary<string, LectureDataModel> lecturesDataModelById
        )
        {
            var lectures = subject.Lectures.Select(l =>
            {
                LectureDataModel lectureDataModel;
                if (!string.IsNullOrWhiteSpace(l.Id) && lecturesDataModelById.ContainsKey(l.Id))
                {
                    lectureDataModel = lecturesDataModelById[l.Id];
                }
                else
                {
                    lectureDataModel = new LectureDataModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = l.Name
                    };
                }

                lectureDataModel.LectureTheatreLinks = l.LectureSchedules.Select(ls => new LectureTheatreLink()
                {
                    DayOfWeek = ls.DayOfWeek,
                    StartHour = ls.StartHour,
                    EndHour = ls.EndHour,
                    LectureId = l.Id,
                    TheatreId = ls.Theatre.TheatreId
                }).ToList();

                return lectureDataModel;
            }).ToList();
            return lectures;
        }
    }
}