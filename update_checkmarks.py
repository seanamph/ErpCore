#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
更新 ErpCore_專案目錄結構.md 中的打勾狀態
根據實際存在的文件自動更新 ✅ 標記
"""

import os
import re
from pathlib import Path

# 專案根目錄
PROJECT_ROOT = Path("ErpCore")
STRUCTURE_FILE = Path("ErpCore_專案目錄結構.md")

def normalize_path(path_str):
    """正規化路徑，處理不同格式的檔案路徑"""
    # 移除前綴如 src/、ErpCore/ 等
    path_str = path_str.strip()
    # 移除開頭的 src/、ErpCore/ 等
    if path_str.startswith("src/"):
        path_str = path_str[4:]
    if path_str.startswith("ErpCore/"):
        path_str = path_str[8:]
    # 移除開頭的 /
    if path_str.startswith("/"):
        path_str = path_str[1:]
    return path_str

def extract_file_path(line):
    """從文件結構行中提取檔案路徑"""
    # 匹配格式：│   ├── FileName.cs # ✅ 或 │   └── FileName.cs
    match = re.search(r'├──\s+([^\s#]+)|└──\s+([^\s#]+)', line)
    if match:
        file_name = match.group(1) or match.group(2)
        return file_name.strip()
    return None

def check_file_exists(file_path, base_path):
    """檢查檔案是否存在"""
    full_path = base_path / file_path
    return full_path.exists()

def update_checkmarks():
    """更新文件結構中的打勾狀態"""
    if not STRUCTURE_FILE.exists():
        print(f"錯誤：找不到文件 {STRUCTURE_FILE}")
        return
    
    print(f"讀取文件：{STRUCTURE_FILE}")
    with open(STRUCTURE_FILE, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    
    updated_lines = []
    updated_count = 0
    
    for i, line in enumerate(lines):
        original_line = line
        
        # 檢查是否包含檔案名稱（.cs, .vue, .js, .sql, .md 等）
        if re.search(r'\.(cs|vue|js|ts|sql|md|json|config|props|targets|sln|csproj|html|scss|css)$', line, re.IGNORECASE):
            # 提取檔案名稱
            file_name = extract_file_path(line)
            
            if file_name:
                # 構建可能的檔案路徑
                # 從當前行的縮排推斷目錄結構
                indent_level = len(line) - len(line.lstrip())
                
                # 向上查找目錄結構
                current_dir = None
                for j in range(i - 1, max(0, i - 50), -1):
                    prev_line = lines[j]
                    # 查找目錄定義
                    if '├──' in prev_line or '└──' in prev_line:
                        if not re.search(r'\.(cs|vue|js|ts|sql|md|json|config|props|targets|sln|csproj|html|scss|css)$', prev_line, re.IGNORECASE):
                            # 這是一個目錄
                            dir_name = extract_file_path(prev_line)
                            if dir_name:
                                current_dir = dir_name
                                break
                
                # 構建完整路徑
                possible_paths = []
                
                # 嘗試不同的路徑組合
                if current_dir:
                    possible_paths.append(f"{current_dir}/{file_name}")
                
                # 直接嘗試檔案名稱
                possible_paths.append(file_name)
                
                # 嘗試在常見目錄中查找
                common_dirs = [
                    "src/ErpCore.Api",
                    "src/ErpCore.Application",
                    "src/ErpCore.Domain",
                    "src/ErpCore.Infrastructure",
                    "src/ErpCore.Shared",
                    "src/ErpCore.Web",
                    "database/Scripts",
                    "docs",
                    "scripts",
                    "docker"
                ]
                
                for common_dir in common_dirs:
                    if current_dir:
                        possible_paths.append(f"{common_dir}/{current_dir}/{file_name}")
                    possible_paths.append(f"{common_dir}/{file_name}")
                
                # 檢查檔案是否存在
                file_exists = False
                for path in possible_paths:
                    normalized = normalize_path(path)
                    if check_file_exists(normalized, PROJECT_ROOT):
                        file_exists = True
                        break
                
                # 更新打勾狀態
                if file_exists and '✅' not in line:
                    # 在檔案名稱後添加 ✅
                    line = re.sub(r'(├──\s+|└──\s+)([^\s#]+)', r'\1\2 ✅', line, count=1)
                    updated_count += 1
                    print(f"已更新：{file_name}")
                elif not file_exists and '✅' in line and '# ✅' not in line:
                    # 如果檔案不存在但已有 ✅，保留註解中的 ✅
                    pass
        
        updated_lines.append(line)
    
    # 寫回文件
    if updated_count > 0:
        with open(STRUCTURE_FILE, 'w', encoding='utf-8') as f:
            f.writelines(updated_lines)
        print(f"\n完成！已更新 {updated_count} 個檔案的打勾狀態")
    else:
        print("\n沒有需要更新的檔案")

if __name__ == "__main__":
    update_checkmarks()

