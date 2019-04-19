using System;
using System.Collections.Generic;
using BCUniversity.Domain.Common;

namespace BCUniversity.Domain.SubjectAggregate
{
    public class LectureSchedule: ValueObject
    {        
        public int DayOfWeek { get; private set; }
        
        public int StartHour { get; private set; }
        public int EndHour { get; private set; }
        
        public TheatreReference Theatre { get; private set; }

        public LectureSchedule(TheatreReference theatre, int dayOfWeek,
            int startHour, int endHour)
        {
            Theatre = theatre;
            DayOfWeek = dayOfWeek;
            StartHour = startHour;
            EndHour = endHour;
        }
    }
}