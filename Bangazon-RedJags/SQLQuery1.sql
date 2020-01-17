SELECT p.Id, p.[Name], p.StartDate, p.EndDate, p.MaxAttendees
                                        FROM TrainingProgram p
                                        LEFT JOIN EmployeeTraining et
                                            ON p.Id = et.TrainingProgramId
                                        WHERE et.EmployeeId = 5
