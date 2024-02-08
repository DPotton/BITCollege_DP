/*
 * Name:    Dylan Potton
 * Program: Business Information Technology
 * Course:  ADEV-3008 Programming 3
 * Created: 01.25.2024
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Security;
using static BITCollege_DP.Models.GradePointState;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;
using Utility;
using BITCollege_DP.Data;
using System.EnterpriseServices;
using System.Security.Cryptography.X509Certificates;
using System.Data.Entity;


namespace BITCollege_DP.Models
{
    /// <summary>
    /// Student Model, represents the Students table in the database.
    /// </summary>
    public class Student
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }

        [Required (ErrorMessage = "Please enter a valid Grade Point State.")]
        [ForeignKey("GradePointState")]
        public int GradePointStateId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Required (ErrorMessage = "Please enter a valid Student Number.")]
        [Range(10000000, 99999999)]
        [Display(Name = "Student\nNumber")]
        public long StudentNumber { get; set; }

        [Required (ErrorMessage = "Please enter a last name.")]
        [Display(Name = "First Name", Prompt = "First\nName")]
        public string FirstName { get; set; }

        [Required (ErrorMessage = "Please enter a first name.")]
        [Display(Name = "Last Name", Prompt = "Last\nName")]
        public string LastName { get; set; }

        [Required (ErrorMessage = "Please enter a valid Address.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter a valid City")]
        public string City { get; set; }

        [Required]
        [RegularExpression("^(N[BLSTU]|[AMN]B|[BQ]C|ON|PE|SK|YT)", ErrorMessage = "Please enter a valid Canadian Province.")]
        public string Province { get; set; }

        [Required (ErrorMessage = "Please enter a valid date.")]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateCreated { get; set; }

        [Display(Name ="Grade Point\nAverage")]
        [Range(0.00, 4.50)]
        public double? GradePointAverage { get; set; }

        [Required (ErrorMessage = "Please enter an Outstanding Fee value.")]
        [Display(Name = "Fees")]
        [DisplayFormat(DataFormatString = "{0:c")]
        [Range(0.00, 999999.99)]
        public double OutstandingFees { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Name")]
        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }

        [Display(Name = "Address")]
        public string FullAddress
        {
            get { return String.Format("{0} {1} {2}", Address, City, Province); }
        }


        private BITCollege_DPContext dbContext = new BITCollege_DPContext();

        /// <summary>
        /// Method for ChangeState.
        /// </summary>
        /// 
        public void ChangeState()
        {
            GradePointState currentState = dbContext.GradePointStates.Find(GradePointStateId);
            int newState = 0;

            while (newState != currentState.GradePointStateId)
            {
                currentState.StateChangeCheck(this);
                newState = currentState.GradePointStateId;
                currentState = dbContext.GradePointStates.Find(GradePointStateId);
            }
        }


        /// <summary>
        /// Navigation property for the GradePointState cardinality.
        /// </summary>
        public virtual GradePointState GradePointState { get; set; }

        /// <summary>
        /// Navigation Property for the AcademicProgram cardinality.
        /// </summary>
        public virtual AcademicProgram AcademicProgram { get; set; }

        /// <summary>
        /// Navigation Property for the Registration collection cardinality.
        /// </summary>
        public virtual ICollection<Registration> Registration { get; set; }
    }

    /// <summary>
    /// AcademicProgram Model, represents the AcademicProgram table in the database.
    /// </summary>
    public class AcademicProgram
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AcademicProgramId { get; set; }

        [Required]
        [Display(Name = "Program")]
        public string ProgramAcronym { get; set; }

        [Required]
        [Display(Name = "Program Name", Prompt = "Last\nName")]
        public string Description { get; set; }

        /// <summary>
        /// Navigation Property for the Student collection cardinality.
        /// </summary>
        public virtual ICollection<Student> Student { get; set; }

        /// <summary>
        /// Navigation Property for the Course cardinality.
        /// </summary>
        public virtual ICollection<Course> Course { get; set; }
    }

    /// <summary>
    /// GradePointState Model, represents the GradePointState table in the database.
    /// </summary>
    public abstract class GradePointState
    {
        
        

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int GradePointStateId { get; set; }

        [Required]
        [Range(0.00, 999999.99)]
        [Display(Name ="Lower\nLimit")]
        public double LowerLimit { get; set; }

        [Required]
        [Range(0.00, 999999.99)]
        [Display(Name ="Upper\nLimit")]
        public double UpperLimit { get; set; }

        [Required]
        [Range(0.00, 999999.99)]
        [Display(Name ="Tuition Rate\nFactor")]
        public double TuitionRateFactor { get; set; }

        [DisplayName("State")]
        public string Description
        {
            get
            {
                string typeName = this.GetType().Name;

                int stateIndex = typeName.IndexOf("State");

                string stateName;

                if (stateIndex != -1)
                {
                    stateName = typeName.Substring(0, stateIndex);
                }
                else
                {
                    stateName = typeName;
                }

                return stateName;
            }

        }


        /// <summary>
        /// Abstract method for TuitionRateAdjustment.
        /// </summary>
        public abstract double TuitionRateAdjustment(Student student);

        /// <summary>
        /// Abstract method for StateChangeCheck.
        /// </summary>
        public abstract void StateChangeCheck(Student student);

        protected static BITCollege_DPContext dbContext = new BITCollege_DPContext();

        /// <summary>
        /// Navigation Property for the Student collection cardinality.
        /// </summary>
        public virtual ICollection<Student> Student { get; set; }
    }


    /// <summary>
    /// SuspendedState Model, derived from the GradPointState class.
    /// </summary>
    public class SuspendedState : GradePointState
    {
        private static SuspendedState instance;

        private const double SUSPENDED_LOWER_LIMIT = 0.0;
        private const double SUSPENDED_UPPER_LIMIT = 1.0;
        private const double SUSPENDED_TUITION_RATE_FACTOR = 1.1;

        /// <summary>
        /// Method for SuspendState.
        /// </summary>
        private SuspendedState()
        {
            LowerLimit = SUSPENDED_LOWER_LIMIT;
            UpperLimit = SUSPENDED_UPPER_LIMIT;
            TuitionRateFactor = SUSPENDED_TUITION_RATE_FACTOR;
        }

        /// <summary>
        /// Method for SuspendedState.
        /// </summary>
        public static SuspendedState GetInstance()
        {
            if (instance == null)
            {
                instance = dbContext.SuspendedStates.SingleOrDefault();

                if (instance == null)
                {
                    instance = new SuspendedState();

                    dbContext.GradePointStates.Add(instance);

                    dbContext.SaveChanges();
                }
            }

            return instance;
        }

        /// <summary>
        /// Method for TuitionRateAdjustment.
        /// </summary>
        public override double TuitionRateAdjustment(Student student)
        {
            double adjustedTuition = TuitionRateFactor;

            if (student.GradePointAverage < 0.75)
            {
                adjustedTuition += 0.2;
            }
            else if (student.GradePointAverage < 0.50)
            {
                adjustedTuition += 0.5;
            }
            return adjustedTuition;
        }

        /// <summary>
        /// Method for StateChangeCheck.
        /// </summary>
        public override void StateChangeCheck(Student student)
        {
            if (student.GradePointAverage > UpperLimit)
            {
                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            }
        }
    }


    /// <summary>
    /// ProbationState Model, derived from the GradPointState class.
    /// </summary>
    public class ProbationState : GradePointState
    {
        private static ProbationState instance;

        private const double PROBATION_LOWER_LIMIT = 1.00;
        private const double PROBATION_UPPER_LIMIT = 2.00;
        private const double PROBATION_TUITION_RATE_FACTOR = 1.075;

        /// <summary>
        /// Method for ProbationState.
        /// </summary>
        private ProbationState()
        {
            LowerLimit = PROBATION_LOWER_LIMIT;
            UpperLimit = PROBATION_UPPER_LIMIT;
            TuitionRateFactor = PROBATION_TUITION_RATE_FACTOR;
        }

        /// <summary>
        /// Method for ProbationState.
        /// </summary>
        public static ProbationState GetInstance()
        {
            if (instance == null)
            {
                instance = dbContext.ProbationStates.SingleOrDefault();

                if (instance == null)
                {
                    instance = new ProbationState();

                    dbContext.GradePointStates.Add(instance);

                    dbContext.SaveChanges();
                }
            }

            return instance;
        }

        /// <summary>
        /// Method for TuitionRateAdjustment.
        /// </summary>
        public override double TuitionRateAdjustment(Student student)
        {

            IQueryable<Registration> courses = dbContext.Registrations.Where(x => x.StudentId == student.StudentId && x.Grade != null);

            int courseCount = courses.Count();

            if (student.GradePointState == ProbationState.GetInstance())
            {
                return student.OutstandingFees * 0.075;
            }
            else
            {
                if (courseCount >= 5)
                {
                    return student.OutstandingFees * 0.035;
                }
                else
                {
                    return TuitionRateFactor;
                }
            }
        }

        /// <summary>
        /// Method for StateChangeCheck.
        /// </summary>
        public override void StateChangeCheck(Student student)
        {
            // State from Probation to Regular
            if (student.GradePointAverage > UpperLimit)
            {
                GradePointState regState = RegularState.GetInstance();

                student.GradePointStateId = regState.GradePointStateId;
            }

            // State from Probation to Suspended
            else if (student.GradePointAverage < LowerLimit)
            {
                GradePointState susState = SuspendedState.GetInstance();

                student.GradePointStateId = susState.GradePointStateId;
            }
        }
    }

    /// <summary>
    /// RegularState Model, derived from the GradPointState class.
    /// </summary>
    public class RegularState : GradePointState
    {
        private static RegularState instance;

        private const double REGULAR_LOWER_LIMIT = 2.00;
        private const double REGULAR_UPPER_LIMIT = 3.70;
        private const double REGULAR_TUITION_RATE_FACTOR = 1.0;

        /// <summary>
        /// Method for RegularState.
        /// </summary>
        private RegularState()
        {
            LowerLimit = REGULAR_LOWER_LIMIT;
            UpperLimit = REGULAR_UPPER_LIMIT;
            TuitionRateFactor = REGULAR_TUITION_RATE_FACTOR;
        }

        /// <summary>
        /// Method for RegularState.
        /// </summary>
        public static RegularState GetInstance()
        {
            if (instance == null)
            {
                instance = dbContext.RegularStates.SingleOrDefault();

                if (instance == null)
                {
                    instance = new RegularState();

                    dbContext.GradePointStates.Add(instance);

                    dbContext.SaveChanges();
                }
            }

            return instance;
        }

        /// <summary>
        /// Method for TuitionRateAdjustment.
        /// </summary>
        public override double TuitionRateAdjustment(Student Student)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method for StateChangeCheck.
        /// </summary>
        public override void StateChangeCheck(Student student)
        {
            // State from Regular to Honours
            if (student.GradePointAverage > UpperLimit)
            {
                student.GradePointStateId = HonoursState.GetInstance().GradePointStateId;
            }

            // State from Regular to Probation
            else if (student.GradePointAverage < LowerLimit)
            {

                student.GradePointStateId = ProbationState.GetInstance().GradePointStateId;
            }
        }
    }

    /// <summary>
    /// HonoursState Model, derived from the GradPointState class.
    /// </summary>
    public class HonoursState : GradePointState
    {
        private static HonoursState instance;

        // Private constants for readability
        private const double HONOURS_LOWER_LIMIT = 3.70;
        private const double HONOURS_UPPER_LIMIT = 4.50;
        private const double HONOURS_TUITION_RATE_FACTOR = 0.9;

        /// <summary>
        /// Method for HonoursState.
        /// </summary>
        private HonoursState()
        {
            LowerLimit = HONOURS_LOWER_LIMIT;
            UpperLimit = HONOURS_UPPER_LIMIT;
            TuitionRateFactor = HONOURS_TUITION_RATE_FACTOR;
        }

        /// <summary>
        /// Method for HonoursState.
        /// </summary>
        public static HonoursState GetInstance()
        {
            if (instance == null)
            {
                instance = dbContext.HonoursStates.SingleOrDefault();

                if (instance == null)
                {
                    instance = new HonoursState();

                    dbContext.GradePointStates.Add(instance);

                    dbContext.SaveChanges();
                }
            }

            return instance;
        }

        /// <summary>
        /// Method for TuitionRateAdjustment.
        /// </summary>
        public override double TuitionRateAdjustment(Student Student)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method for StateChangeCheck.
        /// </summary>
        public override void StateChangeCheck(Student student)
        {
            // State from Honours to Regular
            if (student.GradePointAverage < LowerLimit)
            {
                student.GradePointStateId = RegularState.GetInstance().GradePointStateId;
            }
        }
    }

    /// <summary>
    /// Course Model, represents the Course table in the database.
    /// </summary>
    public abstract class Course
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }

        [ForeignKey("AcademicProgram")]
        public int? AcademicProgramId { get; set; }

        [Required]
        [Display(Name ="Course\nNumber")]
        public string CourseNumber { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Range(0.00, 9999.99)]
        [Display(Name ="Credit\nHours")]
        public double CreditHours { get; set; }

        [Required(ErrorMessage = "Please enter a tuition amount.")]
        [Display(Name = "Tuition")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public double TuitionAmount { get; set; }

        [Display(Name ="Course\nType")]
        public string CourseType 
        { 
            get
            {
                string typeName = this.GetType().Name;

                int courseIndex = typeName.IndexOf("Course Type");

                string courseName;

                if (courseIndex != -1)
                {
                    courseName = typeName.Substring(0, courseIndex);
                }
                else
                {
                    courseName = typeName;
                }

                return courseName;
            }
        }

        public string Notes { get; set; }

        /// <summary>
        /// Navigation Property for the Registration cardinality.
        /// </summary>
        public virtual ICollection<Registration> Registration { get; set; }

        /// <summary>
        /// Navigation Property for the AcademicProgram cardinality.
        /// </summary>
        public virtual AcademicProgram AcademicProgram { get; set; }
    }

    /// <summary>
    /// GradedCourse Model, derived from the Course class.
    /// </summary>
    public class GradedCourse : Course
    {
        [Required]
        [DisplayName("Assignments")]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double AssignmentWeight { get; set; }

        [Required]
        [DisplayName("Exams")]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double ExamWeight { get; set; }
    }

    /// <summary>
    /// MasteryCourse Model, derived from the Course class.
    /// </summary>
    public class MasteryCourse : Course
    {
        [Required]
        [Display(Name ="Maximum\nAttempts")]
        public int MaximumAttempts { get; set; }
    }

    /// <summary>
    /// AugitCourse Model, derived from the course class.
    /// </summary>
    public class AuditCourse : Course
    {

    }

    /// <summary>
    /// Registration Model, represents the Registration table in the database.
    /// </summary>
    public class Registration
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RegistrationId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Required]
        [Display(Name ="Registration\nNumber")]
        public long RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Please enter a valid date.")]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime RegistrationDate { get; set; }

        [DisplayFormat(NullDisplayText ="Ungraded")]
        [Range(0, 1)]
        public double? Grade { get; set; }

        public string Notes { get; set; }

        /// <summary>
        /// Navigation Property for the Student cardinality.
        /// </summary>
        public virtual Student Student { get; set; }

        /// <summary>
        /// Navigation Property for the Course cardinality.
        /// </summary>
        public virtual Course Course { get; set; }
    }
}