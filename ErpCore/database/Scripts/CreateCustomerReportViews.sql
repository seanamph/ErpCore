-- 客戶報表查詢視圖 (CUS5130)

IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_CUS5130_Report]'))
BEGIN
    DROP VIEW [dbo].[V_CUS5130_Report];
END
GO

CREATE VIEW [dbo].[V_CUS5130_Report] AS
SELECT 
    c.[CustomerId],
    c.[CustomerName],
    c.[GuiId],
    c.[GuiType],
    c.[ContactStaff],
    c.[CompTel],
    c.[Cell],
    c.[Email],
    c.[City],
    c.[Canton],
    c.[Addr],
    c.[Status],
    c.[TransDate],
    c.[AccAmt],
    c.[MonthlyYn],
    ISNULL(COUNT(ct.[TransactionId]), 0) AS TransactionCount,
    ISNULL(SUM(ct.[Amount]), 0) AS TotalAmount
FROM [dbo].[Customers] c
LEFT JOIN [dbo].[CustomerTransactions] ct ON c.[CustomerId] = ct.[CustomerId]
GROUP BY 
    c.[CustomerId], c.[CustomerName], c.[GuiId], c.[GuiType],
    c.[ContactStaff], c.[CompTel], c.[Cell], c.[Email],
    c.[City], c.[Canton], c.[Addr], c.[Status],
    c.[TransDate], c.[AccAmt], c.[MonthlyYn];
GO

PRINT 'V_CUS5130_Report 視圖建立成功';
GO

