﻿namespace Shared.DTOs.ScheduleJob;

public record ReminderCheckoutOrderDto(string email, string subject, string content, DateTimeOffset enqueue);