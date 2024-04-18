/*
 * Name:    Dylan Potton
 * Program: Business Information Technology
 * Course:  ADEV-3008 Programming 3
 * Created: 04/01/2024
 */


using BITCollege_DP.Data;
using BITCollege_DP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class History : Form
    {
        BITCollege_DPContext db = new BITCollege_DPContext();

        // Declare a private field named constructorData of type ConstructorData
        private ConstructorData constructorData;

        /// <summary>
        /// Initializes a new instance of the History class with the provided ConstructorData object.
        /// </summary>
        /// <param name="constructorData"></param>
        public History(ConstructorData constructorData)
        {
            InitializeComponent();

            this.constructorData = constructorData;
        }

        /// <summary>
        /// Handles the click event for the lnkReturn LinkLabel.
        /// Returns to the StudentData form with the data selected for this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //return to student with the data selected for this form
            StudentData student = new StudentData(constructorData);
            student.MdiParent = this.MdiParent;
            student.Show();
            this.Close();
        }

        /// <summary>
        /// Handles the load event of the History form.
        /// Loads data from the database and sets up the DataGridView for displaying registration information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void History_Load(object sender, EventArgs e)
        {
            try
            {
                this.Location = new Point(0, 0);

                // Retrieve a singular student based on ID.
                int studentId = constructorData.studentData.StudentId;

                // LINQ-to-SQL Server query selecting data from Registrations and Courses tables
                // Joining Registrations and Courses tables based on CourseId
                var query = from registration in db.Registrations
                            join student in db.Students on registration.StudentId equals student.StudentId
                            join academicProgram in db.AcademicPrograms on student.AcademicProgramId equals academicProgram.AcademicProgramId
                            where student.StudentId == studentId
                            select new
                            {
                                student.StudentNumber,
                                FullName = student.FirstName + " " + student.LastName,
                                academicProgram.Description,
                                registration.RegistrationNumber,
                                registration.RegistrationDate,
                                Course = registration.Course.Title,
                                registration.Grade,
                                registration.Notes
                            };

                registrationDataGridView.DataSource = query.ToList();

                studentBindingSource.DataSource = constructorData.studentData;
            }
            catch (Exception ex)
            {
                // Proper exception handling
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
