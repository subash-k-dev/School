using System;

namespace School.Web.Services;

public class ToastService
{
    public event Action<string, string>? OnShow;
    public event Action? OnHide;

    public void ShowSuccess(string message) => OnShow?.Invoke(message, "success");
    public void ShowError(string message) => OnShow?.Invoke(message, "danger");
    public void ShowInfo(string message) => OnShow?.Invoke(message, "info");
    public void ShowWarning(string message) => OnShow?.Invoke(message, "warning");

    public void Hide() => OnHide?.Invoke();
}