using ozakboy.NLOG;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
Console.WriteLine("Hello, World!");
LOG.SetLogKeepDay(-7);
LOG.Trace_Log("LOG_TEST");
LOG.Info_Log("LOG_TEST", false);
LOG.Debug_Log("LOG_TEST");
LOG.Error_Log("LOG_TEST");
LOG.Warn_Log("LOG_TEST");
LOG.Fatal_Log("LOG_TEST");
LOG.CostomName_Log("ABC", "LOG_TEST");



try
{
    throw new ErrorMessageException("發生錯誤");
}
catch (ErrorMessageException ex)
{
    LOG.Warn_Log(ex);
}

try
{
    throw new ErrorMessageException("發生錯誤");
}
catch (Exception ex)
{
    LOG.Warn_Log(ex);
}

try
{
    throw new Exception("發生錯誤");
}
catch (Exception ex)
{
    LOG.Warn_Log(ex);
}

string[] argsb = new[] { "參數1", "參數2" };
LOG.Info_Log("格式化訊息: {0}, {1}", true, argsb);
var jsonData = new
{
    abc = "abc",
    num = 123,
    cob = new[]
    {
        new { name = "find", arg = 10 },
        new { name = "find", arg = 10 }
    }
};

LOG.Info_Log(JsonSerializer.Serialize(jsonData));

//for (int i = 0; i < 1000000; i++)
//{
//    LOG.CostomName_Log("BigFile_1", "LOG_TEST");
//}