/*
   Скрипт для создания структуры базы данных Kopilka в SQL Server Management Studio (SSMS).
   Выполните этот скрипт в контексте вашей базы данных (например, KopilkaDB).
*/

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Families] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [HomePassword] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Families] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Login] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [FamilyId] int NULL,
    [Email] nvarchar(max) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Families_FamilyId] FOREIGN KEY ([FamilyId]) REFERENCES [Families] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [IsIncome] bit NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [DebtContracts] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Principal] decimal(18,2) NOT NULL,
    [InterestRate] decimal(18,2) NOT NULL,
    [PenaltyRate] decimal(18,2) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    [Counterparty] nvarchar(max) NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_DebtContracts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DebtContracts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FinancialGoals] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [TargetAmount] decimal(18,2) NOT NULL,
    [CurrentAmount] decimal(18,2) NOT NULL,
    [TargetDate] datetime2 NOT NULL,
    [OwnerUserId] int NOT NULL,
    [IsFamilyGoal] bit NOT NULL,
    CONSTRAINT [PK_FinancialGoals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FinancialGoals_Users_OwnerUserId] FOREIGN KEY ([OwnerUserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Reminders] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [ReminderDate] datetime2 NOT NULL,
    [IsRecurring] bit NOT NULL,
    [TransactionCategoryId] int NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Reminders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reminders_Categories_TransactionCategoryId] FOREIGN KEY ([TransactionCategoryId]) REFERENCES [Categories] ([Id]),
    CONSTRAINT [FK_Reminders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Transactions] (
    [Id] int NOT NULL IDENTITY,
    [Amount] decimal(18,2) NOT NULL,
    [Date] datetime2 NOT NULL,
    [Comment] nvarchar(max) NOT NULL,
    [CategoryId] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transactions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Transactions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PaymentHistories] (
    [Id] int NOT NULL IDENTITY,
    [DebtContractId] int NOT NULL,
    [Date] datetime2 NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Note] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PaymentHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentHistories_DebtContracts_DebtContractId] FOREIGN KEY ([DebtContractId]) REFERENCES [DebtContracts] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PaymentSchedules] (
    [Id] int NOT NULL IDENTITY,
    [DebtContractId] int NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [AmountDue] decimal(18,2) NOT NULL,
    [IsPaid] bit NOT NULL,
    [PaidDate] datetime2 NULL,
    CONSTRAINT [PK_PaymentSchedules] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentSchedules_DebtContracts_DebtContractId] FOREIGN KEY ([DebtContractId]) REFERENCES [DebtContracts] ([Id]) ON DELETE CASCADE
);
GO

-- Индексы для оптимизации
CREATE INDEX [IX_Categories_UserId] ON [Categories] ([UserId]);
CREATE INDEX [IX_DebtContracts_UserId] ON [DebtContracts] ([UserId]);
CREATE INDEX [IX_FinancialGoals_OwnerUserId] ON [FinancialGoals] ([OwnerUserId]);
CREATE INDEX [IX_PaymentHistories_DebtContractId] ON [PaymentHistories] ([DebtContractId]);
CREATE INDEX [IX_PaymentSchedules_DebtContractId] ON [PaymentSchedules] ([DebtContractId]);
CREATE INDEX [IX_Transactions_CategoryId] ON [Transactions] ([CategoryId]);
CREATE INDEX [IX_Transactions_UserId] ON [Transactions] ([UserId]);
GO

COMMIT;
GO
