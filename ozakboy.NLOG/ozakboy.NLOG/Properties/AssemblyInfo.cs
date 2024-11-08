using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

// 組件的一般資訊是由下列的屬性集控制。
// 變更這些屬性的值即可修改組件的相關資訊
[assembly: AssemblyCopyright("Copyright © ozakboy 2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 將 ComVisible 設定為 false 會使得這個組件中的類型
// 對 COM 元件而言是不可見的。如果您需要從 COM 存取這個組件中
// 的類型，請在該類型上將 ComVisible 屬性設定為 true。
[assembly: ComVisible(false)]

// 下列 GUID 為專案公開 (Expose) 至 COM 時所要使用的 typelib ID
[assembly: Guid("93EB57B9-FE9A-4A40-954F-C223EA7C2F90")]

// 混淆設定
[assembly: Obfuscation(Feature = "all")]

// 如果需要排除某些類別不被混淆,可以加入以下設定
// [assembly: Obfuscation(Feature = "rename", Exclude = true, ApplyToMembers = true, Scope = "type", Target = "ozakboy.NLOG.LOG")]