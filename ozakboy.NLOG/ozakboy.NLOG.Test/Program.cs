using ozakboy.NLOG;
Console.WriteLine("Hello, World!");
LOG.SetLogKeepDay(-7);
LOG.Trace_Log("LOG_TEST");
LOG.Info_Log("LOG_TEST");
LOG.Debug_Log("LOG_TEST");
LOG.Error_Log("LOG_TEST");
LOG.Warn_Log("LOG_TEST");
LOG.Fatal_Log("LOG_TEST");
LOG.CostomName_Log("ABC","LOG_TEST");

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