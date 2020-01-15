SELECT * FROM Department;

SELECT COUNT(e.Id) as EmployeeCount, d.[Name], d.Id, d.Budget
FROM Department d
LEFT JOIN Employee e On d.Id = e.DepartmentId
GROUP BY d.Name, d.Id, d.Budget;