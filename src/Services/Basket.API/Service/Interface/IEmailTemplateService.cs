﻿namespace Basket.API.Service.Interface;

public interface IEmailTemplateService
{
    string GenerateReminderEmail(string username);
}