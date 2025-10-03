-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Transactions_Insert] 
	@UserId int,
	@TransactionDate date,
	@Amount decimal(18,2),
	@CategorieId int,
	@AccountId int,
	@Note nvarchar(1000) = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO Transactions(UserId, TransactionDate,
	Amount, CategorieId, AccountId, Note)
	Values(@UserId, @TransactionDate, ABS(@Amount), @CategorieId, @AccountId, @Note)

	UPDATE Accounts
	SET Balance += @Amount
	WHERE Id = @AccountId;

	SELECT SCOPE_IDENTITY();
END
