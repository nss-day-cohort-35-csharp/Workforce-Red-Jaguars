SELECT d.Name, d.Budget, e.firstName, e.lastName
FROM Department d
RIGHT JOIN Employee e ON e.DepartmentId = d.Id
WHERE d.Id = 2;