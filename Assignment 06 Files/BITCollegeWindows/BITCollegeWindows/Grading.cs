/*
 * Name:    Dylan Potton
 * Program: Business Information Technology
 * Course:  ADEV-3008 Programming 3
 * Created: 04/01/2024
 */

using BITCollege_DP.Data;
using BITCollege_DP.Models;
using BITCollegeWindows.CollegeRegistrationService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace BITCollegeWindows
{
    /// <summary>
    /// Represents a form for grading student registrations. 
    /// Allows updating grades and navigating back to the StudentData form.
    /// </summary>
    public partial class Grading : Form
    {
        BITCollege_DPContext db = new BITCollege_DPContext();

        CollegeRegistrationClient client = new CollegeRegistrationClient();

        ConstructorData constructorData;

        /// <summary>
        /// Initializes a new instance of the Grading form with the provided constructor data.
        /// </summary>
        /// <param name="constructor"></param>
        public Grading(ConstructorData constructor)
        {
            InitializeComponent();
            this.constructorData = constructor;

            studentBindingSource.DataSource = constructor.studentData;
            registrationBindingSource.DataSource = constructor.registrationData;

            this.lnkReturn.LinkClicked += lnkReturn_LinkClicked;

            this.lnkUpdate.LinkClicked += lnkUpdate_LinkClicked;

        }


        /// <summary>
        /// Returns to the StudentData form with the selected data from this form.
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
        /// Sets up initial form settings and data bindings for grading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grading_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            Registration registration = new Registration();
            registration = constructorData.registrationData;
            registrationBindingSource.DataSource = registration;

           Student student = new Student();
            student = constructorData.studentData;
            studentBindingSource.DataSource = student;

            string mask = BusinessRules.CourseFormat(registration.Course.CourseType);
            courseNumberMaskedLabel.Mask = mask;

            if (registration.Grade != null)
            {
                gradeTextBox.Enabled = true;
                lnkUpdate.Enabled = false;
                lblExisting.Visible = true;
            }
            else
            {
                gradeTextBox.Enabled = true;
                lnkUpdate.Enabled = true;
                lblExisting.Visible = false;
                gradeTextBox.Focus();

            }
        }

        /// <summary>
        /// Updates the grade for the current registration data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string gradeText = gradeTextBox.Text; ;

            if (!double.TryParse(gradeText, out double gradeValue))
            {
                MessageBox.Show("Please enter the grade as a decimal value such that it appropriately displays when formatted as a percent.",
                                "Invalid Grade", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (gradeValue < 0 || gradeValue > 1)
            {
                MessageBox.Show("Grade must be within the range of 0 to 1.", "Invalid Grade", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Attempt to update the grade using the client
            try
            {
                client.UpdateGrade(constructorData.registrationData.Grade, constructorData.registrationData.RegistrationId, constructorData.registrationData.Notes);

                gradeTextBox.Enabled = true;

                MessageBox.Show("Grade updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating the grade: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}