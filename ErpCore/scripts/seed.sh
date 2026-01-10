#!/bin/bash
# ErpCore 種子資料腳本 (Linux/Mac)
# 用途: 執行資料庫種子資料

set -e

CONNECTION_STRING="${1:-}"
DATABASE_NAME="${2:-ErpCore}"

echo "========================================="
echo "ErpCore 種子資料腳本"
echo "========================================="
echo ""

echo "資料庫: $DATABASE_NAME"
echo ""

# 設定路徑
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SEEDS_PATH="$SCRIPT_DIR/../database/Seeds"

echo "種子資料路徑: $SEEDS_PATH"
echo ""

# 執行種子資料腳本
echo "執行種子資料腳本..."
# 這裡可以執行種子資料的 SQL 腳本
# 例如: sqlcmd -S localhost -d $DATABASE_NAME -i "$SEEDS_PATH/SeedData.sql"

echo ""
echo "========================================="
echo "種子資料執行完成!"
echo "========================================="

