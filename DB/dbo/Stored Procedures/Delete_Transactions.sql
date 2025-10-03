-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Delete_Transactions
	-- Add the parameters for the stored procedure here
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @Amount decimal(18,2);
	DECLARE @AccountId int;
	DECLARE @OperationTypeId int;

	SELECT @Amount = T.Amount, @AccountId = T.AccountId,
	@OperationTypeId = cat.OperationTypeId
	FROM Transactions T
	INNER JOIN Categories Cat
	ON Cat.Id = T.CategorieId
	WHERE T.Id = @Id;

	DECLARE @MultiplicativeFactor int = 1;

	IF (@OperationTypeId = 2)
		SET @MultiplicativeFactor = -1;

	SET @Amount = @Amount * @MultiplicativeFactor;

	UPDATE Accounts
	SET Balance -= @Amount
	WHERE Id = @AccountId;

	DELETE Transactions
	WHERE Id = @Id;
END
