#!/bin/bash
# ErpCore 建置腳本 (Linux/Mac)
# 用途: 建置整個 ErpCore Solution

set -e

CONFIGURATION="${1:-Release}"
CLEAN="${2:-false}"

echo "========================================="
echo "ErpCore 建置腳本"
echo "========================================="
echo ""

# 設定路徑
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_PATH="$SCRIPT_DIR/../ErpCore.sln"

echo "Solution 路徑: $SOLUTION_PATH"
echo "建置設定: $CONFIGURATION"
echo ""

# 清理 (如果指定)
if [ "$CLEAN" = "true" ]; then
    echo "清理舊的建置檔案..."
    dotnet clean "$SOLUTION_PATH" --configuration "$CONFIGURATION"
    if [ $? -ne 0 ]; then
        echo "清理失敗!"
        exit 1
    fi
    echo "清理完成"
    echo ""
fi

# 還原 NuGet 套件
echo "還原 NuGet 套件..."
dotnet restore "$SOLUTION_PATH"
if [ $? -ne 0 ]; then
    echo "還原失敗!"
    exit 1
fi
echo "還原完成"
echo ""

# 建置 Solution
echo "建置 Solution..."
dotnet build "$SOLUTION_PATH" --configuration "$CONFIGURATION" --no-restore
if [ $? -ne 0 ]; then
    echo "建置失敗!"
    exit 1
fi
echo "建置完成"
echo ""

echo "========================================="
echo "建置成功完成!"
echo "========================================="

