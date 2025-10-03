-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE AccountTypes_Insert
	@Name varchar(50),
	@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Order int;
	SELECT @Order = COALESCE(MAX(At."Order"),0)+1
	FROM AccountTypes At
	WHERE at.UserId = @UserId;

	INSERT INTO AccountTypes(Name, UserId, "Order")
	VALUES(@Name, @UserId, @Order);

	SELECT SCOPE_IDENTITY();
END
