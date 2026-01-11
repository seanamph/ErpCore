# ErpCore Web 前端编译说明

## 方法一：使用 PowerShell 脚本（推荐）

如果 Node.js 已正确安装并添加到 PATH，请执行：

```powershell
cd ErpCore
.\build-web.ps1
```

## 方法二：手动编译

### 步骤 1：打开新的 PowerShell 或命令提示符

**重要**：请重新打开一个新的 PowerShell 窗口或命令提示符，以确保环境变量已更新。

### 步骤 2：验证 Node.js 安装

```powershell
node --version
npm --version
```

如果显示版本号，说明 Node.js 已正确安装。

### 步骤 3：进入 Web 项目目录

```powershell
cd ErpCore\src\ErpCore.Web
```

### 步骤 4：安装依赖

```powershell
npm install
```

这可能需要几分钟时间，会下载所有需要的依赖套件。

### 步骤 5：编译项目

```powershell
npm run build
```

编译完成后，生成的文件将位于 `dist` 目录中。

## 方法三：如果 Node.js 不在 PATH 中

### 查找 Node.js 安装位置

1. 检查常见安装位置：
   - `C:\Program Files\nodejs\`
   - `C:\Program Files (x86)\nodejs\`
   - `%LOCALAPPDATA%\Programs\nodejs\`

2. 或者使用文件搜索功能查找 `node.exe`

### 使用完整路径执行

找到 Node.js 安装目录后（例如：`C:\Program Files\nodejs\`），执行：

```powershell
cd ErpCore\src\ErpCore.Web
& "C:\Program Files\nodejs\npm.cmd" install
& "C:\Program Files\nodejs\npm.cmd" run build
```

## 编译输出

编译成功后，生成的文件将位于：
```
ErpCore\src\ErpCore.Web\dist\
```

这个目录包含所有编译后的静态文件，可以部署到 Web 服务器。

## 常见问题

### 1. 找不到 node 或 npm 命令

**解决方案**：
- 重新启动 PowerShell 或命令提示符
- 确认 Node.js 已正确安装
- 检查环境变量 PATH 是否包含 Node.js 安装目录

### 2. npm install 失败

**解决方案**：
- 检查网络连接
- 尝试清除 npm 缓存：`npm cache clean --force`
- 删除 `node_modules` 文件夹后重新安装

### 3. 编译错误

**解决方案**：
- 检查 `package.json` 中的依赖版本
- 确保所有依赖都已正确安装
- 查看错误信息并修复相关问题

## 开发模式运行

如果需要以开发模式运行（支持热重载），可以使用：

```powershell
cd ErpCore\src\ErpCore.Web
npm run serve
```

这将启动开发服务器，通常运行在 `http://localhost:8080`

## 需要帮助？

如果遇到问题，请检查：
1. Node.js 版本是否符合要求（建议 v16 或以上）
2. 网络连接是否正常
3. 是否有足够的磁盘空间
