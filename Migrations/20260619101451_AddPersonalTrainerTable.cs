using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace slowfit.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalTrainerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Progetto db-first: applichiamo solo le modifiche realmente nuove (tabella personalTrainer,
            // colonne ptId su training/nutrition, spostamento dei PT esistenti e foreign key) con SQL
            // idempotente, coerente con lo stile delle altre migration scritte a mano.
            migrationBuilder.Sql("""
                SET XACT_ABORT ON;

                -- 1) Tabella personalTrainer (stessi campi di un utente + credenziali + dati fiscali)
                IF OBJECT_ID(N'[personalTrainer]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [personalTrainer] (
                        [ptId] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_personalTrainer] PRIMARY KEY,
                        [firstName] VARCHAR(50) NOT NULL,
                        [surname] VARCHAR(50) NOT NULL,
                        [email] VARCHAR(50) NOT NULL,
                        [password] VARCHAR(MAX) NOT NULL,
                        [address] VARCHAR(50) NULL,
                        [city] VARCHAR(50) NULL,
                        [country] VARCHAR(50) NULL,
                        [province] VARCHAR(50) NULL,
                        [zipCode] INT NULL,
                        [birthDate] DATE NULL,
                        [imageProfile] VARCHAR(MAX) NULL,
                        [phone] VARCHAR(50) NULL,
                        [FcmToken] NVARCHAR(MAX) NULL,
                        [refreshTokenHash] VARCHAR(256) NULL,
                        [refreshTokenExpiresAt] DATETIME2 NULL,
                        [refreshTokenRevokedAt] DATETIME2 NULL,
                        [vatNumber] VARCHAR(20) NULL,
                        [fiscalCode] VARCHAR(16) NULL,
                        [sdiCode] VARCHAR(7) NULL,
                        [pecEmail] VARCHAR(100) NULL
                    );
                END

                -- 2) Colonne ptId (nullable) per attribuire i piani al PT che li ha assegnati
                IF COL_LENGTH(N'[training]', N'ptId') IS NULL
                    ALTER TABLE [training] ADD [ptId] INT NULL;

                IF COL_LENGTH(N'[nutrition]', N'ptId') IS NULL
                    ALTER TABLE [nutrition] ADD [ptId] INT NULL;

                -- 3) Spostamento dei PT esistenti (roleId = 2) dalla tabella user a personalTrainer,
                --    preservando l'id così tutti i riferimenti (user.ptId, appointment.ptId) restano validi.
                IF EXISTS (SELECT 1 FROM [user] WHERE [roleId] = 2)
                BEGIN
                    SET IDENTITY_INSERT [personalTrainer] ON;
                    INSERT INTO [personalTrainer]
                        ([ptId], [firstName], [surname], [email], [password], [address], [city], [country],
                         [province], [zipCode], [birthDate], [imageProfile], [phone], [FcmToken],
                         [refreshTokenHash], [refreshTokenExpiresAt], [refreshTokenRevokedAt])
                    SELECT
                        [userId], [firstName], [surname], [email], [password], [address], [city], [country],
                        [province], [zipCode], [birthDate], [imageProfile], [phone], [FcmToken],
                        [refreshTokenHash], [refreshTokenExpiresAt], [refreshTokenRevokedAt]
                    FROM [user] WHERE [roleId] = 2;
                    SET IDENTITY_INSERT [personalTrainer] OFF;

                    -- Rimuove eventuali notifiche destinate ai PT (la FK punta a user) prima di eliminarli.
                    DELETE FROM [NotificationsFire]
                    WHERE [ReceiverId] IN (SELECT [userId] FROM [user] WHERE [roleId] = 2);

                    DELETE FROM [user] WHERE [roleId] = 2;
                END

                -- 3b) Bonifica i riferimenti orfani prima di creare le foreign key:
                --     ptId che non corrispondono ad alcun personal trainer (dati pregressi/seed incoerenti).
                UPDATE [user] SET [ptId] = NULL
                WHERE [ptId] IS NOT NULL
                  AND [ptId] NOT IN (SELECT [ptId] FROM [personalTrainer]);

                -- appointment.ptId è NOT NULL: gli appuntamenti che puntano a un PT inesistente vengono rimossi.
                DELETE FROM [appointment]
                WHERE [ptId] NOT IN (SELECT [ptId] FROM [personalTrainer]);

                -- 4) Indici sulle colonne ptId
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_user_ptId' AND object_id = OBJECT_ID(N'[user]'))
                    CREATE INDEX [IX_user_ptId] ON [user] ([ptId]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_training_ptId' AND object_id = OBJECT_ID(N'[training]'))
                    CREATE INDEX [IX_training_ptId] ON [training] ([ptId]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_nutrition_ptId' AND object_id = OBJECT_ID(N'[nutrition]'))
                    CREATE INDEX [IX_nutrition_ptId] ON [nutrition] ([ptId]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_appointment_ptId' AND object_id = OBJECT_ID(N'[appointment]'))
                    CREATE INDEX [IX_appointment_ptId] ON [appointment] ([ptId]);

                -- 5) Foreign key verso personalTrainer
                IF OBJECT_ID(N'[FK_user_personalTrainer]', N'F') IS NULL
                    ALTER TABLE [user] ADD CONSTRAINT [FK_user_personalTrainer]
                        FOREIGN KEY ([ptId]) REFERENCES [personalTrainer] ([ptId]);
                IF OBJECT_ID(N'[FK_training_personalTrainer]', N'F') IS NULL
                    ALTER TABLE [training] ADD CONSTRAINT [FK_training_personalTrainer]
                        FOREIGN KEY ([ptId]) REFERENCES [personalTrainer] ([ptId]);
                IF OBJECT_ID(N'[FK_nutrition_personalTrainer]', N'F') IS NULL
                    ALTER TABLE [nutrition] ADD CONSTRAINT [FK_nutrition_personalTrainer]
                        FOREIGN KEY ([ptId]) REFERENCES [personalTrainer] ([ptId]);
                IF OBJECT_ID(N'[FK_appointment_personalTrainer]', N'F') IS NULL
                    ALTER TABLE [appointment] ADD CONSTRAINT [FK_appointment_personalTrainer]
                        FOREIGN KEY ([ptId]) REFERENCES [personalTrainer] ([ptId]);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SET XACT_ABORT ON;

                -- Rimuove le foreign key
                IF OBJECT_ID(N'[FK_appointment_personalTrainer]', N'F') IS NOT NULL
                    ALTER TABLE [appointment] DROP CONSTRAINT [FK_appointment_personalTrainer];
                IF OBJECT_ID(N'[FK_nutrition_personalTrainer]', N'F') IS NOT NULL
                    ALTER TABLE [nutrition] DROP CONSTRAINT [FK_nutrition_personalTrainer];
                IF OBJECT_ID(N'[FK_training_personalTrainer]', N'F') IS NOT NULL
                    ALTER TABLE [training] DROP CONSTRAINT [FK_training_personalTrainer];
                IF OBJECT_ID(N'[FK_user_personalTrainer]', N'F') IS NOT NULL
                    ALTER TABLE [user] DROP CONSTRAINT [FK_user_personalTrainer];

                -- Rimuove gli indici
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_appointment_ptId' AND object_id = OBJECT_ID(N'[appointment]'))
                    DROP INDEX [IX_appointment_ptId] ON [appointment];
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_nutrition_ptId' AND object_id = OBJECT_ID(N'[nutrition]'))
                    DROP INDEX [IX_nutrition_ptId] ON [nutrition];
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_training_ptId' AND object_id = OBJECT_ID(N'[training]'))
                    DROP INDEX [IX_training_ptId] ON [training];
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_user_ptId' AND object_id = OBJECT_ID(N'[user]'))
                    DROP INDEX [IX_user_ptId] ON [user];

                -- Riporta i PT nella tabella user (roleId = 2) preservando l'id
                IF OBJECT_ID(N'[personalTrainer]', N'U') IS NOT NULL
                   AND EXISTS (SELECT 1 FROM [personalTrainer])
                BEGIN
                    SET IDENTITY_INSERT [user] ON;
                    INSERT INTO [user]
                        ([userId], [firstName], [surname], [email], [password], [address], [city], [country],
                         [province], [zipCode], [birthDate], [imageProfile], [phone], [FcmToken],
                         [refreshTokenHash], [refreshTokenExpiresAt], [refreshTokenRevokedAt], [roleId])
                    SELECT
                        [ptId], [firstName], [surname], [email], [password], [address], [city], [country],
                        [province], [zipCode], [birthDate], [imageProfile], [phone], [FcmToken],
                        [refreshTokenHash], [refreshTokenExpiresAt], [refreshTokenRevokedAt], 2
                    FROM [personalTrainer];
                    SET IDENTITY_INSERT [user] OFF;
                END

                -- Rimuove le colonne ptId dai piani
                IF COL_LENGTH(N'[nutrition]', N'ptId') IS NOT NULL
                    ALTER TABLE [nutrition] DROP COLUMN [ptId];
                IF COL_LENGTH(N'[training]', N'ptId') IS NOT NULL
                    ALTER TABLE [training] DROP COLUMN [ptId];

                -- Elimina la tabella personalTrainer
                IF OBJECT_ID(N'[personalTrainer]', N'U') IS NOT NULL
                    DROP TABLE [personalTrainer];
                """);
        }
    }
}
