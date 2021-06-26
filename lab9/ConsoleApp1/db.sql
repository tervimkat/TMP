CREATE TABLE Bicycle
(
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(30) NOT NULL,
    Firm NVARCHAR(20) NOT NULL,
    Cost INT NOT NULL
);
CREATE TABLE Client
(
    Id INT IDENTITY PRIMARY KEY,
    FirstName NVARCHAR(30) NOT NULL
);
CREATE TABLE Orders
(
    Id INT IDENTITY PRIMARY KEY,
    BicycleId INT NOT NULL REFERENCES Bicycle(Id) ON DELETE CASCADE,
    ClientId INT NOT NULL REFERENCES Client(Id) ON DELETE CASCADE,
    OrderDate Datetime NOT NULL,
    RentCost INT NOT NULL,
);

Go
CREATE PROCEDURE GetRents AS
SELECT Bicycle.Name, Bicycle.Firm, Orders.RentCost, Client.FirstName
FROM Bicycle INNER JOIN Orders ON Orders.BicycleId = Bicycle.Id 
	INNER JOIN Client ON Orders.ClientId = Client.Id 
Go

EXEC GetRents

go
CREATE FUNCTION GetCountByFirm (@firm NVARCHAR(20))
    RETURNS INT
    AS 
	BEGIN 
		DECLARE @returnvalue INT;
		SELECT @returnvalue = COUNT(*) FROM Bicycle WHERE Bicycle.Firm like @firm
		RETURN(@returnvalue);
	END
go

SELECT * FROM GetCountByFirm('stela')

go
CREATE TYPE MyTableType AS Table
(BicycleId int,
ClientId int,
OrderDate datetime);
go

if (EXISTS (select top 1 * from Client) and EXISTS (select top 1 * from Bicycle))
begin
	declare @perem as MyTableType
	insert into @perem values ((select top 1 id from Bicycle ORDER BY id desc),
							   (select top 1 id from Client ORDER BY id desc),
							   '10.10.2020');
	insert into Orders values((select top 1 BicycleId from @perem),
							   (select top 1 ClientID from @perem),
							   (select top 1 OrderDate from @perem),
							   1000);
end;
