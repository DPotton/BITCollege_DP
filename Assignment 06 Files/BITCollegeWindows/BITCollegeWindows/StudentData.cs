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
    public partial class StudentData : Form
    {

        BITCollege_DPContext db = new BITCollege_DPContext();


        ///Given: Student and Registration data will be retrieved
        ///in this form and passed throughout application
        ///These variables will be used to store the current
        ///Student and selected Registration
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

        private void StudentNumberMaskedTextBox_Leave(object sender, EventArgs e)
        {
            string studentNumberString = studentNumberMaskedTextBox.Text;
            long studentNumber;


            // Remove any hyphens from the input string
            studentNumberString = studentNumberString.Replace("-", "");


            // Try parsing the student number string to a long
            if (!long.TryParse(studentNumberString, out studentNumber))
            {
                // Handle invalid input here (e.g., show error message)
                MessageBox.Show("Invalid student number format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                studentNumberMaskedTextBox.Focus();
                return; // Exit the method
            }

            // Define LINQ-to-SQL Server query selecting data from the Students table
            var studentQuery = from student in db.Students
                               where student.StudentNumber == studentNumber
                               select student;

            var studentRecord = studentQuery.FirstOrDefault(); 

            if (studentRecord == null)
            {
                // No student record found
                lnkUpdateGrade.Enabled = false;
                lnkViewDetails.Enabled = false;
                studentBindingSource.DataSource = typeof(Student); 
                registrationBindingSource1.DataSource = typeof(Registration);
                MessageBox.Show("The student number entered does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                studentNumberMaskedTextBox.Focus();
            }
            else
            {
                // Student record found
                studentBindingSource.DataSource = studentRecord;

                // Define LINQ-to-SQL Server query selecting all Registrations
                var registrationQuery = from registration in db.Registrations
                                        where registration.StudentId == studentRecord.StudentId
                                        select registration;

                var registrationRecords = registrationQuery.ToList(); // Retrieve registration records

                if (registrationRecords.Count == 0)
                {
                    // No registration records found
                    lnkUpdateGrade.Enabled = false;
                    lnkViewDetails.Enabled = false;
                    registrationBindingSource1.DataSource = typeof(Registration); // Clear previous results
                }
                else
                {
                    // Registration records found
                    lnkUpdateGrade.Enabled = true;
                    lnkViewDetails.Enabled = true;
                    registrationBindingSource1.DataSource = registrationRecords;
                }
            }

            this.Refresh();
        }

            /// <summary>
            /// given:  This constructor will be used when returning to StudentData
            /// from another form.  This constructor will pass back
            /// specific information about the student and registration
            /// based on activites taking place in another form.
            /// </summary>
            /// <param name="constructorData">constructorData object containing
            /// specific student and registration data.</param>
            public StudentData (ConstructorData constructor)
        {
            InitializeComponent();
            //Further code to be added.
        }

        /// <summary>
        /// given: Open grading form passing constructor data.
        /// </summary>
        private void lnkUpdateGrade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Grading grading = new Grading(constructorData);
            grading.MdiParent = this.MdiParent;
            grading.Show();
            this.Close();
        }


        /// <summary>
        /// given: Open history form passing constructor data.
        /// </summary>
        private void lnkViewDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            History history = new History(constructorData);
            history.MdiParent = this.MdiParent;
            history.Show();
            this.Close();
        }

        /// <summary>
        /// given:  Opens the form in top right corner of the frame.
        /// </summary>
        private void StudentData_Load(object sender, EventArgs e)
        {
            //keeps location of form static when opened and closed


            this.Location = new Point(0, 0);

            // Get the entered student number string from the MaskedTextBox
            string enteredStudentNumberString = studentNumberMaskedTextBox.Text;

            // Convert the entered student number string to a long
            long enteredStudentNumber;
            if (!long.TryParse(enteredStudentNumberString, out enteredStudentNumber))
            {
                // Handle invalid input here, such as displaying an error message
                return;
            }

            // LINQ query using method syntax to retrieve student data based on student number
            IEnumerable<Student> query = db.Students
                .Where(student => student.StudentNumber == enteredStudentNumber);

            // Execute the query and get the result
            Student studentData = query.FirstOrDefault();


            courseBindingSource.DataSource = (from results in db.Courses select results).ToList();

            studentBindingSource.DataSource = (from results in db.Students select results).ToList();

            registrationBindingSource1.DataSource = (from results in db.Registrations select results).ToList();

            this.Refresh();

            lnkUpdateGrade.Enabled = false;
            lnkViewDetails.Enabled = false;

            studentNumberMaskedTextBox.Focus();

        }
    }
}
