/*
==============================================================================
ПОЛНЫЙ SQL-СКРИПТ ДЛЯ СОЗДАНИЯ БАЗЫ ДАННЫХ KOPILKA + ДОПОЛНИТЕЛЬНЫЕ ТАБЛИЦЫ
Версия 1.1: Исправлена ошибка множественных каскадных путей (Error 1785)
==============================================================================
*/

USE [master];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'KopilkaDB')
BEGIN
    CREATE DATABASE [KopilkaDB];
END
GO

USE [KopilkaDB];
GO

-- Удаление в обратном порядке
IF OBJECT_ID('[MaterialSupplier]', 'U') IS NOT NULL DROP TABLE [MaterialSupplier];
IF OBJECT_ID('[Material]', 'U') IS NOT NULL DROP TABLE [Material];
IF OBJECT_ID('[MaterialType]', 'U') IS NOT NULL DROP TABLE [MaterialType];
IF OBJECT_ID('[Supplier]', 'U') IS NOT NULL DROP TABLE [Supplier];
IF OBJECT_ID('[SupplierType]', 'U') IS NOT NULL DROP TABLE [SupplierType];
IF OBJECT_ID('[ProductType]', 'U') IS NOT NULL DROP TABLE [ProductType];

IF OBJECT_ID('[PaymentSchedules]', 'U') IS NOT NULL DROP TABLE [PaymentSchedules];
IF OBJECT_ID('[PaymentHistories]', 'U') IS NOT NULL DROP TABLE [PaymentHistories];
IF OBJECT_ID('[Transactions]', 'U') IS NOT NULL DROP TABLE [Transactions];
IF OBJECT_ID('[Reminders]', 'U') IS NOT NULL DROP TABLE [Reminders];
IF OBJECT_ID('[FinancialGoals]', 'U') IS NOT NULL DROP TABLE [FinancialGoals];
IF OBJECT_ID('[DebtContracts]', 'U') IS NOT NULL DROP TABLE [DebtContracts];
IF OBJECT_ID('[Categories]', 'U') IS NOT NULL DROP TABLE [Categories];
IF OBJECT_ID('[UserSettings]', 'U') IS NOT NULL DROP TABLE [UserSettings];
IF OBJECT_ID('[Accounts]', 'U') IS NOT NULL DROP TABLE [Accounts];
IF OBJECT_ID('[Users]', 'U') IS NOT NULL DROP TABLE [Users];
IF OBJECT_ID('[Families]', 'U') IS NOT NULL DROP TABLE [Families];
GO

/* ЧАСТЬ 1: KOPILKA */

CREATE TABLE [Families] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [HomePassword] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_Families] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Login] NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [Role] NVARCHAR(50) NOT NULL,
    [FamilyId] INT NULL,
    [Email] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Families_FamilyId] FOREIGN KEY ([FamilyId]) REFERENCES [Families]([Id]) ON DELETE SET NULL
);

CREATE TABLE [Accounts] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [Type] NVARCHAR(100) NOT NULL,
    [Balance] DECIMAL(18, 2) NOT NULL DEFAULT 0,
    [Name] NVARCHAR(255) NOT NULL,
    [Currency] NVARCHAR(10) NOT NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Accounts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

CREATE TABLE [Categories] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [IsIncome] BIT NOT NULL DEFAULT 0,
    [UserId] INT NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

CREATE TABLE [Transactions] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Amount] DECIMAL(18, 2) NOT NULL,
    [Date] DATETIME2 NOT NULL,
    [Comment] NVARCHAR(MAX) NOT NULL,
    [CategoryId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [AccountId] INT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transactions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([Id]) ON DELETE CASCADE,
    -- Исправлено: ON DELETE NO ACTION для предотвращения циклов (Error 1785)
    CONSTRAINT [FK_Transactions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Transactions_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts]([Id])
);

