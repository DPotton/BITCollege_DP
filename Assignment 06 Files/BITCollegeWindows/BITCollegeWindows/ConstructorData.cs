using BITCollege_DP.Data;
using BITCollege_DP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BITCollegeWindows
{
    /// <summary>
    /// ConstructorData:  This class is used to capture data to be passed
    /// among the windows forms.
    /// Further code to be added.
    /// </summary>
    public class ConstructorData
    {
        BITCollege_DPContext db = new BITCollege_DPContext();

        public Student studentData { get; set; }

        public Registration registrationData { get; set; }

        public Course courseData { get; set; }

    }
}
