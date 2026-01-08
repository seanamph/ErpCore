namespace ErpCore.Domain.Entities.Sales;

/// <summary>
/// 銷售處理記錄 (SYSD210-SYSD230)
/// </summary>
public class SalesProcessLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 銷售單號
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 處理類型 (SHIP:出貨, RETURN:退貨, CANCEL:取消)
    /// </summary>
    public string ProcessType { get; set; } = string.Empty;

    /// <summary>
    /// 處理狀態 (SUCCESS:成功, FAILED:失敗)
    /// </summary>
    public string ProcessStatus { get; set; } = string.Empty;

    /// <summary>
    /// 處理訊息
    /// </summary>
    public string? ProcessMessage { get; set; }

    /// <summary>
    /// 處理人員
    /// </summary>
    public string? ProcessUserId { get; set; }

    /// <summary>
    /// 處理時間
    /// </summary>
    public DateTime ProcessDate { get; set; }

    /// <summary>
    /// 處理資料（JSON格式）
    /// </summary>
    public string? ProcessData { get; set; }
}

