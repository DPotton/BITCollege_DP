using BITCollege_DP.Data;
using BITCollege_DP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BITCollegeWindows
{
    public partial class BatchUpdate : Form
    {
        BITCollege_DPContext db = new BITCollege_DPContext();
        private Batch batch = new Batch(); 

        /// <summary>
        /// 
        /// </summary>
        public BatchUpdate()
        {
            InitializeComponent();
            radAll.CheckedChanged += RadAll_CheckedChanged;
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadAll_CheckedChanged(object sender, EventArgs e)
        {
            // Check if radAll is checked
            if (radAll.Checked)
            {
                // Enable the AcademicProgram ComboBox
                descriptionComboBox.Enabled = false;
            }
            else
            {
                // Disable the AcademicProgram ComboBox
                descriptionComboBox.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the Batch processing
        /// Further code to be added.
        /// </summary>
        private void lnkProcess_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (radAll.Checked)
            {
                // All transmissions selected
                foreach (var item in descriptionComboBox.Items)
                {
                    var programAcronym = ((AcademicProgram)item).ProgramAcronym;
                    batch.ProcessTransmission(programAcronym);
                    rtxtLog.Text += batch.WriteLogData();
                    
                }
            }
            else
            {
                var selectedProgramAcronym = ((AcademicProgram)descriptionComboBox.SelectedItem).ProgramAcronym;
                batch.ProcessTransmission(selectedProgramAcronym);
                rtxtLog.Text += batch.WriteLogData();
            }
        }

        /// <summary>
        /// given:  Always open this form in top right of frame.
        /// Further code to be added.
        /// </summary>
        private void BatchUpdate_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);

            IEnumerable<AcademicProgram> academicPrograms = from academicProgram in db.AcademicPrograms select academicProgram;

            academicProgramBindingSource.DataSource = academicPrograms.ToList();

            descriptionComboBox.DisplayMember = "Description";
            descriptionComboBox.DisplayMember = "ProgramAcronym";

            descriptionComboBox.Enabled = false;

        }
    }
}
