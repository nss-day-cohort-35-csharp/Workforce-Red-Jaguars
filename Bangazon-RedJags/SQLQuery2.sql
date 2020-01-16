SELECT c.Id, c.PurchaseDate, c.DecomissionDate, c.Make, c.Model
                                        FROM Computer c
                                        LEFT JOIN Employee e
                                            ON c.Id = e.ComputerId
                                        WHERE e.ComputerId is Null
