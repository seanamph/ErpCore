#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
簡單的腳本來更新 ErpCore_專案目錄結構.md 中的打勾狀態
根據實際存在的文件自動更新 ✅ 標記
"""

import os
import re
from pathlib import Path

# 專案根目錄
PROJECT_ROOT = Path("ErpCore")
STRUCTURE_FILE = Path("ErpCore_專案目錄結構.md")

def check_file_exists(file_path, project_root):
    """檢查文件是否存在"""
    # 嘗試多種路徑格式
    possible_paths = [
        project_root / file_path,
        Path(file_path),
        project_root / "src" / file_path,
        project_root / "database" / file_path,
        project_root / "docs" / file_path,
        project_root / "scripts" / file_path,
        project_root / "docker" / file_path,
    ]
    
    for path in possible_paths:
        if path.exists() and path.is_file():
            return True
    return False

def extract_file_info(line):
    """從文件結構行中提取檔案資訊"""
    # 匹配格式：│   ├── FileName.cs # ✅ 或 │   └── FileName.cs # 說明
    match = re.search(r'(?:├──|└──)\s+([^\s#]+)', line)
    if match:
        filename = match.group(1).strip()
        # 嘗試提取路徑上下文
        # 從前面的行中提取目錄結構
        return filename
    return None

def update_checkmarks():
    """更新文件結構中的打勾狀態"""
    if not STRUCTURE_FILE.exists():
        print(f"錯誤：找不到文件 {STRUCTURE_FILE}")
        return
    
    if not PROJECT_ROOT.exists():
        print(f"錯誤：找不到專案目錄 {PROJECT_ROOT}")
        return
    
    # 建立所有實際存在的文件集合
    existing_files = set()
    for root, dirs, files in os.walk(PROJECT_ROOT):
        # 跳過不需要的目錄
        dirs[:] = [d for d in dirs if d not in ['.git', '__pycache__', 'node_modules', 'bin', 'obj', '.vs']]
        for file in files:
            rel_path = os.path.relpath(os.path.join(root, file), PROJECT_ROOT)
            existing_files.add(rel_path.replace('\\', '/'))
            existing_files.add(file)  # 也添加文件名
    
    print(f"已掃描 {len(existing_files)} 個實際存在的檔案")
    
    with open(STRUCTURE_FILE, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    
    updated_lines = []
    updated_count = 0
    current_path_context = []
    
    for i, line in enumerate(lines):
        original_line = line
        
        # 更新路徑上下文
        if '├──' in line or '└──' in line:
            # 提取目錄層級
            indent_level = len(line) - len(line.lstrip())
            # 簡化：只保留文件名匹配
        
        # 檢查是否包含檔案名稱
        filename = extract_file_info(line)
        if filename:
            # 檢查檔案是否存在
            file_exists = False
            
            # 方法1：檢查完整路徑
            for existing in existing_files:
                if existing.endswith(filename) or existing == filename:
                    file_exists = True
                    break
            
            # 方法2：檢查相對路徑
            if not file_exists:
                # 嘗試構建可能的相對路徑
                possible_paths = [
                    filename,
                    f"src/ErpCore.Api/{filename}",
                    f"src/ErpCore.Application/{filename}",
                    f"src/ErpCore.Domain/{filename}",
                    f"src/ErpCore.Infrastructure/{filename}",
                    f"src/ErpCore.Shared/{filename}",
                    f"src/ErpCore.Web/{filename}",
                    f"database/Scripts/{filename}",
                    f"docs/{filename}",
                    f"scripts/{filename}",
                    f"docker/{filename}",
                ]
                
                for path in possible_paths:
                    if path in existing_files:
                        file_exists = True
                        break
            
            # 更新打勾狀態
            if file_exists:
                # 如果行中沒有 ✅，添加 ✅
                if '✅' not in line:
                    # 在檔案名稱後添加 ✅
                    line = re.sub(r'((?:├──|└──)\s+[^\s#]+)', r'\1 ✅', line, count=1)
                    updated_count += 1
                    if updated_count <= 20:  # 只顯示前20個
                        print(f"已更新：{filename}")
        
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

