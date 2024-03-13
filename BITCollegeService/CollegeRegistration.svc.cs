/*
* Name:    Dylan Potton
* Program: Business Information Technology
* Course:  ADEV-3008 Programming 3
* Created: 03.13.2024
*/

using BITCollege_DP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using BITCollege_DP.Models;

namespace BITCollegeService
{
    public class CollegeRegistration : ICollegeRegistration
    {
        BITCollege_DPContext dbContext = new BITCollege_DPContext();

        /// <summary>
        /// Removes a course registration from the database based on the provided registration ID.
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        public bool DropCourse(int registrationId)
        {
            Registration currentRegistration = dbContext.Registrations.FirstOrDefault(r => r.RegistrationId == registrationId);

            if (currentRegistration != null)
            {
                dbContext.Registrations.Remove(currentRegistration);
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Registers a course for a student.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public int RegisterCourse(int studentId, int courseId, string notes)
        {
            try
            {
                IQueryable<Registration> registrationQuery = dbContext.Registrations
                    .Where(r => r.StudentId == studentId && r.CourseId == courseId);

                Course registerCourse = dbContext.Courses.FirstOrDefault(c => c.CourseId == courseId);

                if (registrationQuery.Any(r => r.Grade == null))
                {
                    return -100;
                }

                if (registerCourse is MasteryCourse masteryCourse)
                {
                    int registrationsCount = registrationQuery.Count();
                    if (registrationsCount >= masteryCourse.MaximumAttempts)
                    {
                        return -200;
                    }
                }

                Student student = dbContext.Students.FirstOrDefault(s => s.StudentId == studentId);

                double adjustedTuition = registerCourse.TuitionAmount * student.GradePointState.TuitionRateAdjustment(student);

                Registration newRegistration = new Registration
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    RegistrationDate = DateTime.Now,
                    Notes = notes
                };

                newRegistration.SetNextRegistrationNumber();

                dbContext.Registrations.Add(newRegistration);

                dbContext.SaveChanges();

                student.OutstandingFees += adjustedTuition;

                dbContext.SaveChanges();

                return 0;
            }
            catch (Exception)
            {
                return -300;
            }
        }

        /// <summary>
        /// Updates the grade for a registration.
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="registrationId"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public double? UpdateGrade(double grade, int registrationId, string notes)
        {
            Registration currentRegistration = dbContext.Registrations.FirstOrDefault(r => r.RegistrationId == registrationId);

            if (currentRegistration != null)
            {
                currentRegistration.Grade = grade;
                currentRegistration.Notes = notes;

                dbContext.SaveChanges();

                double? gradePointAverage = CalculateGradePointAverage(currentRegistration.StudentId);

                return gradePointAverage;
            }

            return null;
        }

        /// <summary>
        /// Calculates the grade point average for a student.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        private double? CalculateGradePointAverage(int studentId)
        {
            IQueryable<Registration> registrations = dbContext.Registrations
                .Where(r => r.StudentId == studentId && r.Grade.HasValue)
                .Where(r => !(r.Course is AuditCourse));

            double totalGradePointValue = registrations.Sum(r => r.Student.GradePointState.GradePointStateId);
            double totalCreditHours = registrations.Sum(r => r.Course.CreditHours);

            if (totalCreditHours == 0)
            {
                return null;
            }

            double gradePointAverage = totalGradePointValue / totalCreditHours;

            Student student = dbContext.Students.FirstOrDefault(s => s.StudentId == studentId);
            student.GradePointAverage = gradePointAverage;

            dbContext.SaveChanges();

            student.ChangeState();

            return gradePointAverage;
        }
    }
}