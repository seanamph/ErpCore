#!/bin/bash
# ErpCore 部署腳本 (Linux/Mac)
# 用途: 部署 ErpCore 應用程式

set -e

ENVIRONMENT="${1:-Production}"
CONFIGURATION="${2:-Release}"

echo "========================================="
echo "ErpCore 部署腳本"
echo "========================================="
echo ""

echo "部署環境: $ENVIRONMENT"
echo "建置設定: $CONFIGURATION"
echo ""

# 設定路徑
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_PROJECT_PATH="$SCRIPT_DIR/../src/ErpCore.Api/ErpCore.Api.csproj"

# 建置專案
echo "建置專案..."
bash "$SCRIPT_DIR/build.sh" "$CONFIGURATION" "false"
if [ $? -ne 0 ]; then
    echo "建置失敗!"
    exit 1
fi

# 發佈 API 專案
echo "發佈 API 專案..."
PUBLISH_PATH="$SCRIPT_DIR/../publish/ErpCore.Api"
dotnet publish "$API_PROJECT_PATH" --configuration "$CONFIGURATION" --output "$PUBLISH_PATH" --no-build
if [ $? -ne 0 ]; then
    echo "發佈失敗!"
    exit 1
fi
echo "發佈完成: $PUBLISH_PATH"
echo ""

echo "========================================="
echo "部署成功完成!"
echo "========================================="

