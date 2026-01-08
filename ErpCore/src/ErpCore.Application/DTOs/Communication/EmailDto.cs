namespace ErpCore.Application.DTOs.Communication;

/// <summary>
/// 郵件記錄 DTO
/// </summary>
public class EmailLogDto
{
    public long Id { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string? FromName { get; set; }
    public string ToAddress { get; set; } = string.Empty;
    public string? CcAddress { get; set; }
    public string? BccAddress { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string? BodyType { get; set; } = "Text";
    public int? Priority { get; set; } = 3;
    public string Status { get; set; } = "Pending";
    public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasAttachment { get; set; }
    public int? AttachmentCount { get; set; } = 0;
}

/// <summary>
/// 發送郵件請求 DTO
/// </summary>
public class SendEmailRequestDto
{
    public string FromAddress { get; set; } = string.Empty;
    public string? FromName { get; set; }
    public string ToAddress { get; set; } = string.Empty;
    public string? CcAddress { get; set; }
    public string? BccAddress { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Body { get; set; }
    public string? BodyType { get; set; } = "Text";
    public int? Priority { get; set; } = 3;
    public List<EmailAttachmentDto>? Attachments { get; set; }
}

/// <summary>
/// 使用模板發送郵件請求 DTO
/// </summary>
public class SendEmailTemplateRequestDto
{
    public string TemplateCode { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string? CcAddress { get; set; }
    public string? BccAddress { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
}

/// <summary>
/// 郵件附件 DTO
/// </summary>
public class EmailAttachmentDto
{
    public long Id { get; set; }
    public long EmailLogId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 郵件記錄查詢 DTO
/// </summary>
public class EmailLogQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 發送郵件回應 DTO
/// </summary>
public class SendEmailResponseDto
{
    public long EmailLogId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SentAt { get; set; }
}

