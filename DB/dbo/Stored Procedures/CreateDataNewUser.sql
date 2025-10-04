-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE CreateDataNewUser
	@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	DECLARE @Cash nvarchar(50) ='Cash';
	DECLARE @BankAccounts nvarchar(50) = 'Bank Accounts';
	DECLARE @Cards nvarchar(50) = 'Cards';

	INSERT INTO AccountTypes(Name, UserId, "Order")
	VALUES (@Cash, @UserId, 1),
	(@BankAccounts, @UserId, 2),
	(@Cards, @UserId, 3);

	INSERT INTO Accounts(Name, Balance, AccountTypeId)
	SELECT Name, 0, Id
	FROM AccountTypes At
	WHERE UserId = @UserId;

	INSERT INTO Categories(Name, OperationTypeId, UserId)
	VALUES
	('Books', 2, @UserId),
	('Salary', 1, @UserId),
	('Pension', 1, @UserId),
	('Food', 2, @UserId);

END
