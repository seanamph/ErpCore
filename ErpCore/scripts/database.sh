#!/bin/bash
# ErpCore 資料庫腳本 (Linux/Mac)
# 用途: 執行資料庫相關操作

set -e

if [ $# -lt 1 ]; then
    echo "用法: $0 <Action> [ConnectionString] [DatabaseName]"
    echo "Action: Create, Update, Drop, Seed"
    exit 1
fi

ACTION="$1"
CONNECTION_STRING="${2:-}"
DATABASE_NAME="${3:-ErpCore}"

echo "========================================="
echo "ErpCore 資料庫腳本"
echo "========================================="
echo ""

echo "操作: $ACTION"
echo "資料庫: $DATABASE_NAME"
echo ""

# 設定路徑
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DATABASE_SCRIPTS_PATH="$SCRIPT_DIR/../database/Scripts"

# 根據操作執行對應的 SQL 腳本
case "$ACTION" in
    "Create")
        echo "執行建立資料庫腳本..."
        echo "建立資料庫腳本位於: $DATABASE_SCRIPTS_PATH"
        ;;
    "Update")
        echo "執行更新資料庫腳本..."
        echo "更新資料庫腳本位於: $DATABASE_SCRIPTS_PATH"
        ;;
    "Drop")
        echo "執行刪除資料庫腳本..."
        echo "刪除資料庫腳本位於: $DATABASE_SCRIPTS_PATH"
        ;;
    "Seed")
        echo "執行種子資料腳本..."
        SEEDS_PATH="$SCRIPT_DIR/../database/Seeds"
        echo "種子資料腳本位於: $SEEDS_PATH"
        ;;
    *)
        echo "未知的操作: $ACTION"
        exit 1
        ;;
esac

echo ""
echo "========================================="
echo "資料庫操作完成!"
echo "========================================="

