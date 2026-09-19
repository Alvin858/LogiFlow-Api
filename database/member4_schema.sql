-- LogiFlow Member 4: Billing, Payments and Notifications
-- Use this only if you are managing the database manually instead of EF migrations.

CREATE TABLE Invoices (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Invoices PRIMARY KEY,
    InvoiceNumber NVARCHAR(50) NOT NULL,
    ShipmentId INT NOT NULL,
    CustomerId INT NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,
    TaxAmount DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    IssuedAtUtc DATETIME2 NOT NULL,
    DueDateUtc DATETIME2 NULL,
    PaidAtUtc DATETIME2 NULL,
    CONSTRAINT FK_Invoices_Shipments FOREIGN KEY (ShipmentId) REFERENCES Shipments(Id),
    CONSTRAINT FK_Invoices_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);
CREATE UNIQUE INDEX IX_Invoices_InvoiceNumber ON Invoices(InvoiceNumber);
CREATE UNIQUE INDEX IX_Invoices_ShipmentId ON Invoices(ShipmentId);
CREATE INDEX IX_Invoices_CustomerId ON Invoices(CustomerId);

CREATE TABLE Payments (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Payments PRIMARY KEY,
    InvoiceId INT NOT NULL,
    PaymentReference NVARCHAR(100) NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    PaidAtUtc DATETIME2 NULL,
    CONSTRAINT FK_Payments_Invoices FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX IX_Payments_PaymentReference ON Payments(PaymentReference);
CREATE INDEX IX_Payments_InvoiceId ON Payments(InvoiceId);

CREATE TABLE Notifications (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Notifications PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(1000) NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    IsRead BIT NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE INDEX IX_Notifications_UserId_IsRead_CreatedAtUtc ON Notifications(UserId, IsRead, CreatedAtUtc);
