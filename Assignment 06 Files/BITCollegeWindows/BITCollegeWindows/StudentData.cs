/*
 * Name:    Dylan Potton
 * Program: Business Information Technology
 * Course:  ADEV-3008 Programming 3
 * Created: 04/01/2024
 */

using BITCollege_DP.Data;
using BITCollege_DP.Models;
using System;
using BITCollegeWindows.CollegeRegistrationService;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class StudentData : Form
    {

        BITCollege_DPContext db = new BITCollege_DPContext();

        CollegeRegistrationClient service = new CollegeRegistrationClient();

        // Create a new instance of the ConstructorData class
        ConstructorData constructorData = new ConstructorData();

        /// <summary>
        /// This constructor will be used when this form is opened from
        /// the MDI Frame.
        /// </summary>
        public StudentData()
        {
            InitializeComponent();

            this.studentNumberMaskedTextBox.Leave += StudentNumberMaskedTextBox_Leave;
        }

        /// <summary>
        /// Initializes a new instance of the StudentData class with data from the specified ConstructorData object.
        /// </summary>
        /// <param name="constructor"></param>
        public StudentData (ConstructorData constructor)
        {
            InitializeComponent();

            constructorData = constructor;
            this.constructorData.studentData = constructor.studentData;
            this.constructorData.registrationData = constructor.registrationData;
            this.constructorData.courseData = constructor.courseData;
            
            
        }

        /// <summary>
        /// Populates the constructorData object with current student and registration data.
        /// </summary>
        private void PopulateConstructorData()
        {
            Student selectedStudent = studentBindingSource.Current as Student;
            Registration selectedRegistration = registrationBindingSource.Current as Registration;

            constructorData = new ConstructorData
            {
                studentData = selectedStudent,
                registrationData = selectedRegistration
            };
        }


        /// <summary>
        /// Opens the Grading form with constructor data and closes the current form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkUpdateGrade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData();
            // Open the grading form passing constructorData
            Grading grading = new Grading(constructorData);
            grading.MdiParent = this.MdiParent;
            grading.Show();
            this.Close();
        }

        /// <summary>
        /// Opens the History form with constructor data and closes the current form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkViewDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PopulateConstructorData();
            History history = new History(constructorData);
            history.MdiParent = this.MdiParent;
            history.Show();
            this.Close();
        }

        /// <summary>
        /// Validates and processes a student number input. 
        /// Checks if the student exists in the database and updates the UI accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StudentNumberMaskedTextBox_Leave(object sender, EventArgs e)
        {
            string studentNumberString = studentNumberMaskedTextBox.Text;
            long studentNumber;


            // Remove any hyphens from the input string
            studentNumberString = studentNumberString.Replace("-", "");


            // Try parsing the student number string to a long
            if (!long.TryParse(studentNumberString, out studentNumber))
            {
                studentNumberMaskedTextBox.Focus();
                return;
            }

            // Define LINQ-to-SQL Server query selecting data from the Students table
            var studentQuery = from student in db.Students
                               where student.StudentNumber == studentNumber
                               select student;

            Student studentRecord = studentQuery.FirstOrDefault();


            if (studentRecord == null)
            {
                // No student record found
                lnkUpdateGrade.Enabled = false;
                lnkViewDetails.Enabled = false;
                studentBindingSource.DataSource = typeof(Student);
                registrationBindingSource.DataSource = typeof(Registration);

                MessageBox.Show($"Student {studentNumber} does not exist.", "Invalid Student Number",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                studentNumberMaskedTextBox.Focus();
                return;
            }
            else
            {
                studentBindingSource.DataSource = studentRecord;
                constructorData.studentData = studentRecord;

                // Define LINQ-to-SQL Server query selecting all Registrations
                var registrationQuery = from registration in db.Registrations
                                        where registration.StudentId == studentRecord.StudentId
                                        select registration;

                var registrationRecords = registrationQuery.ToList();



                if (registrationRecords.Count == 0)
                {
                    // No records found
                    lnkUpdateGrade.Enabled = false;
                    lnkViewDetails.Enabled = false;
                    registrationBindingSource.DataSource = typeof(Registration);
                }
                else
                {
                    // Records found
                    lnkUpdateGrade.Enabled = true;
                    lnkViewDetails.Enabled = true;
                    registrationBindingSource.DataSource = registrationRecords;
                    registrationNumberComboBox.Focus();
                }
            }

            return;
        }

        /// <summary>
        /// Sets up initial form settings such as location and focus.
        /// </summary>
        private void StudentData_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            lnkUpdateGrade.Enabled = false;
            lnkViewDetails.Enabled = false;

            studentNumberMaskedTextBox.Focus();
        }
    }
}
