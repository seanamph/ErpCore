using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.System;
using ErpCore.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace ErpCore.Api.Controllers.System;

/// <summary>
/// 角色欄位權限控制器 (SYS0330)
/// </summary>
[ApiController]
[Route("api/v1/role-field-permissions")]
public class RoleFieldPermissionsController : ControllerBase
{
    private readonly IRoleFieldPermissionService _service;

    public RoleFieldPermissionsController(IRoleFieldPermissionService service)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢資料庫列表
    /// </summary>
    [HttpGet("databases")]
    public async Task<ActionResult<ApiResponse<List<DatabaseDto>>>> GetDatabases()
    {
        try
        {
            var result = await _service.GetDatabasesAsync();
            return Ok(new ApiResponse<List<DatabaseDto>>
            {
                Success = true,
                Data = result,
                Message = "查詢成功"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<DatabaseDto>>
            {
                Success = false,
                Message = $"查詢失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 查詢表格列表
    /// </summary>
    [HttpGet("tables")]
    public async Task<ActionResult<ApiResponse<List<TableDto>>>> GetTables([FromQuery] string dbName)
    {
        try
        {
            if (string.IsNullOrEmpty(dbName))
            {
                return BadRequest(new ApiResponse<List<TableDto>>
                {
                    Success = false,
                    Message = "資料庫名稱不能為空"
                });
            }

            var result = await _service.GetTablesAsync(dbName);
            return Ok(new ApiResponse<List<TableDto>>
            {
                Success = true,
                Data = result,
                Message = "查詢成功"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<TableDto>>
            {
                Success = false,
                Message = $"查詢失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 查詢欄位列表
    /// </summary>
    [HttpGet("fields")]
    public async Task<ActionResult<ApiResponse<List<FieldDto>>>> GetFields([FromQuery] string dbName, [FromQuery] string tableName)
    {
        try
        {
            if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(tableName))
            {
                return BadRequest(new ApiResponse<List<FieldDto>>
                {
                    Success = false,
                    Message = "資料庫名稱和表格名稱不能為空"
                });
            }

            var result = await _service.GetFieldsAsync(dbName, tableName);
            return Ok(new ApiResponse<List<FieldDto>>
            {
                Success = true,
                Data = result,
                Message = "查詢成功"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<FieldDto>>
            {
                Success = false,
                Message = $"查詢失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 查詢角色欄位權限列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<RoleFieldPermissionDto>>>> GetPermissions([FromQuery] RoleFieldPermissionQueryDto query)
    {
        try
        {
            var result = await _service.GetPermissionsAsync(query);
            return Ok(new ApiResponse<PagedResult<RoleFieldPermissionDto>>
            {
                Success = true,
                Data = result,
                Message = "查詢成功"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<PagedResult<RoleFieldPermissionDto>>
            {
                Success = false,
                Message = $"查詢失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 新增角色欄位權限
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreatePermission([FromBody] CreateRoleFieldPermissionDto dto)
    {
        try
        {
            var id = await _service.CreatePermissionAsync(dto);
            return Ok(new ApiResponse<Guid>
            {
                Success = true,
                Data = id,
                Message = "新增成功"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<Guid>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<Guid>
            {
                Success = false,
                Message = $"新增失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 修改角色欄位權限
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> UpdatePermission(Guid id, [FromBody] UpdateRoleFieldPermissionDto dto)
    {
        try
        {
            var result = await _service.UpdatePermissionAsync(id, dto);
            if (result)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "修改成功"
                });
            }
            else
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "角色欄位權限不存在"
                });
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"修改失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 刪除角色欄位權限
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeletePermission(Guid id)
    {
        try
        {
            var result = await _service.DeletePermissionAsync(id);
            if (result)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "刪除成功"
                });
            }
            else
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "角色欄位權限不存在"
                });
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"刪除失敗: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// 批次設定角色欄位權限
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<ApiResponse<int>>> BatchSetPermissions([FromBody] BatchSetRoleFieldPermissionDto dto)
    {
        try
        {
            var count = await _service.BatchSetPermissionsAsync(dto);
            return Ok(new ApiResponse<int>
            {
                Success = true,
                Data = count,
                Message = "批次設定成功"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<int>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<int>
            {
                Success = false,
                Message = $"批次設定失敗: {ex.Message}"
            });
        }
    }
}

