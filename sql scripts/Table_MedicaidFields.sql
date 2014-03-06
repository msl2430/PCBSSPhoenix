ALTER TABLE [Phoenix].[MedicaidFields] DROP CONSTRAINT [FK_MedicaidFields_MedicaidForms]
GO

DROP TABLE [Phoenix].[MedicaidFields]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Phoenix].[MedicaidFields](
	[MedicaidFieldId] [int] IDENTITY(1,1) NOT NULL,
	[MedicaidFormId] [int] NOT NULL,
	[FieldName] [nvarchar](2000) NOT NULL,
	[FieldNumber] [int] NOT NULL,
	[FieldLength] [int] NOT NULL,
	[StartIndex] [int] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [Phoenix].[MedicaidFields]  WITH CHECK ADD  CONSTRAINT [FK_MedicaidFields_MedicaidForms] FOREIGN KEY([MedicaidFormId])
REFERENCES [Phoenix].[MedicaidForms] ([FormId])
GO

ALTER TABLE [Phoenix].[MedicaidFields] CHECK CONSTRAINT [FK_MedicaidFields_MedicaidForms]
GO


