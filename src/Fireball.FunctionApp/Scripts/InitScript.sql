IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'DistributedCache')
BEGIN
    CREATE TABLE [dbo].[DistributedCache] (
        [Id]                         NVARCHAR (449)     COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
        [Value]                      VARBINARY (MAX)    NOT NULL,
        [ExpiresAtTime]              DATETIMEOFFSET (7) NOT NULL,
        [SlidingExpirationInSeconds] BIGINT             NULL,
        [AbsoluteExpiration]         DATETIMEOFFSET (7) NULL,
        PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE NONCLUSTERED INDEX [Index_ExpiresAtTime]
        ON [dbo].[DistributedCache]([ExpiresAtTime] ASC);
END