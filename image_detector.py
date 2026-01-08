#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
螢幕圖案偵測程式
持續偵測螢幕上是否出現指定圖案 (a.png)
只在狀態變化時輸出訊息
"""

import pyautogui
import time
import os

class ImageDetector:
    def __init__(self, image_path='a.png'):
        # 嘗試找到圖片檔案（不區分大小寫）
        if not os.path.exists(image_path):
            # 嘗試尋找不同大小寫的檔案
            base_name = os.path.splitext(image_path)[0]
            ext = os.path.splitext(image_path)[1]
            dir_path = os.path.dirname(image_path) if os.path.dirname(image_path) else '.'
            
            # 在目錄中尋找匹配的檔案
            if os.path.exists(dir_path):
                for file in os.listdir(dir_path):
                    if file.lower() == image_path.lower():
                        image_path = os.path.join(dir_path, file)
                        break
        
        self.image_path = image_path
        self.last_found = False  # 上次是否找到
        self.last_position = None  # 上次找到的座標
        self.running = True
        
        # 檢查圖片檔案是否存在
        if not os.path.exists(image_path):
            print(f"錯誤: 找不到圖片檔案 '{image_path}'")
            print(f"請確認檔案存在於當前目錄: {os.getcwd()}")
            self.running = False
            return
        
        print(f"開始偵測圖案: {image_path}")
        print("程式持續運行中... (按 Ctrl+C 停止)")
        print("-" * 60)
    
    def detect(self):
        """偵測圖案並返回座標"""
        try:
            # 在螢幕上尋找圖案（不使用 confidence 參數，以兼容沒有 OpenCV 的情況）
            location = pyautogui.locateOnScreen(self.image_path)
            
            if location:
                # 計算中心點座標
                center_x = location.left + location.width // 2
                center_y = location.top + location.height // 2
                return (center_x, center_y, location)
            else:
                return None
                
        except pyautogui.ImageNotFoundException:
            return None
        except Exception as e:
            # 只在第一次發生錯誤時顯示，避免重複輸出
            if not hasattr(self, '_error_shown'):
                print(f"注意: {e}")
                print("程式將繼續運行，但可能無法精確匹配圖案")
                self._error_shown = True
            return None
    
    def run(self):
        """持續偵測"""
        if not self.running:
            return
            
        try:
            while self.running:
                position = self.detect()
                
                if position:
                    # 找到圖案
                    x, y, location = position
                    current_pos = (x, y)
                    
                    # 檢查是否為新發現或座標改變
                    if not self.last_found:
                        # 從沒找到變成找到
                        print(f"✓ 找到圖案！座標: ({x}, {y})")
                        self.last_found = True
                        self.last_position = current_pos
                    elif self.last_position != current_pos:
                        # 座標改變
                        print(f"→ 座標改變: ({x}, {y})")
                        self.last_position = current_pos
                    # 如果找到且座標相同，不輸出（避免重複）
                    
                else:
                    # 沒找到圖案
                    if self.last_found:
                        # 從找到變成沒找到
                        print("✗ 沒找到")
                        self.last_found = False
                        self.last_position = None
                    # 如果持續沒找到，不輸出（避免重複）
                
                # 短暫延遲，避免 CPU 使用率過高
                time.sleep(0.1)
                
        except KeyboardInterrupt:
            print("\n程式已停止")
        except Exception as e:
            print(f"\n發生錯誤: {e}")

if __name__ == "__main__":
    # 設定 pyautogui 的安全設定
    pyautogui.FAILSAFE = True  # 滑鼠移到螢幕角落可觸發異常
    
    detector = ImageDetector('a.png')
    detector.run()

