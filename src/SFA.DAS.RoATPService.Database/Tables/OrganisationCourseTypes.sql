CREATE TABLE [dbo].[OrganisationCourseTypes]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [OrganisationId] UNIQUEIDENTIFIER NOT NULL, 
    [CourseTypeId] INT NOT NULL, 
    [HasAccess] BIT NOT NULL,
    CONSTRAINT [UQ_OrganisationCourseTypes_OrganisationId_CourseTypeId] UNIQUE (OrganisationId, CourseTypeId),
    CONSTRAINT [FK_OrganisationCourseTypes_Organisations] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisations]([Id]),
    CONSTRAINT [FK_OrganisationCourseTypes_CourseTypes] FOREIGN KEY ([CourseTypeId]) REFERENCES [CourseTypes]([Id])
)
