using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BITCollege_DP.Data
{
    public class BITCollege_DPContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public BITCollege_DPContext() : base("name=BITCollege_DPContext")
        {
        }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.Student> Students { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.AcademicProgram> AcademicPrograms { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.Registration> Registrations { get; set; }


        public System.Data.Entity.DbSet<BITCollege_DP.Models.GradePointState> GradePointStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.ProbationState> ProbationStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.SuspendedState> SuspendedStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.RegularState> RegularStates { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.HonoursState> HonoursStates { get; set; }
        

        public System.Data.Entity.DbSet<BITCollege_DP.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.GradedCourse> GradedCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.AuditCourse> AuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.MasteryCourse> MasteryCourses { get; set; }


        public System.Data.Entity.DbSet<BITCollege_DP.Models.NextGradedCourse> NextGradedCourses  { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.NextAuditCourse> NextAuditCourses { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.NextMasteryCourse> NextMasteryCourses { get; set; }


        public System.Data.Entity.DbSet<BITCollege_DP.Models.NextUniqueNumber> NextUniqueNumbers { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.NextRegistration> NextRegistrations { get; set; }

        public System.Data.Entity.DbSet<BITCollege_DP.Models.NextStudent> NextStudents { get; set; }
    }
}
