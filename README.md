<div align="center">

# SeetaFace6Sharp  

[![Nuget](https://img.shields.io/nuget/v/SeetaFace6Sharp?color=%233F48CC&style=flat-square)](https://www.nuget.org/packages/SeetaFace6Sharp/) &nbsp;&nbsp;
[![GitHub license](https://img.shields.io/github/license/SeetaFace6Sharp/SeetaFace6Sharp?style=flat-square)](https://github.com/SeetaFace6Sharp/SeetaFace6Sharp/blob/main/LICENSE) &nbsp;&nbsp;
![GitHub stars](https://img.shields.io/github/stars/SeetaFace6Sharp/SeetaFace6Sharp?color=%23FCD53F&style=flat-square) &nbsp;&nbsp;
![GitHub forks](https://img.shields.io/github/forks/SeetaFace6Sharp/SeetaFace6Sharp?style=flat-square)

<br/>

—— [💎 关于](#-关于) &nbsp;| [⭐ 快速开始](#-快速开始) &nbsp;| [🚀 性能](#-性能) &nbsp;| [🔧 构建](#-构建) &nbsp;| [📦 包清单](#-包清单) &nbsp;| [🐟 API文档](#-api文档) &nbsp; ——
<br/>
—— [🔎 参考](#-参考) &nbsp;| [❓ 问答](#-问答) &nbsp;| [🧩 贡献](#-贡献) &nbsp;| [📄 许可](#-许可) &nbsp; ——

</div>

## 💎 关于
- 一个基于 [SeetaFace6](https://github.com/SeetaFace6Open/index) 的 .NET 离线人脸识别解决方案
- 此项目来源于ViewFaceCore，在ViewFaceCore基础上二次开发与分发
- 开源、免费、跨平台

受支持的 .NET Runtime 和 操作系统  

| OS  |  Runtime   |  x86  |  x64  |  ARM  | ARM64  | LoongArch64  |
| ------------ | ------------ | :------------: | :------------: | :------------: | :------------: | :------------: |
| Windows  | .NET Framework  |  √  |  √  |     |      |      |
| Windows  | .NET Core 3.1+  |  √  |  √  |     |      |      |
| Linux    | .NET Core 3.1+  |     |  √  |  √  |  √   |  √   |


**注意：** Windows 系统需要安装Visual C++ 14，下载链接：https://learn.microsoft.com/zh-CN/cpp/windows/latest-supported-vc-redist

## ⭐ 快速开始

- [Examples](/src/SeetaFace6Sharp/Examples)  

- 在 *Windows x64* 下, 快速集成人脸检测  

  1. 创建控制台项目
  1. 使用 [Nuget](https://www.nuget.org) 安装以下依赖

	| 包名称  | 版本  | 说明  |
	| :------------ | :------------: | :------------ |
	| [SeetaFace6Sharp](https://www.nuget.org/packages/SeetaFace6Sharp/)                                         | ![Version](https://img.shields.io/nuget/v/SeetaFace6Sharp.svg?color=%233F48CC&label=%20&style=flat-square)                         | *SeetaFace6Sharp* 核心包       |
	| [SeetaFace6Sharp.model.face_detector](https://www.nuget.org/packages/SeetaFace6Sharp.model.face_detector)   | ![Version](https://img.shields.io/nuget/v/SeetaFace6Sharp.model.face_detector.svg?color=%233F48CC&label=%20&style=flat-square)     | *人脸检测* 模型包           |
	| [SeetaFace6Sharp.runtime.win.x64](https://www.nuget.org/packages/SeetaFace6Sharp.runtime.win.x64)           | ![Version](https://img.shields.io/nuget/v/SeetaFace6Sharp.runtime.win.x64.svg?color=%233F48CC&label=%20&style=flat-square)         | *Windows-x64* 运行时包      |
	| [SeetaFace6Sharp.Extension.SystemDrawing](https://www.nuget.org/packages/SeetaFace6Sharp.Extension.SystemDrawing) | ![Version](https://img.shields.io/nuget/v/SeetaFace6Sharp.Extension.SystemDrawing.svg?color=%233F48CC&label=%20&style=flat-square) | *System.Drawing* 图像扩展包 |
  
  1. 获取人脸信息
  
  ```csharp
	using SeetaFace6Sharp;
	using System;
	using System.Drawing;

	namespace ConsoleApp1
	{
		internal class Program
		{
			static void Main(string[] args)
			{
				string imagePath = @"images/Jay_3.jpg";
				using var bitmap = (Bitmap)Image.FromFile(imagePath);
				using var imgData = bitmap.ToFaceImage();
				using FaceDetector faceDetector = new FaceDetector();
				FaceInfo[] infos = faceDetector.Detect(imgData);
				Console.WriteLine($"识别到 {infos.Length} 个人脸信息：\n");
				Console.WriteLine($"No.\t人脸置信度\t位置信息");
				for (int i = 0; i < infos.Length; i++)
				{
					Console.WriteLine($"{i}\t{infos[i].Score:f8}\t{infos[i].Location}");
				}
				Console.ReadKey();
			}
		}
	}
  ```

## 🚀 性能

| 测试项目  | 特征长度  | 速度（AMD 5950x）  | 速度（Intel N305）  | 速度（NVDIA GTX1650）  | 速度（RK3588）  | 速度（3A6000）  |  
| ------------ | ------------ | ------------ | ------------ | ------------ | ------------ | ------------ |  
| 特征值检测               |  1024 | 14ms  | 42ms  | 7ms  |  未测试 |  86ms |  
| 人脸比对（从跟踪到比对）  |  1024 | 54ms  | 96ms  | 18ms  | 未测试  |  165ms |  

## 🔧 构建
   
- [*SeetaFace6 构建*](/docs/SeetaFace6OpenBuild.md)
- [*SeetaFace6Sharp 构建*](/docs/SeetaFace6SharpBuild.md)

## 📦 包清单

- [*SeetaFace6Sharp 的 Nuget 包清单*](/docs/SeetaFace6SharpPackages.md)

## 🐟 API文档

- [*SeetaFace6Sharp API*](/docs/SeetaFace6SharpAPI.md)

## 🔎 参考
- [*SeetaFace6 说明*](https://github.com/seetafaceengine/SeetaFace6/blob/master/README.md)  
- [*SeetaFace 各接口说明*](https://github.com/seetafaceengine/SeetaFace6/tree/master/docs)  
- [*SeetaFace 入门教程*](http://leanote.com/blog/post/5e7d6cecab64412ae60016ef)  


## ❓ 问答

- [Issues](https://github.com/SeetaFace6Sharp/SeetaFace6Sharp/issues)  
- [常见问题](/docs/QA.md)  

## 🧩 贡献

- [PR](https://github.com/SeetaFace6Sharp/SeetaFace6Sharp/pull)  
- [参与贡献](/docs/Contribute.md)  

## 📄 许可 
<div align="center">

[Copyright (c) 2021, SeetaFace6Sharp](https://github.com/SeetaFace6Sharp/SeetaFace6Sharp/blob/main/LICENSE) | [*Copyright (c) 2019, SeetaTech*](https://github.com/SeetaFace6Open/index/blob/master/LICENSE)

</div>

> *[SeetaFace 开源版](https://github.com/SeetaFace6Open/index#%E8%81%94%E7%B3%BB%E6%88%91%E4%BB%AC) 可以免费用于商业和个人用途。如果需要更多的商业支持，请联系商务邮件 bd@seetatech.com*
