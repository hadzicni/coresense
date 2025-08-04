using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using CoreSense.Services;

namespace CoreSense.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly Timer _timer;
    private readonly HardwareService _hardware;

    private string _cpuTemperature = "--";
    private string _cpuUsage = "--";
    private string _memoryUsed = "--";
    private string _memoryTotal = "--";
    private string _memoryPercent = "--";
    private string _diskUsed = "--";
    private string _diskTotal = "--";
    private string _diskPercent = "--";
    private string _diskFree = "--";

    public string CpuTemperature
    {
        get => _cpuTemperature;
        set => SetField(ref _cpuTemperature, value);
    }

    public string CpuUsage
    {
        get => _cpuUsage;
        set => SetField(ref _cpuUsage, value);
    }

    public string MemoryUsed
    {
        get => _memoryUsed;
        set => SetField(ref _memoryUsed, value);
    }

    public string MemoryTotal
    {
        get => _memoryTotal;
        set => SetField(ref _memoryTotal, value);
    }

    public string MemoryPercent
    {
        get => _memoryPercent;
        set => SetField(ref _memoryPercent, value);
    }

    public string DiskUsed
    {
        get => _diskUsed;
        set => SetField(ref _diskUsed, value);
    }

    public string DiskTotal
    {
        get => _diskTotal;
        set => SetField(ref _diskTotal, value);
    }

    public string DiskPercent
    {
        get => _diskPercent;
        set => SetField(ref _diskPercent, value);
    }

    public string DiskFree
    {
        get => _diskFree;
        set => SetField(ref _diskFree, value);
    }

    public MainWindowViewModel()
    {
        _hardware = new HardwareService();
        _timer = new Timer(5000);
        _timer.Elapsed += UpdateData;
        _timer.Start();
    }

    private void UpdateData(object? sender, ElapsedEventArgs e)
    {
        CpuTemperature = $"{_hardware.GetCpuTemperature()?.ToString("0.0") ?? "--"}";
        CpuUsage = $"{_hardware.GetCpuUsage():0.0}";

        var (memUsed, memTotal, memPercent) = _hardware.GetMemoryUsage();
        MemoryUsed = $"{memUsed:0.0}";
        MemoryTotal = $"{memTotal:0.0}";
        MemoryPercent = $"{memPercent:0}";

        var (diskUsed, diskTotal, diskPercent, diskFree) = _hardware.GetDiskUsage("C");
        DiskUsed = $"{diskUsed:0}";
        DiskTotal = $"{diskTotal:0}";
        DiskPercent = $"{diskPercent:0}";
        DiskFree = $"{diskFree:0}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
