#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""调试 auto_clicker.py"""

import sys
import os

print("=" * 60)
print("调试信息")
print("=" * 60)
print(f"Python 版本: {sys.version}")
print(f"Python 路径: {sys.executable}")
print(f"当前目录: {os.getcwd()}")
print()

# 检查图片文件
print("检查图片文件...")
image_files = {
    'a.png': ['a.png', 'a.PNG'],
    'b.PNG': ['b.PNG', 'b.png'],
    'c.PNG': ['c.PNG', 'c.png'],
    'd.PNG': ['d.PNG', 'd.png'],
    'e.PNG': ['e.PNG', 'e.png']
}

for key, variants in image_files.items():
    found = False
    for variant in variants:
        if os.path.exists(variant):
            print(f"✓ {key}: 找到 {variant}")
            found = True
            break
    if not found:
        print(f"✗ {key}: 未找到任何变体")

print()

# 尝试导入模块
print("检查 Python 模块...")
try:
    import pyautogui
    print("✓ pyautogui")
except Exception as e:
    print(f"✗ pyautogui: {e}")

try:
    from PIL import Image
    print("✓ PIL")
except Exception as e:
    print(f"✗ PIL: {e}")

try:
    import numpy
    print("✓ numpy")
except Exception as e:
    print(f"✗ numpy: {e}")

print()

# 尝试导入 auto_clicker
print("尝试导入 auto_clicker...")
try:
    import auto_clicker
    print("✓ 导入成功")
    print()
    print("尝试创建 AutoClicker 实例...")
    try:
        clicker = auto_clicker.AutoClicker()
        print(f"✓ 创建成功，running = {clicker.running}")
        if not clicker.running:
            print("⚠ 程序未运行，可能缺少必要的图片文件")
    except Exception as e:
        print(f"✗ 创建失败: {e}")
        import traceback
        traceback.print_exc()
except Exception as e:
    print(f"✗ 导入失败: {e}")
    import traceback
    traceback.print_exc()

print()
print("=" * 60)
