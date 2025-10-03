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
	
	DECLARE @Efectivo nvarchar(50) ='Efectivo';
	DECLARE @CuentasDeBanco nvarchar(50) = 'Cuentas de Banco';
	DECLARE @Tarjetas nvarchar(50) = 'Tarjetas';

	INSERT INTO AccountTypes(Name, UserId, "Order")
	VALUES (@Efectivo, @UserId, 1),
	(@CuentasDeBanco, @UserId, 2),
	(@Tarjetas, @UserId, 3);

	INSERT INTO Accounts(Name, Balance, AccountTypeId)
	SELECT Name, 0, Id
	FROM AccountTypes At
	WHERE UserId = @UserId;

	INSERT INTO Categories(Name, OperationTypeId, UserId)
	VALUES
	('Libros', 2, @UserId),
	('Salario', 1, @UserId),
	('Mesada', 1, @UserId),
	('Comida', 2, @UserId);

END
