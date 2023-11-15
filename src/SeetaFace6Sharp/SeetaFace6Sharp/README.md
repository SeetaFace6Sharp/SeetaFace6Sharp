## 1. 关于
- 一个基于 [SeetaFace6](https://github.com/SeetaFace6Open/index) 的 .NET 离线人脸识别解决方案
- 此项目来源于ViewFaceCore，在ViewFaceCore基础上二次开发与分发
- 开源、免费、跨平台 (win/linux)

## 2. 快速开始
### 2.1 受支持的 .NET 框架 和 操作系统  

   | 目标框架 | 最低版本 | 操作系统 |
   | :-: |:-: | :-: |
   | .NET Framework |4.0 | win ( x64/x86 ) |
   | .NET Standard |2.0 | win ( x64/x86 ) |
   | .NET / .NET Core |3.1+ | win ( x64/x86 )、linux ( arm/arm64/x64 ) |

### 2.2 简单的人脸信息检测  
以 Windows x64平台 为例，一个简单的人脸检测Demo。
1. 使用 [nuget](https://www.nuget.org) 安装依赖  

| 包名称 | 最小版本 | 生成文件夹 | 说明 |
| :- | :-: | - | - |
| [SeetaFace6Sharp](https://www.nuget.org/packages/SeetaFace6Sharp/) | [![](https://img.shields.io/nuget/v/SeetaFace6Sharp.svg)](https://www.nuget.org/packages/SeetaFace6Sharp) | —— | SeetaFace6Sharp .NET 核心库 |
| [SeetaFace6Sharp.model.all](https://www.nuget.org/packages/SeetaFace6Sharp.model.all) | [![](https://img.shields.io/nuget/v/SeetaFace6Sharp.model.all.svg)](https://www.nuget.org/packages/SeetaFace6Sharp.model.all) | `runtimes\models` | 人脸检测的模型支持(图省事可以直接安装这个) |
| [SeetaFace6Sharp.runtime.win.x64](https://www.nuget.org/packages/SeetaFace6Sharp.runtime.win.x64) | [![](https://img.shields.io/nuget/v/SeetaFace6Sharp.runtime.win.x64.svg)](https://www.nuget.org/packages/SeetaFace6Sharp.runtime.win.x64) | `runtimes\win-x64\native` | Windows-x64 的本机运行时，其它平台自行选择安装，可安装多个 |
| [SeetaFace6Sharp.Extension.SkiaSharp](https://www.nuget.org/packages/SeetaFace6Sharp.Extension.SkiaSharp) | [![](https://img.shields.io/nuget/v/SeetaFace6Sharp.Extension.SkiaSharp.svg)](https://www.nuget.org/packages/SeetaFace6Sharp.Extension.SkiaSharp) |  —— | SkiaSharp图像处理扩展，ImageSharp、SkiaSharp、System.Drawing三选一 |

2. 获取人脸信息  
```csharp
using SkiaSharp;
using System;
using SeetaFace6Sharp;

namespace SeetaFace6Sharp.Example.ConsoleApp
{
    internal class Program
    {
        private readonly static string imagePath = @"images/Jay_3.jpg";

        static void Main(string[] args)
        {
            using var bitmap = SKBitmap.Decode(imagePath);
            using FaceDetector faceDetector = new FaceDetector();
            FaceInfo[] infos = faceDetector.Detect(bitmap);
            Console.WriteLine($"识别到的人脸数量：{infos.Length} 个人脸信息：\n");
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

更多案例参见 `src/Examples`

## 3. 使用许可   
[Copyright (c) 2021, SeetaFace6Sharp](https://github.com/SeetaFace6Sharp/SeetaFace6Sharp/blob/main/LICENSE) | [*Copyright (c) 2019, SeetaTech*](https://github.com/SeetaFace6Open/index/blob/master/LICENSE)

> [\[源\]](https://github.com/SeetaFace6Open/index#%E8%81%94%E7%B3%BB%E6%88%91%E4%BB%AC) > *`SeetaFace` 开源版可以免费用于商业和个人用途。如果需要更多的商业支持，请联系商务邮件 bd@seetatech.com*
