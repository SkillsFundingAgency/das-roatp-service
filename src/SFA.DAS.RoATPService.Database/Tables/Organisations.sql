﻿CREATE TABLE [dbo].[Organisations](
	[Id] [uniqueidentifier] NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL, 
    [CreatedBy] NVARCHAR(30) NOT NULL, 
    [UpdatedAt] DATETIME2 NULL, 
    [UpdatedBy] NVARCHAR(30) NULL, 
    [Status] NVARCHAR(20) NOT NULL, 
	[ApplicationRouteId] INT NOT NULL,
	[OrganisationTypeId] int NOT NULL,  
	[UKPRN] bigint NOT NULL,
	[LegalName] [nvarchar] (200) NOT NULL,
	[TradingName] [nvarchar] (200) NULL,
	[StatusStartDate] [datetime] NOT NULL,
	[StatusEndDate] [datetime] NULL,
	OrganisationData [nvarchar](max) NULL
	
 CONSTRAINT [PK_Organisations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [FK_Organisations_ApplicationRoute] FOREIGN KEY ([ApplicationRouteId]) REFERENCES [ApplicationRoutes]([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Organisations] ADD CONSTRAINT [FK_OrganisationType_Organisations] FOREIGN KEY([OrganisationTypeId])
REFERENCES [dbo].[OrganisationTypes] ([Id]);
GO
 ALTER TABLE [dbo].[Organisations] CHECK CONSTRAINT [FK_OrganisationType_Organisations];
GO

CREATE INDEX [IX_Organisations_UKPRN] ON [dbo].[Organisations] ([UKPRN])

GO

CREATE INDEX [IX_Organisations_LegalName] ON [dbo].[Organisations] ([LegalName])

GO

CREATE INDEX [IX_Organisations_TradingName] ON [dbo].[Organisations] ([TradingName])
