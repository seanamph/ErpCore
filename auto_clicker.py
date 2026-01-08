#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
自動點擊程式
循環偵測圖案並執行點擊操作：
1. 偵測到 a.png -> 點擊固定座標 (1260, 100)
2. 偵測到 b.PNG -> 點擊 b.PNG 座標 + (10, 10)
3. 偵測到 c.PNG -> 點擊 c.PNG 座標 + (50, 10)
4. 回到步驟 1，循環執行
"""

import pyautogui
import time
import os
import math
import json
from PIL import Image
from datetime import datetime
import numpy as np

# 檢測是否支援 confidence 參數（需要 OpenCV）
try:
    import cv2
    HAS_OPENCV = True
except ImportError:
    HAS_OPENCV = False

# 檢測是否支援 Windows API（用於檢測滑鼠指標類型）
try:
    import win32api
    import win32con
    HAS_WIN32 = True
except ImportError:
    HAS_WIN32 = False

class AutoClicker:
    def __init__(self):
        # 設定 pyautogui 的安全設定
        pyautogui.FAILSAFE = True  # 滑鼠移到螢幕角落可觸發異常
        pyautogui.PAUSE = 0.1  # 每次操作間隔 0.1 秒
        
        # 獲取螢幕尺寸
        screen_size = pyautogui.size()
        self.screen_width = screen_size.width
        self.screen_height = screen_size.height
        
        # 座標緩存文件（修改原因：記錄已確認的座標，下次直接使用，不再搜索）
        self.coord_cache_file = 'coordinates_cache.json'
        self.coord_cache = self.load_coord_cache()
        
        # 檢查 OpenCV 支援
        self.has_opencv = HAS_OPENCV
        if not self.has_opencv:
            print("⚠ 警告: 未安裝 OpenCV，將使用基本圖像匹配（不支援相似度匹配）")
            print("   建議安裝 OpenCV 以支援 90% 相似度匹配和不同大小的圖像:")
            print("   pip install opencv-python")
            print("   目前可能無法正確偵測到圖像！")
        else:
            print("✓ OpenCV 已安裝，支援 90% 相似度匹配和不同大小的圖像")
        
        # 圖片檔案路徑
        self.image_a = self._find_image_file('a.png')
        self.image_b = self._find_image_file('b.PNG')
        self.image_c = self._find_image_file('c.PNG')
        self.image_d = self._find_image_file('d.PNG')
        self.image_e = self._find_image_file('e.PNG')
        
        # 定義初始搜尋區域 (left, top, width, height)
        # 修改原因：根據圖片在屏幕上的位置動態計算搜索區域
        # a.PNG 和 d.PNG：通常在屏幕右下角
        # b.PNG：在屏幕右上角，但有可能偏下 1/3 vh 處
        # c.PNG：在屏幕中間
        self.region_a = self.get_a_region()  # 右下角區域
        self.region_b = self.get_b_region()  # 右上角區域（可能偏下 1/3 vh）
        self.region_c = self.get_c_region()  # 屏幕中間區域
        
        # 記錄初始區域（用於重置）
        self.initial_region_a = self.region_a
        self.initial_region_b = self.region_b
        self.initial_region_c = self.region_c
        
        # 注意：已改為只點擊 e.PNG 左下角區域，不再使用保存的固定座標
        
        # 注意：已改為使用 e.PNG 來觸發 b.PNG 出現，不再使用 b_pre_click_offset
        # 保留此變量以保持向後兼容（如果未來需要）
        self.b_pre_click_offset = (-30, -30)  # 已棄用，改為使用 e.PNG
        
        # 保存上次檢測到的 b.PNG 位置（用於重試時重新點擊）
        self.last_b_position = None
        
        # 檢查所有圖片檔案是否存在
        self.running = True
        missing_files = []
        
        if not self.image_a:
            missing_files.append('a.png')
        if not self.image_b:
            missing_files.append('b.PNG')
        if not self.image_c:
            missing_files.append('c.PNG')
        
        if missing_files:
            print(f"錯誤: 找不到以下圖片檔案: {', '.join(missing_files)}")
            print(f"請確認檔案存在於當前目錄: {os.getcwd()}")
            self.running = False
            return
        
        print("=" * 60)
        print("自動點擊程式啟動 (高精準模式)")
        print("=" * 60)
        print(f"圖片檔案:")
        print(f"  - a.png: {self.image_a}")
        print(f"  - b.PNG: {self.image_b}")
        print(f"  - c.PNG: {self.image_c}")
        if self.image_d:
            print(f"  - d.PNG: {self.image_d} (可選，用於檢測是否跳過步驟 3)")
        if self.image_e:
            print(f"  - e.PNG: {self.image_e} (用於觸發 b.PNG 出現)")
        print("=" * 60)
        print("搜尋區域 (根據圖片位置動態計算):")
        print(f"  - a.png / d.PNG: 右下角區域")
        print(f"    區域: ({self.region_a[0]}, {self.region_a[1]}) ~ ({self.region_a[0]+self.region_a[2]}, {self.region_a[1]+self.region_a[3]})")
        print(f"    大小: {self.region_a[2]} x {self.region_a[3]} 像素")
        print(f"  - b.PNG: 右上角區域（可能偏下 1/3 vh）")
        print(f"    區域: ({self.region_b[0]}, {self.region_b[1]}) ~ ({self.region_b[0]+self.region_b[2]}, {self.region_b[1]+self.region_b[3]})")
        print(f"    大小: {self.region_b[2]} x {self.region_b[3]} 像素")
        print(f"  - c.PNG: 屏幕中間區域")
        print(f"    區域: ({self.region_c[0]}, {self.region_c[1]}) ~ ({self.region_c[0]+self.region_c[2]}, {self.region_c[1]+self.region_c[3]})")
        print(f"    大小: {self.region_c[2]} x {self.region_c[3]} 像素")
        if self.image_e:
            e_region = self.get_e_region()
            if e_region:
                print(f"  - e.PNG: 右上角區域")
                print(f"    區域: ({e_region[0]}, {e_region[1]}) ~ ({e_region[0]+e_region[2]}, {e_region[1]+e_region[3]})")
                print(f"    大小: {e_region[2]} x {e_region[3]} 像素")
        print("=" * 60)
        print("執行流程:")
        print("  1. 等待 a.png (未執行狀態) -> 點擊 a.png 中心座標 (1891, 981)")
        print("  2. 等待 e.PNG -> 點擊 e.PNG 座標左下 10px -> 等待 b.PNG 出現")
        print("  2.1. 等待 b.PNG -> 點擊 b.PNG 座標 + (-1, -2) -> 等待 5 秒")
        print("  2.5. 檢查 a 區域是否出現 d.PNG (已執行狀態)")
        print("      - 如果出現 d: 表示上一輪還在執行，等待 d 變成 a")
        print("      - 如果未出現 d: 繼續執行步驟 3")
        print("  3. 等待 c.PNG -> 點擊 c.PNG 座標 + (10, 5)")
        print("  3.5. 等待 d.PNG 出現（確認執行成功）")
        print("  3.6. 等待 d.PNG 變成 a.PNG（準備下一輪）")
        print("  4. 回到步驟 1，循環執行")
        print("=" * 60)
        print("精準度設定 (高精準模式):")
        print("  - a.png / d.PNG: 98% 相似度 + 5% 大小容差 (最嚴格，避免混淆)")
        print("  - b.PNG / c.PNG: 95% 相似度 + 10% 大小容差 (高精準)")
        print("大小驗證: 啟用（嚴格驗證匹配區域大小，避免誤判）")
        print("座標緩存: 啟用（已確認的座標會保存，下次直接使用）")
        print(f"注意: 已改為使用 e.PNG 來觸發 b.PNG 出現（點擊 e.PNG 左下 10px）")
        print("=" * 60)
        if HAS_WIN32:
            print("✓ Windows API 已載入，支援檢測滑鼠指標類型")
        else:
            print("⚠ 警告: 未安裝 pywin32，無法檢測滑鼠指標類型")
            print("   建議安裝: pip install pywin32")
        print("=" * 60)
        print("程式運行中... (按 Ctrl+C 停止)")
        print("將滑鼠移到螢幕角落可緊急停止")
        print("-" * 60)
    
    def _find_image_file(self, image_path):
        """尋找圖片檔案（不區分大小寫）"""
        if os.path.exists(image_path):
            return image_path
        
        # 嘗試尋找不同大小寫的檔案
        dir_path = os.path.dirname(image_path) if os.path.dirname(image_path) else '.'
        if os.path.exists(dir_path):
            for file in os.listdir(dir_path):
                if file.lower() == image_path.lower():
                    return os.path.join(dir_path, file)
        
        return None
    
    def load_coord_cache(self):
        """載入座標緩存
        
        修改原因：記錄已確認的座標，下次直接使用，不再搜索
        
        Returns:
            dict: 座標緩存字典，格式：
                {
                    'a.png': {'x': 1220, 'y': 111, 'verified': True, 'timestamp': '2026-01-08 15:30:00'},
                    'b.PNG': {'x': 1729, 'y': 104, 'verified': True, 'timestamp': '2026-01-08 15:30:05'},
                    'c.PNG': {'x': 693, 'y': 312, 'verified': True, 'timestamp': '2026-01-08 15:30:10'},
                    'd.PNG': {'x': 1304, 'y': 797, 'verified': True, 'timestamp': '2026-01-08 15:30:15'}
                }
        """
        try:
            if os.path.exists(self.coord_cache_file):
                with open(self.coord_cache_file, 'r', encoding='utf-8') as f:
                    cache = json.load(f)
                print(f"✓ 已載入座標緩存: {self.coord_cache_file}")
                for img_name, coord in cache.items():
                    if coord.get('verified'):
                        print(f"  - {img_name}: ({coord['x']}, {coord['y']}) [已驗證於 {coord.get('timestamp', 'N/A')}]")
                return cache
            else:
                print(f"ℹ 座標緩存文件不存在，將建立新的緩存")
                return {}
        except Exception as e:
            print(f"⚠ 載入座標緩存失敗: {e}，使用空緩存")
            return {}
    
    def save_coord_cache(self):
        """保存座標緩存到文件
        
        修改原因：持久化保存已確認的座標，下次運行時直接使用
        """
        try:
            with open(self.coord_cache_file, 'w', encoding='utf-8') as f:
                json.dump(self.coord_cache, f, ensure_ascii=False, indent=2)
            print(f"  → 座標緩存已保存: {self.coord_cache_file}")
        except Exception as e:
            print(f"  ⚠ 保存座標緩存失敗: {e}")
    
    def update_coord_cache(self, image_name, x, y, verified=True):
        """更新座標緩存
        
        修改原因：記錄驗證通過的座標，供下次使用
        
        Args:
            image_name: 圖片名稱（如 'a.png', 'b.PNG', 'c.PNG', 'd.PNG'）
            x: X 座標
            y: Y 座標
            verified: 是否已驗證通過
        """
        self.coord_cache[image_name] = {
            'x': int(x),
            'y': int(y),
            'verified': verified,
            'timestamp': datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        }
        self.save_coord_cache()
        print(f"  ✓ 已更新 {image_name} 座標緩存: ({x}, {y})")
    
    def update_region(self, location, region_name, expand_pixels=50):
        """根據檢測到的位置更新搜尋區域
        
        Args:
            location: pyautogui 的 location 物件（包含 left, top, width, height）
            region_name: 區域名稱 ('a', 'b', 'c', 'd')
            expand_pixels: 擴展像素數（往四個方向各擴展）
        """
        # 計算新的區域：以檢測到的位置為中心，往四個方向各擴展
        new_left = max(0, location.left - expand_pixels)
        new_top = max(0, location.top - expand_pixels)
        new_width = location.width + (expand_pixels * 2)
        new_height = location.height + (expand_pixels * 2)
        
        new_region = (new_left, new_top, new_width, new_height)
        
        # 更新對應的區域
        if region_name == 'a':
            old_region = self.region_a
            self.region_a = new_region
            print(f"  → 更新 a.png 搜尋區域: ({old_region[0]}, {old_region[1]}) ~ ({old_region[0]+old_region[2]}, {old_region[1]+old_region[3]})")
            print(f"     新區域: ({new_region[0]}, {new_region[1]}) ~ ({new_region[0]+new_region[2]}, {new_region[1]+new_region[3]})")
        elif region_name == 'b':
            old_region = self.region_b
            self.region_b = new_region
            print(f"  → 更新 b.PNG 搜尋區域: ({old_region[0]}, {old_region[1]}) ~ ({old_region[0]+old_region[2]}, {old_region[1]+old_region[3]})")
            print(f"     新區域: ({new_region[0]}, {new_region[1]}) ~ ({new_region[0]+new_region[2]}, {new_region[1]+new_region[3]})")
        elif region_name == 'c':
            old_region = self.region_c
            self.region_c = new_region
            print(f"  → 更新 c.PNG 搜尋區域: ({old_region[0]}, {old_region[1]}) ~ ({old_region[0]+old_region[2]}, {old_region[1]+old_region[3]})")
            print(f"     新區域: ({new_region[0]}, {new_region[1]}) ~ ({new_region[0]+new_region[2]}, {new_region[1]+new_region[3]})")
        elif region_name == 'd':
            # d 使用 a 的區域，所以更新 a 的區域
            old_region = self.region_a
            self.region_a = new_region
            print(f"  → 更新 d.PNG (使用 a 區域) 搜尋區域: ({old_region[0]}, {old_region[1]}) ~ ({old_region[0]+old_region[2]}, {old_region[1]+old_region[3]})")
            print(f"     新區域: ({new_region[0]}, {new_region[1]}) ~ ({new_region[0]+new_region[2]}, {new_region[1]+new_region[3]})")
    
    def get_image_size(self, image_path):
        """獲取圖片尺寸"""
        try:
            with Image.open(image_path) as img:
                return img.size  # 返回 (width, height)
        except Exception as e:
            print(f"  警告: 無法讀取圖片尺寸 {image_path}: {e}")
            return None
    
    def get_a_region(self):
        """計算 a.PNG 和 d.PNG 的搜索區域（屏幕右下角區域）
        
        Returns:
            tuple: (left, top, width, height) 搜索區域
        """
        # 獲取 a.PNG 的尺寸（如果存在）
        a_size = None
        if self.image_a:
            a_size = self.get_image_size(self.image_a)
        
        if a_size:
            a_width, a_height = a_size
            # 搜索區域：以 a.PNG 尺寸的 5 倍為範圍
            search_width = a_width * 5
            search_height = a_height * 5
        else:
            # 如果無法獲取尺寸，使用默認大小
            search_width = 400
            search_height = 400
        
        # 確保搜索區域至少覆蓋右下角的一定範圍
        min_region_width = min(self.screen_width // 3, 400)
        min_region_height = min(self.screen_height // 3, 400)
        search_width = max(search_width, min_region_width)
        search_height = max(search_height, min_region_height)
        
        # 右下角區域：從屏幕右邊開始，從底部開始
        left = max(0, self.screen_width - search_width)
        top = max(0, self.screen_height - search_height)
        width = min(search_width, self.screen_width - left)
        height = min(search_height, self.screen_height - top)
        
        return (left, top, width, height)
    
    def get_b_region(self):
        """計算 b.PNG 的搜索區域（屏幕右上角區域，但有可能偏下 1/3 vh 處）
        
        Returns:
            tuple: (left, top, width, height) 搜索區域
        """
        # 獲取 b.PNG 的尺寸（如果存在）
        b_size = None
        if self.image_b:
            b_size = self.get_image_size(self.image_b)
        
        if b_size:
            b_width, b_height = b_size
            # 搜索區域：以 b.PNG 尺寸的 5 倍為範圍
            search_width = b_width * 5
            search_height = b_height * 5
        else:
            # 如果無法獲取尺寸，使用默認大小
            search_width = 400
            search_height = 400
        
        # 確保搜索區域至少覆蓋右上角的一定範圍
        min_region_width = min(self.screen_width // 3, 400)
        min_region_height = min(self.screen_height // 3, 300)
        search_width = max(search_width, min_region_width)
        search_height = max(search_height, min_region_height)
        
        # 右上角區域：從屏幕右邊開始
        # 考慮 b.PNG 可能偏下 1/3 vh，所以從頂部開始，但高度要覆蓋到 1/3 vh 處
        left = max(0, self.screen_width - search_width)
        top = 0
        # 高度要覆蓋從頂部到 1/3 vh 處，再加上搜索高度的一半（向下延伸）
        one_third_vh = self.screen_height // 3
        # 搜索區域從頂部開始，高度要能覆蓋到 1/3 vh 處，再加上圖片尺寸的緩衝
        # 確保搜索高度至少能覆蓋到 1/3 vh 處
        search_height = max(search_height, one_third_vh + (search_height // 2))
        width = min(search_width, self.screen_width - left)
        # 限制在屏幕上半部分，但確保能覆蓋到 1/3 vh 處
        max_height = max(self.screen_height // 2, one_third_vh + 100)  # 至少覆蓋到 1/3 vh + 100 像素
        height = min(search_height, max_height)
        
        return (left, top, width, height)
    
    def get_c_region(self):
        """計算 c.PNG 的搜索區域（屏幕中間區域）
        
        Returns:
            tuple: (left, top, width, height) 搜索區域
        """
        # 獲取 c.PNG 的尺寸（如果存在）
        c_size = None
        if self.image_c:
            c_size = self.get_image_size(self.image_c)
        
        if c_size:
            c_width, c_height = c_size
            # 搜索區域：以 c.PNG 尺寸的 5 倍為範圍
            search_width = c_width * 5
            search_height = c_height * 5
        else:
            # 如果無法獲取尺寸，使用默認大小
            search_width = 600
            search_height = 400
        
        # 確保搜索區域至少覆蓋中間區域的一定範圍
        min_region_width = min(self.screen_width // 2, 600)
        min_region_height = min(self.screen_height // 2, 400)
        search_width = max(search_width, min_region_width)
        search_height = max(search_height, min_region_height)
        
        # 屏幕中間區域：從屏幕中心開始，向四周擴展
        left = max(0, (self.screen_width - search_width) // 2)
        top = max(0, (self.screen_height - search_height) // 2)
        width = min(search_width, self.screen_width - left)
        height = min(search_height, self.screen_height - top)
        
        return (left, top, width, height)
    
    def get_e_region(self):
        """計算 e.PNG 的搜索區域（右上角區域，根據 e.PNG 尺寸的 5 倍）
        
        Returns:
            tuple: (left, top, width, height) 搜索區域，如果 e.PNG 不存在則返回 None
        """
        if not self.image_e:
            return None
        
        # 獲取 e.PNG 的尺寸
        e_size = self.get_image_size(self.image_e)
        if not e_size:
            # 如果無法獲取尺寸，使用默認的右上角區域（屏幕寬度的右半部分，高度的上半部分）
            region_width = self.screen_width // 2
            region_height = self.screen_height // 2
            left = self.screen_width - region_width
            top = 0
            return (left, top, region_width, region_height)
        
        e_width, e_height = e_size
        
        # 計算搜索區域：以 e.PNG 尺寸的 5 倍為範圍
        search_width = e_width * 5
        search_height = e_height * 5
        
        # 右上角區域：從屏幕右邊開始，從頂部開始
        # 確保搜索區域至少覆蓋右上角的一定範圍，但不超出屏幕
        # 如果計算出的搜索範圍太小，使用默認的右上角區域
        min_region_width = min(self.screen_width // 3, 400)  # 至少覆蓋屏幕寬度的 1/3 或 400 像素
        min_region_height = min(self.screen_height // 3, 300)  # 至少覆蓋屏幕高度的 1/3 或 300 像素
        
        search_width = max(search_width, min_region_width)
        search_height = max(search_height, min_region_height)
        
        # 從屏幕右邊開始，從頂部開始
        left = max(0, self.screen_width - search_width)
        top = 0
        width = min(search_width, self.screen_width - left)
        height = min(search_height, self.screen_height // 2)  # 限制在屏幕上半部分
        
        return (left, top, width, height)
    
    def detect_image(self, image_path, region=None, confidence=0.95, return_location=False, size_tolerance=0.10):
        """偵測圖案並返回中心點座標（高精準模式）
        
        修改原因：提升精準度，避免誤判
        - a.png/d.PNG: 98% 相似度 + 5% 大小容差（最嚴格）
        - b.PNG/c.PNG: 95% 相似度 + 10% 大小容差（高精準）
        
        Args:
            image_path: 圖片檔案路徑
            region: 搜尋區域 (left, top, width, height)，None 表示全螢幕
            confidence: 匹配信心度 (0.0-1.0)，預設 0.95
            return_location: 是否返回完整的 location 物件（用於更新區域）
            size_tolerance: 大小容差（0.10 表示允許 ±10% 的大小差異）
        
        Returns:
            如果 return_location=False: 返回 (center_x, center_y) 或 None
            如果 return_location=True: 返回 (center_x, center_y, location) 或 None
        """
        try:
            # 獲取模板圖片尺寸
            template_size = self.get_image_size(image_path)
            
            # 根據圖片名稱調整相似度和大小容差（提升精準度）
            image_name = os.path.basename(image_path).lower()
            if 'a.png' in image_name or 'd.png' in image_name:
                # a.png 和 d.PNG 使用最高相似度和最嚴格的大小驗證
                actual_confidence = 0.98  # 98% 相似度
                actual_size_tolerance = 0.05  # 5% 大小容差
            else:
                # b.PNG 和 c.PNG 使用高相似度
                actual_confidence = max(confidence, 0.95)  # 至少 95% 相似度
                actual_size_tolerance = size_tolerance
            
            # 構建參數字典
            kwargs = {}
            if self.has_opencv:
                kwargs['confidence'] = actual_confidence
            else:
                # 沒有 OpenCV 時，使用 grayscale 參數可能有助於匹配
                kwargs['grayscale'] = True
            
            if region:
                location = pyautogui.locateOnScreen(image_path, region=region, **kwargs)
            else:
                location = pyautogui.locateOnScreen(image_path, **kwargs)
            
            if location:
                # 驗證匹配區域的大小是否與模板圖片大小接近（避免誤判）
                if template_size:
                    template_width, template_height = template_size
                    matched_width = location.width
                    matched_height = location.height
                    
                    # 計算大小差異
                    width_diff = abs(matched_width - template_width) / template_width
                    height_diff = abs(matched_height - template_height) / template_height
                    
                    # 如果大小差異超過容差，可能是誤判（例如把 d 誤判為 a）
                    if width_diff > actual_size_tolerance or height_diff > actual_size_tolerance:
                        print(f"  ⚠ 大小驗證失敗: 模板尺寸 {template_size}, 匹配尺寸 ({matched_width}, {matched_height}), 差異: ({width_diff:.2%}, {height_diff:.2%})")
                        return None
                    else:
                        print(f"  ✓ 大小驗證通過: 模板尺寸 {template_size}, 匹配尺寸 ({matched_width}, {matched_height}), 差異: ({width_diff:.2%}, {height_diff:.2%})")
                
                # 計算中心點座標
                center_x = location.left + location.width // 2
                center_y = location.top + location.height // 2
                if return_location:
                    return (center_x, center_y, location)
                else:
                    return (center_x, center_y)
            else:
                return None
                
        except pyautogui.ImageNotFoundException:
            return None
        except TypeError as e:
            # 如果是不支援的參數錯誤，嘗試不使用該參數
            if 'confidence' in str(e) or 'unexpected keyword' in str(e).lower():
                try:
                    if region:
                        location = pyautogui.locateOnScreen(image_path, region=region)
                    else:
                        location = pyautogui.locateOnScreen(image_path)
                    if location:
                        center_x = location.left + location.width // 2
                        center_y = location.top + location.height // 2
                        return (center_x, center_y)
                except:
                    pass
            return None
        except Exception as e:
            # 只在第一次發生錯誤時顯示
            if not hasattr(self, '_error_shown'):
                print(f"注意: {e}")
                self._error_shown = True
            return None
    
    def wait_for_image(self, image_path, image_name, region=None, timeout=None):
        """等待圖案出現，返回座標
        
        修改原因：優先使用緩存的座標，只有在緩存失效時才重新搜索
        
        Args:
            image_path: 圖片檔案路徑
            image_name: 圖片名稱（用於顯示）
            region: 搜尋區域 (left, top, width, height)
            timeout: 超時時間（秒），None 表示不設限
        """
        # 1. 首先檢查是否有緩存的座標
        cached_coord = self.coord_cache.get(image_name)
        if cached_coord and cached_coord.get('verified'):
            print(f"  → 使用緩存座標 {image_name}: ({cached_coord['x']}, {cached_coord['y']}) [緩存於 {cached_coord.get('timestamp', 'N/A')}]")
            # 快速驗證緩存座標是否仍然有效（在該位置附近搜索）
            cached_x = cached_coord['x']
            cached_y = cached_coord['y']
            
            # 創建一個小的搜索區域（以緩存座標為中心，±50 像素）
            verify_region = (
                max(0, cached_x - 50),
                max(0, cached_y - 50),
                100,
                100
            )
            
            # 快速檢查（1秒超時）
            # 修改原因：提升精準度，使用預設的高精準匹配（95%）
            start_verify = time.time()
            while time.time() - start_verify < 1.0:
                result = self.detect_image(image_path, region=verify_region, return_location=True)
                if result:
                    position = (result[0], result[1])
                    location = result[2] if len(result) > 2 else None
                    print(f"  ✓ 緩存座標驗證成功！實際位置: {position}")
                    return (position, location)
                time.sleep(0.1)
            
            print(f"  ⚠ 緩存座標驗證失敗（圖像可能已移動），重新搜索...")
        
        # 2. 如果沒有緩存或緩存失效，執行正常的搜索流程
        # 打印開始偵測的詳細信息
        if region:
            print(f"  → 開始偵測 {image_name}")
            print(f"     搜尋區域: ({region[0]}, {region[1]}) ~ ({region[0]+region[2]}, {region[1]+region[3]})")
            print(f"     區域大小: {region[2]} x {region[3]} 像素")
        else:
            print(f"  → 開始偵測 {image_name} (全螢幕搜尋)")
        
        start_time = time.time()
        check_count = 0
        
        # 只有在 OpenCV 可用時才使用 confidence 參數
        # 修改原因：提升精準度，避免誤判
        # 對於 a.png 和 d.PNG，使用最高相似度（從 0.98 開始），避免 a 和 d 混淆
        # 對於 b.PNG、c.PNG，使用高相似度（從 0.95 開始）
        if self.has_opencv:
            image_name_lower = image_name.lower()
            if 'a.png' in image_name_lower:
                # a.png 使用最高相似度，避免和 d.PNG 混淆
                confidence_levels = [0.98, 0.97, 0.96, 0.95]  # 從 98% 開始，最嚴格
            elif 'd.png' in image_name_lower:
                # d.PNG 使用最高相似度，避免和 a.PNG 混淆
                confidence_levels = [0.98, 0.97, 0.96, 0.95]  # 從 98% 開始，最嚴格
            elif 'b.png' in image_name_lower or 'c.png' in image_name_lower:
                # b.PNG、c.PNG 使用高相似度（提升精準度）
                confidence_levels = [0.95, 0.93, 0.90, 0.85]  # 從 95% 開始，高精準
            else:
                # 其他圖片使用高相似度
                confidence_levels = [0.95, 0.93, 0.90]
            current_confidence_index = 0
            strategy_changed = False
            last_confidence_change = 0
        else:
            confidence_levels = [None]  # 不使用 confidence
            current_confidence_index = 0
            strategy_changed = False
            last_confidence_change = 0
        
        expanded = False
        fullscreen_tried = False
        
        while self.running:
            # 使用當前信心度進行檢測（預設 98% 相似度，更嚴格，避免 a 和 d 混淆）
            # 返回 location 資訊以便更新區域
            if self.has_opencv:
                current_confidence = confidence_levels[current_confidence_index]
                result = self.detect_image(image_path, region=region, confidence=current_confidence, return_location=True)
            else:
                current_confidence = None
                result = self.detect_image(image_path, region=region, confidence=0.98, return_location=True)  # 會被忽略，但保持參數一致
            check_count += 1
            
            if result:
                position = (result[0], result[1])
                location = result[2] if len(result) > 2 else None
                elapsed = time.time() - start_time
                if self.has_opencv and strategy_changed:
                    confidence_str = f" (信心度: {current_confidence})"
                else:
                    confidence_str = ""
                print(f"  ✓ 偵測成功！找到 {image_name}，座標: {position} (耗時: {elapsed:.2f}秒, 檢查次數: {check_count}){confidence_str}")
                # 返回位置和 location 資訊
                return (position, location)
            
            elapsed = time.time() - start_time
            
            # 每 10 次檢查打印一次進度（約 1 秒）
            if check_count % 10 == 0:
                if self.has_opencv:
                    print(f"  ... 正在偵測中... (已等待 {elapsed:.1f}秒, 信心度: {current_confidence})")
                else:
                    print(f"  ... 正在偵測中... (已等待 {elapsed:.1f}秒)")
            
            # 超過 3 秒：降低相似度要求（每 3 秒嘗試一次，僅在 OpenCV 可用時）
            # 從 98% 逐步降低到 96%、95%，但保持高相似度
            if self.has_opencv and elapsed > 3 and elapsed - last_confidence_change > 3:
                if current_confidence_index < len(confidence_levels) - 1:
                    current_confidence_index += 1
                    strategy_changed = True
                    last_confidence_change = elapsed
                    print(f"  ⚠ 檢測超過 {int(elapsed)} 秒，降低相似度要求至 {int(confidence_levels[current_confidence_index]*100)}%")
            
            # 超過 10 秒：擴大搜尋範圍（如果原本有指定區域且尚未擴大）
            if elapsed > 10 and region and not expanded:
                # 對於 b.PNG、c.PNG，擴大更多範圍
                image_name_lower = image_name.lower()
                if 'b.png' in image_name_lower or 'c.png' in image_name_lower:
                    # 擴大範圍：往四個方向各擴展 200 像素（更寬鬆）
                    expand_pixels = 200
                elif 'd.png' in image_name_lower:
                    # 對於 d.PNG，擴大範圍：往四個方向各擴展 100 像素（中等）
                    expand_pixels = 100
                else:
                    # 對於 a.png，擴大範圍：往四個方向各擴展 50 像素
                    expand_pixels = 50
                
                expanded_region = (
                    max(0, region[0] - expand_pixels),
                    max(0, region[1] - expand_pixels),
                    region[2] + (expand_pixels * 2),
                    region[3] + (expand_pixels * 2)
                )
                print(f"  ⚠ 檢測超過 10 秒，擴大搜尋範圍（擴展 {expand_pixels} 像素）")
                print(f"     新範圍: ({expanded_region[0]}, {expanded_region[1]}) ~ ({expanded_region[0]+expanded_region[2]}, {expanded_region[1]+expanded_region[3]})")
                region = expanded_region
                expanded = True
            
            # 超過 15 秒：嘗試全螢幕搜尋（如果原本有指定區域且尚未嘗試）
            if elapsed > 15 and region and not fullscreen_tried:
                print(f"  ⚠ 檢測超過 15 秒，嘗試全螢幕搜尋")
                region = None  # 改為全螢幕搜尋
                fullscreen_tried = True
            
            # 檢查超時（如果設定了 timeout）
            if timeout and elapsed > timeout:
                print(f"  ✗ 偵測超時: {image_name} (超過 {timeout}秒)")
                return None
            
            # 短暫延遲，避免 CPU 使用率過高
            time.sleep(0.1)
        
        return None
    
    def get_cursor_type(self):
        """獲取當前滑鼠指標類型
        
        Returns:
            str: 'pointer' 如果是手型指標，'arrow' 如果是箭頭，None 如果無法檢測
        """
        if not HAS_WIN32:
            return None
        
        try:
            import win32gui
            
            # 獲取當前指標句柄
            cursor_handle = win32gui.GetCursor()
            
            if cursor_handle == 0:
                return None
            
            # 載入標準指標進行比較
            # IDC_HAND = 32649 (手型指標)
            # IDC_ARROW = 32512 (箭頭指標)
            try:
                hand_cursor = win32gui.LoadCursor(0, win32con.IDC_HAND)
                arrow_cursor = win32gui.LoadCursor(0, win32con.IDC_ARROW)
                
                # 比較指標句柄
                if cursor_handle == hand_cursor:
                    return 'pointer'
                elif cursor_handle == arrow_cursor:
                    return 'arrow'
            except:
                pass
            
            return None
        except Exception as e:
            # 如果檢測失敗，返回 None
            return None
    
    def is_pointer_cursor(self, x, y):
        """檢查指定位置是否為可點擊區域（指標為 pointer/hand）
        
        移動滑鼠到該位置，然後檢查指標類型是否為 pointer/hand
        
        Args:
            x, y: 座標位置
            
        Returns:
            bool: 如果指標為 pointer/hand 則返回 True，否則返回 False
        """
        try:
            # 移動滑鼠到該位置
            pyautogui.moveTo(x, y, duration=0.1)
            time.sleep(0.2)  # 等待指標變化
            
            # 檢查指標類型
            cursor_type = self.get_cursor_type()
            
            if cursor_type == 'pointer':
                return True
            elif cursor_type == 'arrow':
                return False
            else:
                # 如果無法檢測指標類型（返回 None），我們採用備用策略：
                # 由於 Windows API 檢測可能不準確，我們允許嘗試點擊
                # 但優先檢查是否為 pointer，如果不是 pointer 且不是 arrow，也允許嘗試
                # 這樣可以處理一些特殊情況
                return True  # 允許嘗試，但會通過點擊後檢查 b.PNG 來驗證
        except Exception as e:
            # 如果檢測失敗，允許嘗試（通過點擊後檢查 b.PNG 來驗證）
            return True
    
    def verify_detected_image(self, location, template_path, image_name):
        """驗證識別到的圖像是否正確
        
        保存識別到的截圖，並檢查是否為可點擊區域
        
        Args:
            location: pyautogui 的 location 物件（包含 left, top, width, height）
            template_path: 模板圖片路徑
            image_name: 圖片名稱（用於顯示和保存）
            
        Returns:
            dict: 驗證結果
                {
                    'is_valid': bool,  # 是否通過驗證
                    'is_clickable': bool,  # 是否為可點擊區域
                    'screenshot_path': str,  # 截圖保存路徑
                    'reason': str  # 驗證失敗原因
                }
        """
        result = {
            'is_valid': False,
            'is_clickable': False,
            'screenshot_path': None,
            'reason': ''
        }
        
        try:
            # 1. 獲取識別到的區域截圖（臨時使用，處理完後刪除）
            print(f"\n  [驗證] 獲取識別區域截圖...")
            # 修改原因：pyautogui.screenshot() 需要 Python int，不能是 numpy.int64
            # 將 location 的屬性轉換為 Python int
            region = (int(location.left), int(location.top), int(location.width), int(location.height))
            screenshot = pyautogui.screenshot(region=region)
            
            # 創建臨時 screenshots 目錄（如果不存在）
            screenshots_dir = 'screenshots'
            if not os.path.exists(screenshots_dir):
                os.makedirs(screenshots_dir)
            
            # 保存截圖（臨時使用）
            timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
            screenshot_filename = f"{image_name.replace('.', '_')}_{timestamp}.png"
            screenshot_path = os.path.join(screenshots_dir, screenshot_filename)
            screenshot.save(screenshot_path)
            result['screenshot_path'] = screenshot_path
            print(f"  [驗證] 截圖已保存（臨時）: {screenshot_path}")
            
            # 2. 檢查識別區域中心點是否為可點擊區域
            # 修改原因：確保座標是 Python int 類型
            center_x = int(location.left + location.width // 2)
            center_y = int(location.top + location.height // 2)
            
            print(f"  [驗證] 檢查中心點 ({center_x}, {center_y}) 是否為可點擊區域...")
            is_clickable = self.is_pointer_cursor(center_x, center_y)
            result['is_clickable'] = is_clickable
            
            if is_clickable:
                print(f"  [驗證] ✓ 中心點為可點擊區域（pointer cursor）")
            else:
                print(f"  [驗證] ✗ 中心點不是可點擊區域（非 pointer cursor）")
                result['reason'] = '不是可點擊區域'
            
            # 3. 比較截圖與模板的相似度
            print(f"  [驗證] 比較截圖與模板的相似度...")
            template = Image.open(template_path)
            
            # 確保兩個圖片大小相同
            if screenshot.size != template.size:
                print(f"  [驗證] ⚠ 截圖大小 {screenshot.size} 與模板大小 {template.size} 不同，調整截圖大小...")
                screenshot = screenshot.resize(template.size, Image.LANCZOS)
            
            # 轉換為 numpy 陣列進行比較
            screenshot_array = np.array(screenshot)
            template_array = np.array(template)
            
            # 計算平均絕對誤差（MAE）
            mae = np.mean(np.abs(screenshot_array.astype(float) - template_array.astype(float)))
            
            # 計算相似度百分比（100% - 誤差百分比）
            similarity = 100 - (mae / 255.0 * 100)
            
            print(f"  [驗證] 相似度: {similarity:.2f}%")
            
            # 如果相似度 >= 85%，認為是正確的圖像
            if similarity >= 85.0:
                print(f"  [驗證] ✓ 相似度驗證通過 (>= 85%)")
                result['is_valid'] = True
            else:
                print(f"  [驗證] ✗ 相似度驗證失敗 (< 85%)")
                result['reason'] = f'相似度不足 ({similarity:.2f}%)'
            
            # 處理完後刪除截圖文件
            if result['screenshot_path'] and os.path.exists(result['screenshot_path']):
                try:
                    os.remove(result['screenshot_path'])
                    print(f"  [驗證] 截圖已刪除: {result['screenshot_path']}")
                except Exception as e:
                    print(f"  [驗證] ⚠ 刪除截圖失敗: {e}")
                finally:
                    # 清除路徑引用
                    result['screenshot_path'] = None
            
            return result
            
        except Exception as e:
            print(f"  [驗證] ✗ 驗證過程發生錯誤: {e}")
            result['reason'] = f'驗證錯誤: {str(e)}'
            
            # 如果發生錯誤，也要刪除截圖
            if result['screenshot_path'] and os.path.exists(result['screenshot_path']):
                try:
                    os.remove(result['screenshot_path'])
                    print(f"  [驗證] 截圖已刪除: {result['screenshot_path']}")
                except:
                    pass
                finally:
                    result['screenshot_path'] = None
            
            return result
    
    def search_clickable_area_in_b_region(self):
        """在 b 區域內搜索可點擊位置
        
        移動滑鼠到右上方 b 區域的不同位置，直接點擊，
        然後檢查 b.PNG 是否出現。
        
        Returns:
            tuple: ((click_x, click_y), (pos_b, location_b)) 如果找到可點擊位置並成功出現 b.PNG
                   返回 ((點擊座標), (b.PNG位置, location))，否則返回 None
        """
        print("\n[步驟 2] b.PNG 未出現，開始搜索可點擊位置...")
        
        # 修改原因：根據圖片尋找工具的實際檢測結果更新搜索中心點
        # 參考：b.PNG 中心座標 (1891, 340)
        # 定義搜索區域（以 (1891, 340) 為中心，涵蓋右上方區域）
        # 搜索範圍：從 (1741, 190) 到 (2041, 490)
        search_center_x = 1891
        search_center_y = 340
        
        # 搜索區域：以中心點為基準，往四個方向各擴展
        search_radius = 150  # 搜索半徑
        search_region = (
            search_center_x - search_radius,
            search_center_y - search_radius,
            search_radius * 2,
            search_radius * 2
        )
        
        print(f"  搜索區域: ({search_region[0]}, {search_region[1]}) ~ "
              f"({search_region[0]+search_region[2]}, {search_region[1]+search_region[3]})")
        print(f"  搜索中心: ({search_center_x}, {search_center_y})")
        
        # 定義搜索網格（每 15 像素一個點，更密集）
        step = 15
        start_x = search_region[0]
        start_y = search_region[1]
        end_x = search_region[0] + search_region[2]
        end_y = search_region[1] + search_region[3]
        
        # 從中心點 (1729, 104) 開始向外搜索
        center_x = search_center_x
        center_y = search_center_y
        
        # 生成搜索點列表（從中心向外，螺旋式搜索）
        search_points = []
        max_radius = max(search_region[2], search_region[3]) // 2
        
        # 先添加中心點
        search_points.append((center_x, center_y))
        
        # 從中心向外螺旋搜索
        for radius in range(step, max_radius, step):
            # 每圈增加點數，使搜索更密集
            num_points = max(8, int(radius / step))
            angle_step = 360 / num_points
            
            for i in range(num_points):
                angle = i * angle_step
                x = int(center_x + radius * math.cos(math.radians(angle)))
                y = int(center_y + radius * math.sin(math.radians(angle)))
                
                # 確保在搜索區域內
                if start_x <= x <= end_x and start_y <= y <= end_y:
                    search_points.append((x, y))
        
        # 去重並保持順序
        seen = set()
        unique_points = []
        for point in search_points:
            if point not in seen:
                seen.add(point)
                unique_points.append(point)
        search_points = unique_points
        
        print(f"  生成 {len(search_points)} 個搜索點（從中心向外螺旋搜索）")
        
        # 限制搜索點數，避免搜索時間過長（最多搜索 50 個點）
        max_search_points = 50
        if len(search_points) > max_search_points:
            print(f"  限制搜索點數為 {max_search_points} 個（從中心開始）")
            search_points = search_points[:max_search_points]
        
        # 依次嘗試每個點
        for idx, (x, y) in enumerate(search_points):
            if not self.running:
                return None
            
            print(f"  [{idx+1}/{len(search_points)}] 嘗試座標: ({x}, {y})")
            
            # 移動滑鼠到該位置
            pyautogui.moveTo(x, y, duration=0.1)
            time.sleep(0.2)  # 等待指標變化
            
            # 直接點擊（不檢測 pointer）
            print(f"    → 移動到 ({x}, {y})，嘗試點擊...")
            
            # 點擊該位置
            pyautogui.mouseDown(button='left')
            time.sleep(0.05)
            pyautogui.mouseUp(button='left')
            time.sleep(0.5)  # 等待響應
            
            # 使用 wait_for_image 策略檢測 b.PNG（擴大範圍、降低相似度、全屏搜索等）
            # 設置足夠長的超時（20秒），讓策略有時間執行擴大範圍和全屏搜索
            print(f"    → 點擊後，使用檢測策略等待 b.PNG 出現（最多等待 20 秒，會自動擴大範圍和全屏搜索）...")
            # 修改原因：根據圖片尋找工具的實際檢測結果更新搜索區域
            # 參考：b.PNG 中心座標 (1891, 340)，區域: 左上(1876, 323) 大小(31x34)
            # 先從較小的區域開始，wait_for_image 會自動擴大
            expanded_region_b = (1876 - 100, 323 - 100, 31 + 200, 34 + 200)  # (1776, 223, 231, 234)
            result_b = self.wait_for_image(self.image_b, 'b.PNG', region=expanded_region_b, timeout=20.0)
            
            if result_b:
                # 解包結果
                if isinstance(result_b, tuple) and len(result_b) == 2:
                    pos_b, location_b = result_b
                elif isinstance(result_b, tuple) and len(result_b) == 3:
                    pos_b = (result_b[0], result_b[1])
                    location_b = result_b[2]
                else:
                    pos_b = result_b
                    location_b = None
                
                print(f"  ✓ 找到可點擊位置！座標: ({x}, {y})")
                print(f"  ✓ b.PNG 已出現，座標: {pos_b}")
                
                # 更新 b 區域
                if location_b:
                    self.update_region(location_b, 'b', expand_pixels=50)
                
                # 返回點擊座標和 b.PNG 位置信息
                return ((x, y), (pos_b, location_b))
            else:
                print(f"    ✗ b.PNG 未出現（3秒內），繼續下一個位置...")
        
        print("  ✗ 搜索完成，未找到可點擊位置")
        return None
    
    def click(self, x, y, description=""):
        """模擬點擊：先移動滑鼠到目標位置，再模擬左鍵點擊"""
        try:
            # 點擊前延遲 1 秒
            print(f"  → 點擊前等待 1 秒...")
            time.sleep(1.0)
            
            # 先移動滑鼠到目標位置
            print(f"  → 移動滑鼠到座標: ({x}, {y})")
            pyautogui.moveTo(x, y, duration=0.1)  # 移動滑鼠，duration 為移動時間
            time.sleep(0.1)  # 移動後稍作延遲，確保滑鼠已到達位置
            
            # 模擬滑鼠左鍵點擊
            print(f"  → 模擬滑鼠左鍵點擊")
            pyautogui.mouseDown(button='left')  # 按下左鍵
            time.sleep(0.05)  # 短暫延遲，模擬真實點擊
            pyautogui.mouseUp(button='left')  # 釋放左鍵
            
            if description:
                print(f"✓ {description} - 點擊完成，座標: ({x}, {y})")
            else:
                print(f"✓ 點擊完成，座標: ({x}, {y})")
            
            # 點擊後延遲 1 秒
            print(f"  → 點擊後等待 1 秒...")
            time.sleep(1.0)
        except Exception as e:
            print(f"✗ 點擊失敗: {e}")
            import traceback
            traceback.print_exc()
    
    def run(self):
        """主循環"""
        if not self.running:
            return
        
        cycle_count = 0
        
        try:
            print("\n" + "=" * 60)
            print("開始執行自動點擊循環")
            print("=" * 60)
            
            while self.running:
                cycle_count += 1
                print(f"\n{'='*60}")
                print(f"[循環 #{cycle_count}] 開始")
                print(f"{'='*60}")
                
                # 步驟 1: 等待 a.png 出現（在指定區域內搜尋）
                print("\n[步驟 1] 偵測 a.png")
                result_a = self.wait_for_image(self.image_a, 'a.png', region=self.region_a)
                
                if not result_a:
                    print("\n程式已停止")
                    break
                
                # 解包結果：result_a 是 (position, location) 或 position
                if isinstance(result_a, tuple) and len(result_a) == 2:
                    pos_a, location_a = result_a
                else:
                    pos_a = result_a
                    location_a = None
                
                # 如果檢測到位置，更新區域和緩存
                if location_a:
                    self.update_region(location_a, 'a', expand_pixels=50)
                    # 修改原因：記錄 a.png 的座標到緩存
                    self.update_coord_cache('a.png', pos_a[0], pos_a[1], verified=True)
                
                # 修改原因：根據圖片尋找工具的實際檢測結果，a.png 中心座標為 (1891, 981)
                # 參考：圖片尋找工具輸出結果（2026-01-08 19:43:46）- a.png 中心座標 (1891, 981)
                # 點擊 a.png 中心座標
                print(f"\n[步驟 1] 執行點擊動作")
                click_x = int(pos_a[0])  # 使用檢測到的 a.png 中心座標 X
                click_y = int(pos_a[1])  # 使用檢測到的 a.png 中心座標 Y
                print(f"  計算點擊座標: a.png 中心座標 {pos_a} = ({click_x}, {click_y})")
                self.click(click_x, click_y, "點擊 a.png 中心座標")
                print(f"[步驟 1] 點擊完成，等待 0.3 秒...")
                time.sleep(0.3)  # 點擊後等待
                
                # 步驟 2: 先尋找 e.PNG，點擊 e.PNG 左下 10px，然後等待 b.PNG 出現
                print("\n[步驟 2] 偵測 e.PNG")
                
                # 初始化變量
                e_click_success = False
                result_b_from_e = None
                
                # 如果 e.PNG 存在，先尋找並點擊
                if self.image_e:
                    # 計算 e.PNG 的搜索區域（右上角區域，根據 e.PNG 尺寸的 5 倍）
                    e_region = self.get_e_region()
                    if e_region:
                        print(f"  → 開始在右上角區域搜索 e.PNG...")
                        print(f"     搜索區域: ({e_region[0]}, {e_region[1]}) ~ "
                              f"({e_region[0]+e_region[2]}, {e_region[1]+e_region[3]})")
                        print(f"     區域大小: {e_region[2]} x {e_region[3]} 像素")
                        result_e = self.wait_for_image(self.image_e, 'e.PNG', region=e_region, timeout=30.0)
                    else:
                        # 如果無法計算區域，使用全屏搜索
                        print("  → 無法計算 e.PNG 搜索區域，使用全屏搜索...")
                        result_e = self.wait_for_image(self.image_e, 'e.PNG', region=None, timeout=30.0)
                    
                    if result_e:
                        # 解包結果
                        if isinstance(result_e, tuple) and len(result_e) == 2:
                            pos_e, location_e = result_e
                        elif isinstance(result_e, tuple) and len(result_e) == 3:
                            pos_e = (result_e[0], result_e[1])
                            location_e = result_e[2]
                        else:
                            pos_e = result_e
                            location_e = None
                        
                        print(f"  ✓ 找到 e.PNG，座標: {pos_e}")
                        
                        # 記錄 e.PNG 座標到緩存
                        if location_e:
                            self.update_coord_cache('e.PNG', pos_e[0], pos_e[1], verified=True)
                        
                        # 保存 e.PNG 位置供後續使用
                        self.last_e_position = pos_e
                        self.last_e_location = location_e  # 保存 location 供後續使用
                        
                        # 計算 e.PNG 左下角邊緣位置
                        # 從左下角邊緣開始計算，而不是從中心點
                        if location_e:
                            # 使用 location 信息計算左下角邊緣
                            e_bottom_left_x = int(location_e.left)  # 左邊緣 x 座標
                            e_bottom_left_y = int(location_e.top + location_e.height)  # 下邊緣 y 座標
                            print(f"  → e.PNG 左下角邊緣座標: ({e_bottom_left_x}, {e_bottom_left_y})")
                            print(f"     e.PNG 區域: 左上({location_e.left}, {location_e.top}), 大小({location_e.width}x{location_e.height})")
                        else:
                            # 如果沒有 location，嘗試從中心點和圖片尺寸計算
                            e_size = self.get_image_size(self.image_e)
                            if e_size:
                                e_width, e_height = e_size
                                # 從中心點計算左下角邊緣
                                e_bottom_left_x = int(pos_e[0]) - e_width // 2  # 中心 x - 寬度的一半 = 左邊緣
                                e_bottom_left_y = int(pos_e[1]) + e_height // 2  # 中心 y + 高度的一半 = 下邊緣
                                print(f"  → e.PNG 左下角邊緣座標（從中心點計算）: ({e_bottom_left_x}, {e_bottom_left_y})")
                            else:
                                # 如果無法獲取尺寸，使用中心點（備用方案）
                                print(f"  ⚠ 無法獲取 e.PNG 尺寸，使用中心點作為參考")
                                e_bottom_left_x = int(pos_e[0])
                                e_bottom_left_y = int(pos_e[1])
                        
                        # 嘗試點擊 e.PNG 左下角邊緣的不同位置（逐步增加偏移量）
                        # 偏移量列表：從左下角邊緣往左下偏移 10px, 20px, 30px
                        e_click_offsets = [(10, 10), (20, 20), (30, 30)]
                        e_click_success = False
                        result_b_from_e = None  # 用於保存從 e.PNG 點擊後檢測到的 b.PNG
                        
                        for offset_idx, (offset_x, offset_y) in enumerate(e_click_offsets):
                            print(f"\n[步驟 2] 嘗試點擊 e.PNG 左下角邊緣往左下 {offset_x}px (嘗試 {offset_idx + 1}/{len(e_click_offsets)})")
                            # 從左下角邊緣往左下偏移
                            click_e_x = e_bottom_left_x - offset_x  # 左：x 減 offset_x
                            click_e_y = e_bottom_left_y + offset_y  # 下：y 加 offset_y
                            print(f"  計算點擊座標: e.PNG左下角邊緣 ({e_bottom_left_x}, {e_bottom_left_y}) 往左下 {offset_x}px = ({click_e_x}, {click_e_y})")
                            self.click(click_e_x, click_e_y, f"點擊 e.PNG 左下角邊緣往左下 {offset_x}px")
                            print(f"  → e.PNG 點擊完成，等待 1.5 秒後檢查 b.PNG...")
                            time.sleep(1.5)  # 點擊後等待
                            
                            # 檢查 b.PNG 是否出現
                            expanded_region_b = self.region_b
                            result_b_check = self.detect_image(self.image_b, region=expanded_region_b, return_location=True)
                            
                            if result_b_check:
                                print(f"  ✓ b.PNG 已出現！")
                                e_click_success = True
                                # 保存檢測結果供後續使用
                                result_b_from_e = result_b_check
                                break
                            else:
                                print(f"  → b.PNG 仍未出現，嘗試下一個偏移量...")
                        
                        if not e_click_success:
                            print(f"  ⚠ 嘗試了所有 e.PNG 左下角位置，b.PNG 仍未出現")
                    else:
                        print(f"  ⚠ e.PNG 未找到（30秒超時），跳過 e.PNG 步驟，直接尋找 b.PNG")
                else:
                    print("  → e.PNG 檔案不存在，跳過 e.PNG 步驟，直接尋找 b.PNG")
                
                # 步驟 2.1: 等待 b.PNG 出現（在指定區域內搜尋）
                # 如果點擊 e.PNG 後 b.PNG 已經出現，使用該結果
                if e_click_success and result_b_from_e:
                    print("\n[步驟 2.1] b.PNG 已在點擊 e.PNG 後出現，使用該結果")
                    result_b = result_b_from_e
                else:
                    print("\n[步驟 2.1] 偵測 b.PNG")
                    
                    # 先快速檢查 b.PNG 是否已經出現（使用高精準匹配）
                    # 使用動態計算的 b 區域
                    expanded_region_b = self.region_b
                    print(f"  → 搜索 b.PNG，區域: ({expanded_region_b[0]}, {expanded_region_b[1]}) ~ "
                          f"({expanded_region_b[0]+expanded_region_b[2]}, {expanded_region_b[1]+expanded_region_b[3]})")
                    result_b = self.detect_image(self.image_b, region=expanded_region_b, return_location=True)
                
                # 標記是否已經處理了 b.PNG（如果檢測到就直接點擊）
                b_already_handled = False
                
                # 如果檢測到 b.PNG，直接使用它的座標並點擊
                if result_b:
                    pos_b = (result_b[0], result_b[1])
                    location_b = result_b[2] if len(result_b) > 2 else None
                    print(f"  ✓ b.PNG 已出現，座標: {pos_b}")
                    
                    # 保存 b.PNG 的位置供後續重試使用
                    self.last_b_position = pos_b
                    
                    # 如果檢測到位置，更新區域和緩存
                    if location_b:
                        self.update_region(location_b, 'b', expand_pixels=50)
                        # 修改原因：記錄 b.PNG 的座標到緩存
                        self.update_coord_cache('b.PNG', pos_b[0], pos_b[1], verified=True)
                    
                    # 直接點擊 b.PNG
                    print(f"\n[步驟 2.1] 執行點擊 b.PNG 動作")
                    click_x = int(pos_b[0]) - 1  # 轉換為整數並減 1
                    click_y = int(pos_b[1]) - 2  # 轉換為整數並減 2
                    print(f"  計算點擊座標: b.PNG座標 {pos_b} + (-1, -2) = ({click_x}, {click_y})")
                    self.click(click_x, click_y, "點擊 b.PNG 有效位置")
                    print(f"[步驟 2] 點擊完成，等待 5 秒...")
                    time.sleep(5.0)  # 點擊後等待 5 秒
                    
                    # 標記已經處理了 b.PNG，跳過後續的檢測邏輯
                    b_already_handled = True
                
                # 如果 b.PNG 仍未出現，使用 wait_for_image 等待（使用更寬鬆的相似度和更大的搜索區域）
                if not b_already_handled and not result_b:
                    print("\n[步驟 2.1] b.PNG 仍未出現，等待 b.PNG 出現...")
                    # 使用動態計算的 b 區域
                    expanded_region_b = (
                        max(0, self.region_b[0] - 200),
                        max(0, self.region_b[1] - 200),
                        self.region_b[2] + 400,
                        self.region_b[3] + 400
                    )
                    result_b = self.wait_for_image(self.image_b, 'b.PNG', region=expanded_region_b)
                
                # 如果仍然沒有結果，停止程式
                if not b_already_handled and not result_b:
                    print("\n程式已停止")
                    break
                
                # 如果還沒有處理 b.PNG，現在處理
                if not b_already_handled:
                    # 解包結果
                    if isinstance(result_b, tuple) and len(result_b) == 2:
                        pos_b, location_b = result_b
                    elif isinstance(result_b, tuple) and len(result_b) == 3:
                        pos_b = (result_b[0], result_b[1])
                        location_b = result_b[2]
                    else:
                        pos_b = result_b
                        location_b = None
                    
                    # 保存 b.PNG 的位置供後續重試使用
                    self.last_b_position = pos_b
                    
                    # 如果檢測到位置，更新區域和緩存
                    if location_b:
                        self.update_region(location_b, 'b', expand_pixels=50)
                        # 修改原因：記錄 b.PNG 的座標到緩存
                        self.update_coord_cache('b.PNG', pos_b[0], pos_b[1], verified=True)
                    
                    # 直接點擊 b.PNG 座標 + (-1, -2) -> 有效點擊位置
                    print(f"\n[步驟 2.1] 執行點擊 b.PNG 動作")
                    click_x = int(pos_b[0]) - 1  # 轉換為整數並減 1
                    click_y = int(pos_b[1]) - 2  # 轉換為整數並減 2
                    print(f"  計算點擊座標: b.PNG座標 {pos_b} + (-1, -2) = ({click_x}, {click_y})")
                    self.click(click_x, click_y, "點擊 b.PNG 有效位置")
                    print(f"[步驟 2] 點擊完成，等待 5 秒...")
                    time.sleep(5.0)  # 點擊後等待 5 秒
                
                # 步驟 2.5: 檢查 d.PNG 是否已出現（表示上一輪還在執行）
                # 修改原因：用戶要求 d=已執行，a=未執行，必須點擊c後等待d出現
                print("\n[步驟 2.5] 檢查 a 區域是否出現 d.PNG（上一輪是否還在執行）")
                if self.image_d:
                    # 擴大搜索區域並使用高相似度
                    expanded_region_a = (
                        max(0, self.region_a[0] - 200),
                        max(0, self.region_a[1] - 200),
                        self.region_a[2] + 400,
                        self.region_a[3] + 400
                    )
                    result_d = self.detect_image(self.image_d, region=expanded_region_a, confidence=0.95, return_location=True, size_tolerance=0.10)
                    if result_d:
                        pos_d = (result_d[0], result_d[1])
                        location_d = result_d[2] if len(result_d) > 2 else None
                        print(f"  ✓ 偵測到 d.PNG（上一輪還在執行），座標: {pos_d}")
                        if location_d:
                            print(f"     檢測區域: ({location_d.left}, {location_d.top}) ~ "
                                  f"({location_d.left + location_d.width}, {location_d.top + location_d.height})")
                        
                        # 如果檢測到 d，更新區域和緩存
                        if location_d:
                            self.update_region(location_d, 'd', expand_pixels=50)
                            # 修改原因：記錄 d.PNG 的座標到緩存
                            self.update_coord_cache('d.PNG', pos_d[0], pos_d[1], verified=True)
                        
                        print("  → d 還在執行中，等待 d 變成 a 才能繼續...")
                        
                        # 等待 d 消失，a 出現
                        print("\n[步驟 2.6] 等待 d 消失，a 出現")
                        while self.running:
                            # 每次循環都重新計算檢測區域（使用最新的 region_a），避免使用舊的暫存區域
                            # 檢查 d 是否還在（提高相似度要求，避免誤檢測）
                            expanded_region_a = (
                                max(0, self.region_a[0] - 200),
                                max(0, self.region_a[1] - 200),
                                self.region_a[2] + 400,
                                self.region_a[3] + 400
                            )
                            print(f"  → 使用最新計算的檢測區域: ({expanded_region_a[0]}, {expanded_region_a[1]}) ~ "
                                  f"({expanded_region_a[0]+expanded_region_a[2]}, {expanded_region_a[1]+expanded_region_a[3]})")
                            result_d_check = self.detect_image(self.image_d, region=expanded_region_a, confidence=0.95, return_location=True, size_tolerance=0.10)
                            if not result_d_check:
                                # d 已消失，檢查 a 是否出現
                                print("  → d 已消失，檢查 a 是否出現...")
                                # 檢測 a.PNG 時使用高相似度和嚴格的大小驗證，避免誤判為 d.PNG
                                expanded_region_a_check = (
                                    max(0, self.region_a[0] - 100),
                                    max(0, self.region_a[1] - 100),
                                    self.region_a[2] + 200,
                                    self.region_a[3] + 200
                                )
                                result_a_check = self.detect_image(self.image_a, region=expanded_region_a_check, confidence=0.98, return_location=True, size_tolerance=0.05)
                                if result_a_check:
                                    pos_a_check = (result_a_check[0], result_a_check[1])
                                    location_a_check = result_a_check[2] if len(result_a_check) > 2 else None
                                    print(f"  ✓ a 已出現（可以開始新的執行），座標: {pos_a_check}")
                                    
                                    # 更新 a 的區域
                                    if location_a_check:
                                        self.update_region(location_a_check, 'a', expand_pixels=50)
                                    
                                    # 回到步驟 1，繼續循環
                                    break
                                else:
                                    print("  ... 等待 a 出現...")
                                    time.sleep(0.5)
                            else:
                                # d 還在，打印檢測到的座標和當前時間
                                pos_d_check = (result_d_check[0], result_d_check[1])
                                location_d_check = result_d_check[2] if len(result_d_check) > 2 else None
                                current_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                                print(f"  → d 還在執行中，檢測到座標: {pos_d_check}，當前時間: {current_time}")
                                if location_d_check:
                                    print(f"     檢測區域: ({location_d_check.left}, {location_d_check.top}) ~ "
                                          f"({location_d_check.left + location_d_check.width}, {location_d_check.top + location_d_check.height})")
                                print(f"  → 等待 1 分鐘後再檢查...")
                                wait_start_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                                print(f"     等待開始時間: {wait_start_time}")
                                time.sleep(60.0)  # 等待 60 秒
                                wait_end_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                                print(f"  → 1 分鐘已過，等待結束時間: {wait_end_time}，重新檢查 d 狀態...")
                                # 添加小延遲，確保屏幕已更新，並強制重新計算檢測區域（不使用緩存）
                                time.sleep(0.5)  # 等待屏幕更新
                                print(f"  → 重新計算檢測區域（使用最新的 region_a），避免使用舊的暫存區域...")
                        
                        # d 消失，a 出現，回到步驟 1
                        current_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                        print(f"\n[循環 #{cycle_count}] 完成（等待上一輪執行結束）")
                        print(f"完成時間: {current_time}")
                        print("準備開始下一輪循環，等待 0.5 秒...")
                        time.sleep(0.5)  # 循環間隔
                        continue  # 繼續下一輪循環
                    else:
                        print("  → 未偵測到 d.PNG，可以繼續執行步驟 3（點擊 c）")
                else:
                    print("  → d.PNG 檔案不存在，繼續執行步驟 3")
                
                # 步驟 3: 等待 c.PNG 出現（在指定區域內搜尋）
                # 添加重試機制：如果90秒內未檢測到c.PNG，重新點擊b並重試
                max_retries = 3  # 最多重試3次
                retry_count = 0
                result_c = None
                
                while retry_count < max_retries and not result_c:
                    if retry_count > 0:
                        print(f"\n[步驟 3] 重試 #{retry_count}：重新點擊 b 並等待 c.PNG")
                        print("  → 原因：c.PNG 檢測超時，可能需要重新觸發")
                        
                        # 重新點擊 e.PNG 左下角邊緣的不同位置（如果存在）
                        if self.image_e:
                            # 優先使用上次保存的 location
                            if hasattr(self, 'last_e_location') and self.last_e_location:
                                print(f"  → 使用上次的 e.PNG location 重新點擊")
                                e_bottom_left_x = int(self.last_e_location.left)
                                e_bottom_left_y = int(self.last_e_location.top + self.last_e_location.height)
                            else:
                                # 使用緩存的座標
                                cached_e = self.coord_cache.get('e.PNG')
                                if cached_e and cached_e.get('verified'):
                                    print(f"  → 使用緩存的 e.PNG 位置重新點擊")
                                    pos_e = (cached_e['x'], cached_e['y'])
                                    # 嘗試從中心點和圖片尺寸計算左下角邊緣
                                    e_size = self.get_image_size(self.image_e)
                                    if e_size:
                                        e_width, e_height = e_size
                                        e_bottom_left_x = int(pos_e[0]) - e_width // 2
                                        e_bottom_left_y = int(pos_e[1]) + e_height // 2
                                    else:
                                        # 備用方案：使用中心點
                                        e_bottom_left_x = int(pos_e[0])
                                        e_bottom_left_y = int(pos_e[1])
                                else:
                                    print(f"  → 無法獲取 e.PNG 位置，跳過")
                                    e_bottom_left_x = None
                            
                            if e_bottom_left_x is not None:
                                # 嘗試不同的偏移量
                                e_click_offsets = [(10, 10), (20, 20), (30, 30)]
                                for offset_x, offset_y in e_click_offsets:
                                    click_e_x = e_bottom_left_x - offset_x
                                    click_e_y = e_bottom_left_y + offset_y
                                    print(f"  → 嘗試點擊 e.PNG 左下角邊緣往左下 {offset_x}px: ({click_e_x}, {click_e_y})")
                                    self.click(click_e_x, click_e_y, f"重新點擊 e.PNG 左下角邊緣往左下 {offset_x}px")
                                    time.sleep(1.5)
                                    
                                    # 檢查 b.PNG 是否出現
                                    result_b_check = self.detect_image(self.image_b, region=self.region_b, return_location=True)
                                    if result_b_check:
                                        print(f"  ✓ b.PNG 已出現！")
                                        break
                        
                        # 如果 b.PNG 出現，點擊它
                        if hasattr(self, 'last_b_position') and self.last_b_position:
                            print(f"  → 使用上次的 b.PNG 位置重新點擊")
                            click_x = int(self.last_b_position[0]) - 1
                            click_y = int(self.last_b_position[1]) - 2
                            self.click(click_x, click_y, "重新點擊 b.PNG 有效位置")
                            print(f"  → 點擊完成，等待 5 秒...")
                            time.sleep(5.0)
                    
                    print(f"\n[步驟 3{f' - 重試 #{retry_count}' if retry_count > 0 else ''}] 偵測 c.PNG")
                    # 修改原因：根據圖片尋找工具的實際檢測結果更新搜索區域
                    # 參考：c.PNG 中心座標 (937, 308)，區域: 左上(855, 290) 大小(165x37)
                    # 擴大搜索區域：往四個方向各擴展 200 像素
                    expanded_region_c = (
                        max(0, self.region_c[0] - 200),
                        max(0, self.region_c[1] - 200),
                        self.region_c[2] + 400,
                        self.region_c[3] + 400
                    )
                    # 設置超時時間為90秒
                    result_c = self.wait_for_image(self.image_c, 'c.PNG', region=expanded_region_c, timeout=90.0)
                    
                    if not result_c:
                        retry_count += 1
                        if retry_count < max_retries:
                            print(f"\n⚠ c.PNG 檢測超時（90秒），準備重試 #{retry_count}/{max_retries-1}")
                        else:
                            print(f"\n✗ c.PNG 檢測失敗，已達最大重試次數（{max_retries}次）")
                            print("⚠ 不停止程式，返回步驟1重新開始")
                            retry_count = 0  # 重置重試計數器
                            continue  # 跳過本輪循環，回到步驟1
                
                if not result_c:
                    print("\n⚠ c.PNG 未檢測到，返回步驟1重新開始")
                    retry_count = 0  # 重置重試計數器
                    continue  # 跳過本輪循環，回到步驟1
                
                # 解包結果
                if isinstance(result_c, tuple) and len(result_c) == 2:
                    pos_c, location_c = result_c
                else:
                    pos_c = result_c
                    location_c = None
                
                # 驗證識別到的 c.PNG 是否正確
                # 修改原因：用戶反饋識別位置不正確，需要驗證是否為真正的 c.PNG
                if location_c:
                    print("\n[驗證 c.PNG] 開始驗證識別結果...")
                    verification_result = self.verify_detected_image(location_c, self.image_c, 'c.PNG')
                    
                    # 修改原因：不要暂停，持續運行，將驗證結果記錄但繼續執行
                    if not verification_result['is_valid']:
                        print(f"\n⚠ c.PNG 驗證失敗: {verification_result['reason']}")
                        print(f"  識別位置: ({pos_c[0]}, {pos_c[1]})")
                        print(f"  可點擊: {'是' if verification_result['is_clickable'] else '否'}")
                        print(f"  → 驗證失敗但仍繼續執行（可能是相似度稍低或非 pointer cursor）")
                    elif not verification_result['is_clickable']:
                        print(f"\n⚠ c.PNG 識別正確，但不是可點擊區域")
                        print(f"  識別位置: ({pos_c[0]}, {pos_c[1]})")
                        print(f"  → 可能需要調整點擊偏移量，但繼續執行")
                    else:
                        print(f"\n✓ c.PNG 驗證通過！")
                        print(f"  相似度驗證: 通過")
                        print(f"  可點擊驗證: 通過")
                        
                        # 驗證通過，保存座標到緩存（修改原因：記錄已確認的座標供下次使用）
                        self.update_coord_cache('c.PNG', pos_c[0], pos_c[1], verified=True)
                    
                    # 更新區域
                    self.update_region(location_c, 'c', expand_pixels=50)
                
                # 點擊 c.PNG 座標 + (10, 5) - 修改原因：用户反馈偏移太大，点击不到目标
                print(f"\n[步驟 3] 執行點擊動作")
                click_x = int(pos_c[0]) + 10
                click_y = int(pos_c[1]) + 5
                print(f"  計算點擊座標: c.PNG座標 {pos_c} + (10, 5) = ({click_x}, {click_y})")
                self.click(click_x, click_y, "點擊 c.PNG 座標 + (10, 5)")
                print(f"[步驟 3] 點擊完成，等待 5 秒...")
                time.sleep(5.0)  # 點擊後等待 5 秒
                
                # 步驟 3.5: 點擊 c 後，等待 d.PNG 出現（表示執行成功）
                # 修改原因：用戶要求 d=已執行，點擊c後必須等待d出現才算成功
                print("\n[步驟 3.5] 等待 d.PNG 出現（確認執行成功）")
                if self.image_d:
                    # 等待 d.PNG 出現（最多等待 30 秒）
                    print("  → 開始等待 d.PNG 出現...")
                    start_wait_time = time.time()
                    result_d = None
                    
                    while time.time() - start_wait_time < 30:
                        # 擴大搜索區域並提高相似度要求，避免誤檢測
                        expanded_region_a = (
                            max(0, self.region_a[0] - 200),
                            max(0, self.region_a[1] - 200),
                            self.region_a[2] + 400,
                            self.region_a[3] + 400
                        )
                        result_d = self.detect_image(self.image_d, region=expanded_region_a, confidence=0.95, return_location=True, size_tolerance=0.10)
                        
                        if result_d:
                            break
                        
                        # 每秒檢查一次
                        time.sleep(1.0)
                        if int(time.time() - start_wait_time) % 5 == 0:
                            print(f"  ... 等待中... (已等待 {int(time.time() - start_wait_time)} 秒)")
                    
                    if result_d:
                        pos_d = (result_d[0], result_d[1])
                        location_d = result_d[2] if len(result_d) > 2 else None
                        print(f"  ✓ d.PNG 已出現（執行成功），座標: {pos_d}")
                        if location_d:
                            print(f"     檢測區域: ({location_d.left}, {location_d.top}) ~ "
                                  f"({location_d.left + location_d.width}, {location_d.top + location_d.height})")
                        
                        # 如果檢測到 d，更新區域和緩存
                        if location_d:
                            self.update_region(location_d, 'd', expand_pixels=50)
                            # 修改原因：記錄 d.PNG 的座標到緩存
                            self.update_coord_cache('d.PNG', pos_d[0], pos_d[1], verified=True)
                        
                        print("  → 執行成功！等待 d 變成 a（準備下一輪）...")
                        
                        # 等待 d 消失，a 出現
                        print("\n[步驟 3.6] 等待 d 消失，a 出現")
                    else:
                        print(f"  ✗ 等待超時（30秒），d.PNG 未出現")
                        print(f"  → 可能點擊 c 失敗或座標不正確")
                        print(f"  → 跳過本輪循環，回到步驟 1")
                        continue  # 回到步驟 1，重新開始
                else:
                    print("  → d.PNG 檔案不存在，無法確認執行狀態")
                
                # 如果有 d.PNG 且已確認出現，才執行等待 d 消失的邏輯
                if self.image_d and result_d:
                    while self.running:
                            # 每次循環都重新計算檢測區域（使用最新的 region_a），避免使用舊的暫存區域
                            # 檢查 d 是否還在（提高相似度要求，避免誤檢測）
                            expanded_region_a = (
                                max(0, self.region_a[0] - 200),
                                max(0, self.region_a[1] - 200),
                                self.region_a[2] + 400,
                                self.region_a[3] + 400
                            )
                            print(f"  → 使用最新計算的檢測區域: ({expanded_region_a[0]}, {expanded_region_a[1]}) ~ "
                                  f"({expanded_region_a[0]+expanded_region_a[2]}, {expanded_region_a[1]+expanded_region_a[3]})")
                            result_d_check = self.detect_image(self.image_d, region=expanded_region_a, confidence=0.95, return_location=True, size_tolerance=0.10)
                            if not result_d_check:
                                # d 已消失，檢查 a 是否出現
                                print("  → d 已消失，檢查 a 是否出現...")
                                # 檢測 a.PNG 時使用高相似度和嚴格的大小驗證，避免誤判為 d.PNG
                                expanded_region_a_check = (
                                    max(0, self.region_a[0] - 100),
                                    max(0, self.region_a[1] - 100),
                                    self.region_a[2] + 200,
                                    self.region_a[3] + 200
                                )
                                result_a_check = self.detect_image(self.image_a, region=expanded_region_a_check, confidence=0.98, return_location=True, size_tolerance=0.05)
                                if result_a_check:
                                    pos_a_check = (result_a_check[0], result_a_check[1])
                                    location_a_check = result_a_check[2] if len(result_a_check) > 2 else None
                                    print(f"  ✓ a 已出現，座標: {pos_a_check}")
                                    
                                    # 更新 a 的區域
                                    if location_a_check:
                                        self.update_region(location_a_check, 'a', expand_pixels=50)
                                    
                                    # 回到步驟 1，繼續循環
                                    break
                                else:
                                    print("  ... 等待 a 出現...")
                                    time.sleep(0.5)
                            else:
                                # d 還在，打印檢測到的座標和當前時間
                                pos_d_check = (result_d_check[0], result_d_check[1])
                                location_d_check = result_d_check[2] if len(result_d_check) > 2 else None
                                current_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                                print(f"  → d 還在，檢測到座標: {pos_d_check}，當前時間: {current_time}")
                                if location_d_check:
                                    print(f"     檢測區域: ({location_d_check.left}, {location_d_check.top}) ~ "
                                          f"({location_d_check.left + location_d_check.width}, {location_d_check.top + location_d_check.height})")
                                print(f"  → 等待 1 分鐘後再檢查...")
                                wait_start_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                                print(f"     等待開始時間: {wait_start_time}")
                                time.sleep(60.0)  # 等待 60 秒
                                wait_end_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                                print(f"  → 1 分鐘已過，等待結束時間: {wait_end_time}，重新檢查 d 狀態...")
                                # 添加小延遲，確保屏幕已更新，並強制重新計算檢測區域（不使用緩存）
                                time.sleep(0.5)  # 等待屏幕更新
                                print(f"  → 重新計算檢測區域（使用最新的 region_a），避免使用舊的暫存區域...")
                
                # 回到步驟 1，繼續循環
                current_time = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
                print(f"\n[循環 #{cycle_count}] 完成")
                print(f"完成時間: {current_time}")
                print("準備開始下一輪循環，等待 0.5 秒...")
                time.sleep(0.5)  # 循環間隔
                
        except KeyboardInterrupt:
            print("\n\n程式已停止")
        except pyautogui.FailSafeException:
            print("\n\n緊急停止：滑鼠移到螢幕角落")
        except Exception as e:
            print(f"\n\n發生錯誤: {e}")
            import traceback
            traceback.print_exc()

if __name__ == "__main__":
    clicker = AutoClicker()
    clicker.run()

