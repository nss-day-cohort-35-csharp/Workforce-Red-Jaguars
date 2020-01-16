<<<<<<< HEAD
﻿SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Model
                                        FROM Computer c
                                        LEFT JOIN Employee e
                                            ON c.Id = e.ComputerId
                                        WHERE e.ComputerId is Null
=======
﻿SELECT d.Name, d.Budget, e.firstName, e.lastName
FROM Department d
RIGHT JOIN Employee e ON e.DepartmentId = d.Id
WHERE d.Id = 2;
>>>>>>> 6023cbad62e688e8b04dbd40dd89abc49e3fb458
