#!/bin/bash
# ErpCore Migration 腳本 (Linux/Mac)
# 用途: 執行資料庫 Migration

set -e

if [ $# -lt 1 ]; then
    echo "用法: $0 <Action> [MigrationName] [ConnectionString]"
    echo "Action: Add, Update, Remove, List"
    exit 1
fi

ACTION="$1"
MIGRATION_NAME="${2:-}"
CONNECTION_STRING="${3:-}"

echo "========================================="
echo "ErpCore Migration 腳本"
echo "========================================="
echo ""

echo "操作: $ACTION"
if [ -n "$MIGRATION_NAME" ]; then
    echo "Migration 名稱: $MIGRATION_NAME"
fi
echo ""

# 設定路徑
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
MIGRATIONS_PATH="$SCRIPT_DIR/../database/Migrations"

echo "Migration 路徑: $MIGRATIONS_PATH"
echo ""

# 根據操作執行對應的命令
case "$ACTION" in
    "Add")
        if [ -z "$MIGRATION_NAME" ]; then
            echo "錯誤: 新增 Migration 需要指定名稱"
            exit 1
        fi
        echo "新增 Migration: $MIGRATION_NAME"
        # 這裡可以執行 dotnet ef migrations add 命令
        ;;
    "Update")
        echo "更新資料庫..."
        # 這裡可以執行 dotnet ef database update 命令
        ;;
    "Remove")
        if [ -z "$MIGRATION_NAME" ]; then
            echo "錯誤: 移除 Migration 需要指定名稱"
            exit 1
        fi
        echo "移除 Migration: $MIGRATION_NAME"
        # 這裡可以執行 dotnet ef migrations remove 命令
        ;;
    "List")
        echo "列出所有 Migration..."
        # 這裡可以執行 dotnet ef migrations list 命令
        ;;
    *)
        echo "未知的操作: $ACTION"
        exit 1
        ;;
esac

echo ""
echo "========================================="
echo "Migration 操作完成!"
echo "========================================="

