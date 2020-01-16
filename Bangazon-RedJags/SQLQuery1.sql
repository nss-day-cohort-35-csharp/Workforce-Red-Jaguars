SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Model
FROM Computer c
LEFT JOIN Employee e
ON c.Id = e.ComputerId
WHERE c.Id = 10
HAVING e.ComputerId is Null