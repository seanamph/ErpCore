#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
滑鼠座標偵測程式
持續監控滑鼠位置，當滑鼠移動時顯示座標
"""

import time
from pynput import mouse

class MouseTracker:
    def __init__(self):
        self.last_x = None
        self.last_y = None
        
    def on_move(self, x, y):
        """當滑鼠移動時觸發"""
        # 檢查是否真的有移動（避免重複打印相同位置）
        if self.last_x != x or self.last_y != y:
            print(f"滑鼠座標: ({x}, {y})")
            self.last_x = x
            self.last_y = y
    
    def start(self):
        """開始監聽滑鼠移動"""
        print("開始監控滑鼠座標... (按 Ctrl+C 停止)")
        print("-" * 50)
        
        # 獲取初始位置
        current_pos = mouse.Controller().position
        self.last_x, self.last_y = current_pos
        print(f"初始座標: ({self.last_x}, {self.last_y})")
        print("-" * 50)
        
        # 監聽滑鼠移動事件
        with mouse.Listener(on_move=self.on_move) as listener:
            try:
                listener.join()  # 持續運行直到停止
            except KeyboardInterrupt:
                print("\n程式已停止")

if __name__ == "__main__":
    tracker = MouseTracker()
    tracker.start()

