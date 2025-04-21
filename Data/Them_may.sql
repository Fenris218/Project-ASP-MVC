INSERT INTO dbo.Devices (Type, Status, PricePerHour)
VALUES (N'Vip', N'available', 10000);

SELECT * FROM dbo.Devices
WHERE (Status='available');