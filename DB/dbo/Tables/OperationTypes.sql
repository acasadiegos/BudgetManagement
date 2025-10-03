CREATE TABLE [dbo].[OperationTypes] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_OperationTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_OperationTypes_Id] UNIQUE NONCLUSTERED ([Id] ASC)
);

