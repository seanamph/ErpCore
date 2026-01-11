# auto_clicker.py 问题诊断

## 问题现象
执行 `python .\auto_clicker.py` 后程序立即退出，没有看到错误信息。

## 可能原因

### 1. 图片文件名不匹配
程序需要以下图片文件：
- `a.png` 或 `a.PNG`
- `b.PNG` 或 `b.png`
- `c.PNG` 或 `c.png`

**检查方法**：
```powershell
Get-ChildItem *.png, *.PNG
```

如果文件名不匹配，程序会在初始化时设置 `self.running = False` 并退出。

### 2. Python 模块未安装
程序需要以下 Python 模块：
- `pyautogui`
- `PIL` (Pillow)
- `numpy`
- `opencv-python` (可选，但建议安装)
- `pywin32` (可选)

**检查方法**：
```powershell
python -c "import pyautogui; print('OK')"
```

**安装命令**：
```powershell
pip install pyautogui Pillow numpy opencv-python pywin32
```

### 3. Python 输出被重定向
如果 Python 输出被重定向或缓冲，可能看不到错误信息。

**解决方法**：
```powershell
python -u auto_clicker.py
```

或者使用批处理文件：
```batch
@echo off
python -u auto_clicker.py
pause
```

### 4. 程序在初始化时遇到异常
程序可能在 `__init__` 方法中遇到未捕获的异常。

## 诊断步骤

### 步骤 1：检查图片文件
```powershell
# 检查当前目录的图片文件
Get-ChildItem *.png, *.PNG | Select-Object Name
```

应该看到：
- a.PNG (或 a.png)
- b.PNG
- c.PNG
- d.PNG (可选)
- e.PNG (可选)

### 步骤 2：检查 Python 模块
```powershell
python -c "import pyautogui, PIL, numpy; print('All modules OK')"
```

### 步骤 3：使用调试模式运行
```powershell
python -u -v auto_clicker.py 2>&1 | Tee-Object output.txt
Get-Content output.txt
```

### 步骤 4：检查程序初始化
创建一个测试脚本 `test_init.py`：
```python
import sys
print("Starting...")
try:
    from auto_clicker import AutoClicker
    print("Import OK")
    clicker = AutoClicker()
    print(f"Init OK, running={clicker.running}")
except Exception as e:
    print(f"Error: {e}")
    import traceback
    traceback.print_exc()
```

## 快速修复

如果图片文件名不匹配，可以：
1. 重命名文件：将 `a.PNG` 重命名为 `a.png`（如果程序需要小写）
2. 或者修改程序：程序已经支持不区分大小写的查找

## 建议

1. **使用新的 PowerShell 窗口**：重新打开 PowerShell，确保环境变量已更新
2. **检查 Python 安装**：确认 Python 已正确安装
3. **查看完整输出**：使用 `python -u` 参数禁用输出缓冲
4. **检查依赖**：确保所有必要的 Python 模块已安装
