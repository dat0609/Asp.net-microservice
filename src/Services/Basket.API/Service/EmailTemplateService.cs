﻿using System.Text;
using Basket.API.Service.Interface;
using Shared.Configurations;

namespace Basket.API.Service;

public class EmailTemplateService
{
    protected readonly BackgroundJobSettings _backgroundJobSettings;
    
    public EmailTemplateService(BackgroundJobSettings backgroundJobSettings)
    {
        _backgroundJobSettings = backgroundJobSettings;
    }
    
    private static readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string _tmpFolder = Path.Combine(_baseDir, "EmailTemplate");
    
    protected string ReadEmailTemplate(string templateName, string format = "html")
    {
        var templatePath = Path.Combine(_tmpFolder, templateName + "." + format);
        
        using var fs = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        
        var emailText = sr.ReadToEnd();
        sr.Close();
        
        return emailText;
    }
}