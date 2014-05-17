DROP TABLE [Phoenix].[Opt61Queue]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Phoenix].[Opt61Queue](
	[Opt61QueueId] [int] IDENTITY(1,1) NOT NULL,
	[CaseNumber] [int] NULL,
	[PersonNumber] [int] NULL,
	[BatchNumber] [nvarchar](4) NULL,
	[ActionCode] [nvarchar](1) NULL,
	[Office] [int] NULL,
	[ProviderWarning] [nvarchar](1) NULL,
	[CaseName] [nvarchar](22) NULL,
	[Address] [nvarchar](20) NULL,
	[Address2] [nvarchar](20) NULL,
	[Address3] [nvarchar](22) NULL,
	[Address4] [nvarchar](22) NULL,
	[City] [nvarchar](18) NULL,
	[State] [nvarchar](2) NULL,
	[Zip] [int] NULL,
	[LastName] [nvarchar](12) NULL,
	[FirstName] [nvarchar](7) NULL,
	[Middle] [nvarchar](1) NULL,
	[DateOfBirth] [datetime] NULL,
	[SocialSecurity] [int] NULL,
	[Sex] [nvarchar](1) NULL,
	[MaritalStatus] [nvarchar](1) NULL,
	[Race] [nvarchar](1) NULL,
	[PriorCase] [nvarchar](10) NULL,
	[PriorPersonNumber] [int] NULL,
	[AlienType] [nvarchar](1) NULL,
	[TempDate] [datetime] NULL,
	[EffectiveDate] [datetime] NULL,
	[EffectiveDate2] [datetime] NULL,
	[EffectiveDate3] [datetime] NULL,
	[EffectiveDate4] [datetime] NULL,
	[TermDate] [datetime] NULL,
	[TermDate2] [datetime] NULL,
	[TermDate3] [datetime] NULL,
	[TermDate4] [datetime] NULL,
	[AddCode] [nvarchar](2) NULL,
	[AddCode2] [nvarchar](2) NULL,
	[AddCode3] [nvarchar](2) NULL,
	[AddCode4] [nvarchar](2) NULL,
	[TRMCode] [nvarchar](2) NULL,
	[TRMCode2] [nvarchar](2) NULL,
	[TRMCode3] [nvarchar](2) NULL,
	[TRMCode4] [nvarchar](2) NULL,
	[PGM] [nvarchar](3) NULL,
	[PGM2] [nvarchar](3) NULL,
	[PGM3] [nvarchar](3) NULL,
	[PGM4] [nvarchar](3) NULL,
	[SUPV] [nvarchar](3) NULL,
	[SUPV2] [nvarchar](3) NULL,
	[SUPV3] [nvarchar](3) NULL,
	[SUPV4] [nvarchar](3) NULL,
	[RES] [nvarchar](2) NULL,
	[RES2] [nvarchar](2) NULL,
	[RES3] [nvarchar](2) NULL,
	[RES4] [nvarchar](2) NULL,
	[ExtType] [nvarchar](1) NULL,
	[ExtType2] [nvarchar](1) NULL,
	[ExtType3] [nvarchar](1) NULL,
	[ExtType4] [nvarchar](1) NULL,
	[PregnancyDueDate] [datetime] NULL,
	[PregnancyDueDate2] [datetime] NULL,
	[PregnancyDueDate3] [datetime] NULL,
	[PregnancyDueDate4] [datetime] NULL,
	[AddressAction] [nvarchar](1) NULL,
	[PersonAction] [nvarchar](1) NULL,
	[EligSeg] [nvarchar](1) NULL,
	[Supervisor] [nvarchar](2) NULL,
	[Worker] [int] NULL
) ON [PRIMARY]

GO


