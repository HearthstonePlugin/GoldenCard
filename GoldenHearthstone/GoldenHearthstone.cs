using Mono.Cecil;
using Mono.Cecil.Cil;
using HearthstonePath;

// 加载炉石传说路径
string HearthstonePath = PathUtil.FindInstallPathFromRegistry("Hearthstone");
if (!string.IsNullOrEmpty(HearthstonePath) && Directory.Exists(HearthstonePath)
    && File.Exists(Path.Combine(HearthstonePath, "Hearthstone.exe")))
{
    Console.WriteLine("自动获取战网路径成功：");
    Console.WriteLine(HearthstonePath);
}
else
{
    Console.WriteLine("自动获取炉石路径失败，请手动输入");
    Console.WriteLine("请输入炉石传说所在路径，例如：");
    Console.WriteLine(@"C:\Program Files (x86)\Hearthstone");
    HearthstonePath = Console.ReadLine();
}

Console.WriteLine("即将Patch，请确认炉石已经关闭");
Console.Write("任意按键继续...");
Console.Read();

// 文件备份、不校验是否存在Patch
string AssemblyCSharpDLL = HearthstonePath + @"\Hearthstone_Data\Managed\Assembly-CSharp.dll";
System.IO.File.Copy(AssemblyCSharpDLL, AssemblyCSharpDLL + ".bak", true);
Console.WriteLine("当前文件已备份：" + AssemblyCSharpDLL + ".bak");

// 加载依赖，准备Patch备份文件，保存原始文件
var resolver = new DefaultAssemblyResolver();
resolver.AddSearchDirectory(HearthstonePath + @"\Hearthstone_Data\Managed\");
var assembly = AssemblyDefinition.ReadAssembly(AssemblyCSharpDLL + ".bak", new ReaderParameters { AssemblyResolver = resolver });


foreach (TypeDefinition type in assembly.MainModule.Types)
{
    // 金卡特效
    if (type.Name == "Entity")
    {
        foreach (MethodDefinition method in type.Methods)
        {
            if (method.Name == "GetPremiumType")
            {
                method.Body.Instructions[11].OpCode = OpCodes.Ldc_I4_1;
                method.Body.Instructions[13].OpCode = OpCodes.Ldc_I4_1;
            }
        }
    }
}


// 保存DLL
assembly.Write(AssemblyCSharpDLL);

Console.Write("Patch成功，任意按键继续按键退出...");
Console.Write(Console.Read());
