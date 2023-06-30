using ozakboy.NLOG;
Console.WriteLine("Hello, World!");
LOG.SetLogKeepDay(-7);
LOG.Trace_Log("LOG_TEST");
LOG.Info_Log("LOG_TEST", false);
LOG.Debug_Log("LOG_TEST");
LOG.Error_Log("LOG_TEST");
LOG.Warn_Log("LOG_TEST");
LOG.Fatal_Log("LOG_TEST");
LOG.CostomName_Log("ABC", "LOG_TEST");

for (int i = 0; i < 1000000; i++)
{
    LOG.CostomName_Log("BigFile_1", "LOG_TEST");
}

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