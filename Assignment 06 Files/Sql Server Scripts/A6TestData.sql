-- ================================================
-- Student and Registration Insert
-- Replace DATABASENAME below with your actual 
-- BITCollege_FLContext database name
-- ================================================

USE [DATABASENAME]
GO

DBCC CHECKIDENT(Students, reseed, 700)
GO

INSERT INTO [dbo].[Students]
           ([StudentNumber]
		,[GradePointStateId]
		,[AcademicProgramId]
		,[FirstName]
		,[LastName]
		,[Address]
		,[City]
		,[Province]
		,[DateCreated]
		,[GradePointAverage]
		,[OutstandingFees]
        ,[Notes])
     VALUES
(70000001,3,4,'Assignment 6','Student 1','701 Assignment 6 Road','Winnipeg','MB','1-Jan-20',NULL,500,'Expected Key 701')
,(70000002,3,4,'Assignment 6','Student 2','702 Assignment 6 Road','Winnipeg','MB','1-Jan-20',NULL,12.50,'Expected Key 702')
,(70000003,3,4,'Assignment 6','Student 3','703 Assignment 6 Road','Winnipeg','MB','1-Jan-20',NULL,12.50,'Expected Key 703')

GO

---

DBCC CHECKIDENT(Registrations, reseed, 700)
GO

INSERT INTO [dbo].[Registrations]
           ([StudentId]
           ,[CourseId]
           ,[RegistrationNumber]
		   ,[Grade]
           ,[RegistrationDate]
		   ,[Notes])
     VALUES
(701,134,7001,NULL,'3-Aug-22','Expected Key 701')
,(701,134,7002,NULL,'3-Aug-22','Expected Key 702')
,(701,134,7003,NULL, '1-Jul-22','Expected Key 703')
,(701,101,7004,NULL,'5-Jul-22','Expected Key 704')
,(701,118,7005,NULL,'8-Jul-22','Expected Key 705')
,(701,120,7006,NULL,'2-Jul-22','Expected Key 706')
,(701,156,7007,NULL,'2-Jul-22','Expected Key 707')
,(701,127,7008,NULL,'2-Jul-22','Expected Key 708')
,(702,134,7009,NULL,'3-Aug-22','Expected Key 709')
,(702,134,7010,NULL,'3-Aug-22','Expected Key 710')
,(702,134,7011,NULL, '1-Jul-22','Expected Key 711')
,(702,101,7012,NULL,'5-Jul-22','Expected Key 712')
,(702,118,7013,NULL,'8-Jul-22','Expected Key 713')
,(702,120,7014,NULL,'2-Jul-22','Expected Key 714')
,(702,156,7015,NULL,'2-Jul-22','Expected Key 715')
,(702,127,7016,NULL,'2-Jul-22','Expected Key 716')
GO

