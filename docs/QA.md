# 常见问题

1. Error: `DirectoryNotFoundException: Can not found library path.`

   - 请检查对应目录下是否存在 Runtime 依赖, 有时网络问题会导致 Nuget 包下载失败

  
2. Unable to load DLL 'SeetaFace6Bridge' or one of its dependencies

	- 检查nuget包是否下载完全, 编译目标文件夹下面的runtimes文件夹中是否有对应平台的依赖文件, 比如说windows x64平台, 在runtimes文件夹下面应该会有win-x64/native文件夹, 文件夹中有很多*.dll文件。  
	- 缺少vc++依赖, Windows 系统需要安装Visual C++ 14，下载链接：https://learn.microsoft.com/zh-CN/cpp/windows/latest-supported-vc-redist

3. 开始人脸识别时卡死, 然后异常结束
   > 或者报异常：0x00007FFC3FDD104E (tennis.dll) (ConsoleApp1.exe 中)处有未经处理的异常: 0xC000001D: IllegInstruction

	- 参考：[特定指令集支持](/docs/SeetaFace6SharpAPI.md#特定指令集支持)

4. libgomp.so.1: cannot open shared object file: No such file or directory  
   通过包管理器安装：`apt install libgomp1`
   
   
