CREATE TABLE [dbo].[OrganisationProduct]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [OrganisationId] UNIQUEIDENTIFIER NOT NULL, 
    [ProductId] UNIQUEIDENTIFIER NOT NULL, 
    [HasAccess] BIT NOT NULL,
    CONSTRAINT [UQ_OrganisationProduct_OrganisationId_ProductId] UNIQUE (OrganisationId, ProductId),
    CONSTRAINT [FK_OrganisationProduct_Organisations] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisations]([Id]),
    CONSTRAINT [FK_OrganisationProduct_Product] FOREIGN KEY ([ProductId]) REFERENCES [Product]([Id])
)
