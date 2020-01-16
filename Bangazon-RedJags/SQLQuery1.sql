SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Model, e.FirstName, e.LastName
FROM Computer c
LEFT JOIN Employee e
ON c.Id = e.ComputerId;