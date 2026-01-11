#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""测试 Python 环境和依赖"""

import sys

print("Python 版本:", sys.version)
print("Python 路径:", sys.executable)
print("\n检查依赖模块...")

try:
    import pyautogui
    print("✓ pyautogui 已安装")
except ImportError as e:
    print("✗ pyautogui 未安装:", e)
    print("  安装命令: pip install pyautogui")

try:
    from PIL import Image
    print("✓ PIL (Pillow) 已安装")
except ImportError as e:
    print("✗ PIL (Pillow) 未安装:", e)
    print("  安装命令: pip install Pillow")

try:
    import numpy as np
    print("✓ numpy 已安装")
except ImportError as e:
    print("✗ numpy 未安装:", e)
    print("  安装命令: pip install numpy")

try:
    import cv2
    print("✓ opencv-python 已安装")
except ImportError as e:
    print("⚠ opencv-python 未安装（可选）:", e)
    print("  安装命令: pip install opencv-python")

try:
    import win32api
    print("✓ pywin32 已安装")
except ImportError as e:
    print("⚠ pywin32 未安装（可选）:", e)
    print("  安装命令: pip install pywin32")

print("\n检查图片文件...")
import os

image_files = ['a.png', 'a.PNG', 'b.PNG', 'c.PNG', 'd.PNG', 'e.PNG']
for img in image_files:
    if os.path.exists(img):
        print(f"✓ {img} 存在")
    else:
        print(f"✗ {img} 不存在")

print("\n当前工作目录:", os.getcwd())
