-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Update_Transactions
	-- Add the parameters for the stored procedure here
	@Id int,
	@TransactionDate datetime,
	@Amount decimal(18,2),
	@PreviousAmount decimal(18,2),
	@AccountId int,
	@PreviousAccountId int,
	@CategorieId int,
	@Note nvarchar(1000) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Revert previous transaction
	UPDATE Accounts
	SET Balance -= @PreviousAmount
	WHERE Id = @PreviousAmount;

	-- Make the new transaction
	Update Accounts
	SET Balance += @Amount
	WHERE Id = @AccountId;

	UPDATE Transactions
	SET Amount = ABS(@Amount),
	TransactionDate = @TransactionDate,
	CategorieId = @CategorieId,
	AccountId = @AccountId,
	Note = @Note
	WHERE Id = @Id;
	
END
