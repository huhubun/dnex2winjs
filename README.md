# dnex2winjs (dnex.js)
dnex 的含义是 **Dot Net EXceptions**。通过 dnex.js 可以让 WinJS 编写的 Windows 商店应用更方便的处理 Windows 运行时组件引发的异常。<br />
在使用过程中存在一些限制，详细内容请参考下文的“注意事项”一节

## 为什么使用 dnex.js
由于 WinJS 编写的 Windows 商店应用不能获取 Windows 运行时组件中引发的异常的类型，只能获得堆栈跟踪信息文本，这使得在 WinJS 中难以判断 Windows 运行时组件引发了什么异常。<br />
dnex.js 记录了 mscorlib.dll 中定义的所有异常，并通过 dnex 类提供给 WinJS，使得在 WinJS 中判断 Windows 运行时组件引发的异常类型变得十分容易。

## 使用 dnex.js
可以通过 NuGet 直接下载 dnex.js，也可以通过 dnex2winjs 项目来生成，有关如何生成请参考下文的“生成 dnex.js”一节
```powershell
Install-Package dnex.js
```

随后，在 WinJS 应用的模板页（如 `default.html`）中添加 dnex.js 的引用
```javascript
<script src="/js/dnex.js"></script>
```

这样便可以在 try..catch 中通过捕获 `WinRTError`（即示例代码中的 e），并通过它的 `number` 属性与 `dnex` 中列出的异常名称属性相比较，得出 Windows 运行时引发的异常的类型
```javascript
try {
    // 调用 Windows 运行时组件中定义的方法
}
catch (e) {
    if (e.number == dnex.ArgumentNullException) {
        // 处理 ArgumentNullException 异常
    }
}
```

## dnex.js 的原理
dnex.js 通过 `HResult` 值来判断异常类型：每种异常都有一个 `HResult` 属性，例如 *ArgumentNullException* 的 `HResult` 为 0x80004003（-2147467261）、*FormatException* 的 `HResult` 为 0x80131537（-2146233033）等等。<br />
dnex2winjs 项目通过反射，获取 mscorlib.dll 中，所有由 `Exception` 类派生的类（包括其自身）的类名以及实例中 HResult 的值，并通过 [T4 模板](https://msdn.microsoft.com/zh-cn/library/bb126445.aspx) 生成 dnex.js。<br />
EN:[T4 Text Templates](https://msdn.microsoft.com/en-us/library/bb126445.aspx)

## 生成 dnex.js
* 使用 Visual Studio 2015 或更高版本打开解决方案，通过“解决方案资源管理器（Solution Explorer）”找到 **dnex2winjs** 项目。右键单击该项目，选择“生成”
* 生成成功后展开 **Template** 目录，找到 **dnex.tt** 文件。右键单击该文件，选择“运行自定义工具（Run Custom Tool）”，或打开文件，执行保存操作
* 若没有发生任何错误，那么“解决方案资源管理器”中的 **dnex.tt** 文件前方会显示箭头，展开该箭头便能找到生成的 **dnex.js**

## 注意事项
* 本文中 WinJS 的运行环境均为使用 WinJS 编写的 Windows 商店应用，无法在 WinJS 制作的网页中使用 Windows 运行时组件或捕获 Windows 运行时异常
* 只支持在 C# 或 Visual Basic 编写的 Windows 运行时组件中引发的，并且不是自定义的异常。详细内容请参考 MSDN 文章《[使用 C# 和 Visual Basic 创建 Windows 运行时组件](https://msdn.microsoft.com/zh-cn/library/windows/apps/xaml/mt609005.aspx)》中“引发异常”一节。<br />
* EN:《[Creating Windows Runtime Components in C# and Visual Basic](https://msdn.microsoft.com/en-us/windows/uwp/winrt-components/creating-windows-runtime-components-in-csharp-and-visual-basic)》:"Throwing exceptions"
* **十分重要**：一些异常的 `HResult` 值是相同的！这意味着若一个方法可能引发多个不同异常，而这些异常使用同一个 `HResult`，那么 dnex.js 将无力分辨具体引发了哪一个异常。下表列出了使用相同 HResult 值的异常
<table>
		<tr>
			<td rowspan="2">
				<b>-2146233029</b><br />
				(0x8013153B)
			</td>
			<td>OperationCanceledException</td>
		</tr>
		<tr>
			<td>TaskCanceledException</td>
		</tr>
		<tr>
			<td rowspan="2">
				<b>-2146233077</b><br />
				(0x8013150B)
			</td>
			<td>RemotingException</td>
		</tr>
		<tr>
			<td>RemotingTimeoutException</td>
		</tr>
		<tr>
			<td rowspan="4">
				<b>-2146233087</b><br />
				(0x80131501)
			</td>
			<td>SystemException</td>
		</tr>
		<tr>
			<td>HostProtectionException</td>
		</tr>
		<tr>
			<td>IdentityNotMappedException</td>
		</tr>
		<tr>
			<td>SemaphoreFullException</td>
		</tr>
		<tr>
			<td rowspan="7">
				<b>-2146233088</b><br />
				(0x80131500)
			</td>
			<td>AggregateException</td>
		</tr>
		<tr>
			<td>Exception</td>
		</tr>
		<tr>
			<td>InvalidTimeZoneException</td>
		</tr>
		<tr>
			<td>TimeZoneNotFoundException</td>
		</tr>
		<tr>
			<td>EventSourceException</td>
		</tr>
		<tr>
			<td>LockRecursionException</td>
		</tr>
		<tr>
			<td>TaskSchedulerException</td>
		</tr>
		<tr>
			<td rowspan="4">
				<b>-2147024809</b><br />
				(0x80070057)
			</td>
			<td>ArgumentException</td>
		</tr>
		<tr>
			<td>CultureNotFoundException</td>
		</tr>
		<tr>
			<td>DecoderFallbackException</td>
		</tr>
		<tr>
			<td>EncoderFallbackException</td>
		</tr>
		<tr>
			<td rowspan="2">
				<b>-2147024891</b><br />
				(0x80070005)
			</td>
			<td>UnauthorizedAccessException</td>
		</tr>
		<tr>
			<td>PrivilegeNotHeldException</td>
		</tr>
		<tr>
			<td rowspan="2">
				<b>-2147024893</b><br />
				(0x80070003)
			</td>
			<td>DirectoryNotFoundException</td>
		</tr>
		<tr>
			<td>DriveNotFoundException</td>
		</tr>
		<tr>
			<td rowspan="3">
				<b>-2147467259</b><br />
				(0x80004005)
			</td>
			<td>COMException</td>
		</tr>
		<tr>
			<td>ExternalException</td>
		</tr>
		<tr>
			<td>SEHException</td>
		</tr>
		<tr>
			<td rowspan="3">
				<b>-2147467261</b><br />
				(0x80004003)
			</td>
			<td>AccessViolationException</td>
		</tr>
		<tr>
			<td>ArgumentNullException</td>
		</tr>
		<tr>
			<td>NullReferenceException</td>
		</tr>
</table>

## License
MIT License