CREATE TABLE [DebtContracts] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [Principal] DECIMAL(18, 2) NOT NULL,
    [InterestRate] DECIMAL(18, 2) NOT NULL,
    [PenaltyRate] DECIMAL(18, 2) NOT NULL,
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NULL,
    [Counterparty] NVARCHAR(255) NOT NULL,
    [UserId] INT NOT NULL,
    CONSTRAINT [PK_DebtContracts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DebtContracts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

CREATE TABLE [PaymentSchedules] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [DebtContractId] INT NOT NULL,
    [DueDate] DATETIME2 NOT NULL,
    [AmountDue] DECIMAL(18, 2) NOT NULL,
    [IsPaid] BIT NOT NULL DEFAULT 0,
    [PaidDate] DATETIME2 NULL,
    CONSTRAINT [PK_PaymentSchedules] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PaymentSchedules_DebtContracts_DebtContractId] FOREIGN KEY ([DebtContractId]) REFERENCES [DebtContracts]([Id]) ON DELETE CASCADE
);

CREATE TABLE [FinancialGoals] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [TargetAmount] DECIMAL(18, 2) NOT NULL,
    [CurrentAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0,
    [TargetDate] DATETIME2 NOT NULL,
    [OwnerUserId] INT NOT NULL,
    [IsFamilyGoal] BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_FinancialGoals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FinancialGoals_Users_OwnerUserId] FOREIGN KEY ([OwnerUserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

CREATE TABLE [Reminders] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [ReminderDate] DATETIME2 NOT NULL,
    [IsRecurring] BIT NOT NULL DEFAULT 0,
    [TransactionCategoryId] INT NULL,
    [UserId] INT NOT NULL,
    CONSTRAINT [PK_Reminders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reminders_Categories_TransactionCategoryId] FOREIGN KEY ([TransactionCategoryId]) REFERENCES [Categories]([Id]) ON DELETE SET NULL,
    -- Исправлено: ON DELETE NO ACTION для предотвращения циклов
    CONSTRAINT [FK_Reminders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE NO ACTION
);

CREATE TABLE [UserSettings] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [Language] NVARCHAR(50) NOT NULL DEFAULT 'ru-RU',
    CONSTRAINT [PK_UserSettings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserSettings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

/* ЧАСТЬ 2: ПРОИЗВОДСТВО */

CREATE TABLE [ProductType] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_ProductType] PRIMARY KEY ([ID])
);

CREATE TABLE [SupplierType] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_SupplierType] PRIMARY KEY ([ID])
);

CREATE TABLE [Supplier] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [SupplierTypeID] INT NOT NULL,
    [INN] NVARCHAR(12) NOT NULL,
    [Rating] INT NOT NULL,
    [StartDate] DATE NOT NULL,
    [City] NVARCHAR(100) NULL,
    CONSTRAINT [PK_Supplier] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_Supplier_SupplierType] FOREIGN KEY ([SupplierTypeID]) REFERENCES [SupplierType]([ID])
);

CREATE TABLE [MaterialType] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_MaterialType] PRIMARY KEY ([ID])
);

CREATE TABLE [Material] (
    [ID] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [MaterialTypeID] INT NOT NULL,
    [CountInPack] INT NOT NULL,
    [Unit] NVARCHAR(10) NOT NULL,
    [CountInStock] FLOAT NOT NULL,
    [MinCount] FLOAT NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Cost] DECIMAL(18, 2) NOT NULL,
    [Image] NVARCHAR(255) NULL,
    CONSTRAINT [PK_Material] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_Material_MaterialType] FOREIGN KEY ([MaterialTypeID]) REFERENCES [MaterialType]([ID])
);

CREATE TABLE [MaterialSupplier] (
    [MaterialID] INT NOT NULL,
    [SupplierID] INT NOT NULL,
    CONSTRAINT [PK_MaterialSupplier] PRIMARY KEY ([MaterialID], [SupplierID]),
    CONSTRAINT [FK_MaterialSupplier_Material] FOREIGN KEY ([MaterialID]) REFERENCES [Material]([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_MaterialSupplier_Supplier] FOREIGN KEY ([SupplierID]) REFERENCES [Supplier]([ID]) ON DELETE CASCADE
);
GO

PRINT 'База данных успешно создана без конфликтов путей удаления.';
GO
