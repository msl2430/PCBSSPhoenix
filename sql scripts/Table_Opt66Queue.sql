DROP TABLE [Phoenix].[Opt66Queue]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Phoenix].[Opt66Queue](
	[Opt66QueueId] [int] IDENTITY(1,1) NOT NULL,
	[CaseNumber] [int] NOT NULL,
	[PersonNumber] [int] NOT NULL,
	[Supv] [nvarchar](2) NULL,
	[Worker] [int] NULL,
	[ProgramStatus] [int] NULL,
	[CaseRedetDate] [datetime] NULL,
	[DisabilityRedetDate] [datetime] NULL,
	[ActionCode] [nvarchar](1) NULL,
 CONSTRAINT [PK_Opt66Queue] PRIMARY KEY CLUSTERED 
(
	[Opt66QueueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


