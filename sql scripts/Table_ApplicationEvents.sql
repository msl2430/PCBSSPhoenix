DROP TABLE [Phoenix].[ApplicationEvents]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Phoenix].[ApplicationEvents](
	[ApplicationEventId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationId] [int] NOT NULL,
	[EventTypeId] [int] NOT NULL,
	[EventTimestamp] [datetime] NOT NULL,
	[Details] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ApplicationEvents] PRIMARY KEY CLUSTERED 
(
	[ApplicationEventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


