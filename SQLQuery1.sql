UPDATE dbo.Histories
SET PaymentStatus = 1
WHERE  Id=4; -- Only change if the current status is Paid

update dbo.Histories
set MonthlyRent=4500
where Id=4;