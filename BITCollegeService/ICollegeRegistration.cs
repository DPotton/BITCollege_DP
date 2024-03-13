/*
* Name:    Dylan Potton
* Program: Business Information Technology
* Course:  ADEV-3008 Programming 3
* Created: 03.13.2024
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BITCollegeService
{
    [ServiceContract]
    public interface ICollegeRegistration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [OperationContract]
        bool DropCourse(int registrationId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="courseId"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        [OperationContract]
        int RegisterCourse(int studentId, int courseId, string notes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="registrationId"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        [OperationContract]
        double? UpdateGrade(double grade, int registrationId, string notes);
    }
}
