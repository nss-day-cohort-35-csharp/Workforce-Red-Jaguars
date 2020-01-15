SELECT e.FirstName, e.LastName, d.Name DepartmentName
                                        FROM Employee e
                                        LEFT JOIN Department d
                                            ON e.DepartmentId = d.Id
