SELECT tp.Id AS TrainingId, tp.[Name] AS TrainingName, tp.StartDate, tp.EndDate, tp.MaxAttendees, e.FirstName, e.LastName
                        FROM TrainingProgram tp
                        LEFT JOIN EmployeeTraining et ON et.TrainingProgramId = tp.Id
                        LEFT JOIN Employee e ON et.EmployeeId = e.Id
                        WHERE tp.Id = 1
