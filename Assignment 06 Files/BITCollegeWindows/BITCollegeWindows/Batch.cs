using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using BITCollege_DP.Data;
using Utility;
using BITCollegeWindows.CollegeRegistrationService;

namespace BITCollegeWindows
{
    /// <summary>
    /// Batch:  This class provides functionality that will validate
    /// and process incoming xml files.
    /// </summary>
    public class Batch
    {
        BITCollege_DPContext db = new BITCollege_DPContext();
        CollegeRegistrationClient client = new CollegeRegistrationClient();

        private String inputFileName;
        private String logFileName;
        private String logData;
        XDocument document;

        


        private void ProcessErrors(IEnumerable<XElement> beforeQuery, IEnumerable<XElement> afterQuery, String message)
        {
            // Find the records that exist in beforeQuery but not in afterQuery
            var failedRecords = beforeQuery.Except(afterQuery);

            // Process each failed record and append relevant information to logData
            foreach (var record in failedRecords)
            {
                logData += $"Error Details:\n";
                logData += $"InputFileName: {inputFileName}\n";
                logData += $"Program: {record.Element("program")}\n";
                logData += $"Student Number: {record.Element("student_no")}\n";
                logData += $"Course Number: {record.Element("course_no")}\n";
                logData += $"Registration Number: {record.Element("registration_no")}\n";
                logData += $"Type: {record.Element("type")}\n";
                logData += $"Grade: {record.Element("grade")}\n";
                logData += $"Notes: {record.Element("notes")}\n";
                logData += $"Number of Nodes: {record.Elements().Count()}\n";
                logData += $"Error Message: {message}\n\n";
            }
        } 


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void ProcessHeader()
        {
            document = XDocument.Load(inputFileName);

            XElement rootStudent = document.Element("student_update");
            IEnumerable<XElement> rootStudents = document.Elements("student_update");

            //IEnumerable<XElement> rootTransactions = document.Descendants("transaction");
            //IEnumerable<XElement> programs = rootStudent.Elements("program");

            XElement checkSum = rootStudent.Element("checksum");
            IEnumerable<XElement> checkSums = rootStudents.Elements("checksum");

            if (rootStudents.Attributes().Count() != 3)
            {
                throw new Exception(String.Format("Incorrect number of root Attributes" + " for file {0}\n", inputFileName));
            }
            else if (!DateTime.Parse(rootStudent.Attribute("date").Value).Equals(DateTime.Now))
            {
                throw new Exception(String.Format("Incorrect date" + " for file {0}\n", inputFileName));
            }
            else if (rootStudent.Attribute("program").Value != db.AcademicPrograms.FirstOrDefault().ProgramAcronym)
            {
                throw new Exception(String.Format("Incorrect program acronym for file {0}\n", inputFileName));

            }
            else if (checkSum != checkSums)
            {
                throw new Exception(String.Format("Checksum did not match sum of all Student Numbers for file {0}", inputFileName));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void ProcessDetails()
        {
            document = XDocument.Load(inputFileName);
            IEnumerable<XElement> rootTransaction = document.Elements("transactions");

            // Round 1: Each transaction element in the xml file must have 7 child elements
            IEnumerable<XElement> round1Query = rootTransaction.Where(element => element.Elements().Nodes().Count() == 7);
            ProcessErrors(rootTransaction, round1Query, "Validation failed: Each transaction must have exactly 7 child elements.");

            // Round 2: The program element must match the program attribute of the root element
            IEnumerable<XElement> round2Query = round1Query.Where(element => element.Element("program").Value.Equals("programacronym"));
            ProcessErrors(round1Query, round2Query, "Validation failed: The program element must match the program attribute of the root element.");

            // Round 3: The type element within each transaction element must be numeric
            IEnumerable<XElement> round3Query = round2Query.Where(element => Numeric.IsNumeric(element.Element("type")?.Value, System.Globalization.NumberStyles.Integer));
            ProcessErrors(round2Query, round3Query, "Validation failed: The type element within each transaction element must be numeric.");

            // Round 4: The grade element within each transaction element must be numeric or have the value of ‘*’
            IEnumerable<XElement> round4Query = round3Query.Where(element => Numeric.IsNumeric(element.Element("grade").Value, System.Globalization.NumberStyles.Integer) || element.Element("grade").Value == "*");
            ProcessErrors(round3Query, round4Query, "Validation failed: The grade element within each transaction must be numeric or have the value of '*'.");

            // Round 5: The type element within each transaction element must have a value of 1 or 2
            IEnumerable<XElement> round5Query = round4Query.Where(element => element.Element("type").Value == "1" || element.Element("type").Value == "2");
            ProcessErrors(round4Query, round5Query, "Validation failed: The type element within each transaction must have a value of 1 or 2.");

            // Round 6: The grade element for course registrations (type element = 1) within each transaction element must have a value of “*”
            IEnumerable<XElement> round6Query = round5Query.Where(element => (element.Element("type").Value == "1" && element.Element("grade").Value == "*") ||
                (element.Element("type").Value == "2" && element.Element("grade").Value == "0 - 100"));
            ProcessErrors(round5Query, round6Query, "Validation failed: The grade element for course registrations (type 1) must have a value of '*' and for course grading (type 2) must be between 0 and 100 inclusive.");

            // Round 7: The student_no element within each transaction element must exist in the database
            var studentNumberQuery = from student in db.Students select student.StudentNumber;
            IEnumerable<long> studentNumbers = studentNumberQuery.ToList();
            IEnumerable<XElement> round7Query = round6Query.Where(element => studentNumbers.Contains(long.Parse(element.Element("student_no").Value)));
            ProcessErrors(round6Query, round7Query, "Validation failed: The student_no element within each transaction must exist in the database.");

            // Round 8: The course_no element within each transaction element must be a “*” for grading (type 2) or it must exist in the database for a registration (type 1)
            // var courseNumbers = GetValidCourseNumbers();
            var courseQuery = from course in db.Courses select course.CourseNumber;
            IEnumerable<string> courseNumbers = courseQuery.ToList();
            IEnumerable<XElement> round8Query = round7Query.Where(element => (element.Element("type").Value == "2" && element.Element("course_no").Value == "*") ||
                (element.Element("type")?.Value == "1" && courseNumbers.Contains(element.Element("course_no").Value)));
            ProcessErrors(round7Query, round8Query, "Validation failed: The course_no element within each transaction must be a '*' for grading (type 2) or it must exist in the database for a registration (type 1).");

            // Round 9: The registration_no element within each transaction element must be a “*” for course registration(type 1) or it must exist in the database for grading(type 2)
            var registrationQuery = from registration in db.Registrations select registration.RegistrationNumber;
            IEnumerable<long> registrationNumbers = registrationQuery.ToList();
            IEnumerable<XElement> round9Query = round8Query.Where(element => (element.Element("type").Value == "1" && element.Element("registration_no").Value == "*") ||
                (element.Element("type").Value == "2" && registrationNumbers.Contains(long.Parse(element.Element("registration_no").Value))));
            ProcessErrors(round8Query, round9Query, "Validation failed: The registration_no element within each transaction must be a '*' for course registration (type 1) or it must exist in the database for grading (type 2).");

            //  Call the ProcessTransactions method passing the error free result set
            ProcessTransactions(round9Query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionRecords"></param>
        private void ProcessTransactions(IEnumerable<XElement> transactionRecords)
        {
            foreach (var transaction in transactionRecords)
            {
                var type = int.Parse(transaction.Element("type").Value);
                var studentNo = long.Parse(transaction.Element("student_no").Value);
                var courseNo = transaction.Element("course_no")?.Value;
                var registrationNo = transaction.Element("registration_no").Value;
                var grade = transaction.Element("grade").Value;
                var notes = transaction.Element("notes").Value;

                if (type == 1) // Registration
                {
                    try
                    {
                        var registrationResult = client.RegisterCourse((int)studentNo, int.Parse(courseNo), notes);

                        if (registrationResult == 0) // Successful registration
                        {
                            logData += $"Registration for Student {studentNo} into Course {courseNo} was successful.\n";
                        }
                        else // Unsuccessful registration
                        {
                            logData += $"Registration for Student {studentNo} into Course {courseNo} failed: {BusinessRules.RegisterError(registrationResult)}.\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        logData += $"Exception occurred while processing registration for Student {studentNo} into Course {courseNo}: {ex.Message}.\n";
                    }
                }
                else if (type == 2) // Grading
                {
                    try
                    {
                        // Convert grade to appropriate format (0 to 1)
                        var normalizedGrade = grade == "*" ? null : (double?)(double.Parse(grade) / 100);

                        // Call WCF Service to update student's grade
                        var gradeUpdateResult = client.UpdateGrade((double)normalizedGrade, int.Parse(registrationNo), notes);

                        if (gradeUpdateResult.HasValue) // Successful grade update
                        {
                            logData += $"Grade update for Student {studentNo} on Registration {registrationNo} was successful.\n";
                        }
                        else // Unsuccessful grade update
                        {
                            logData += $"Grade update for Student {studentNo} on Registration {registrationNo} failed.\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        logData += $"Exception occurred while processing grade update for Student {studentNo} on Registration {registrationNo}: {ex.Message}.\n";
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public String WriteLogData()
        {
            // Check if logData is not empty and logFileName is not empty
            if (!string.IsNullOrEmpty(logData) && !string.IsNullOrEmpty(logFileName))
            {
                using (StreamWriter writer = new StreamWriter(logFileName, true))
                {
                    writer.Write(logData);
                }

                string capturedLogData = logData;

                logData = "";

                logFileName = "";
                
                return capturedLogData;
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="programAcronym"></param>
        public void ProcessTransmission(String programAcronym)
        {
            inputFileName = "Data.xml";

            // Check if the inputFileName file exists
            if (!File.Exists(inputFileName))
            {
                // If the file does not exist, append a relevant message to logData
                logData += $"File {inputFileName} does not exist.\n";
            }
            else
            {
                try
                {
                    // Formulate the inputFileName based on the filename format
                    inputFileName = $"{DateTime.Now.Year}-{DateTime.Now.DayOfYear:D3}-{programAcronym}.xml";

                    // Formulate logFileName that represents the name of the log file
                    logFileName = $"LOG {inputFileName.Replace(".xml", ".txt")}";

                    // Call the ProcessHeader method
                    ProcessHeader();
                    // Call the ProcessDetails method
                    ProcessDetails();
                }
                catch (Exception ex)
                {
                    // If an exception occurs, append a relevant message to logData
                    logData += $"Exception occurred: {ex.Message}\n";
                }
            }
        }
    }
}