using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;

namespace CoreSense.Services;

public class HardwareService
{
    private readonly PerformanceCounter _cpuCounter;

    public HardwareService()
    {
        _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        _cpuCounter.NextValue();
    }

    public float? GetCpuTemperature()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
            foreach (var obj in searcher.Get())
            {
                var name = obj["InstanceName"]?.ToString();
                var raw = obj["CurrentTemperature"];
                if (name != null && name.Contains("CPUZ") && raw != null)
                {
                    double tempCelsius = (Convert.ToDouble(raw) - 2732) / 10.0;
                    return (float)tempCelsius;
                }
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    public float GetCpuUsage()
    {
        try
        {
            return _cpuCounter.NextValue();
        }
        catch
        {
            return 0f;
        }
    }

    public (float usedGb, float totalGb, float percentUsed) GetMemoryUsage()
    {
        try
        {
            var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
            foreach (var obj in searcher.Get())
            {
                ulong totalKb = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                ulong freeKb = Convert.ToUInt64(obj["FreePhysicalMemory"]);
                ulong usedKb = totalKb - freeKb;

                float totalGb = totalKb / 1024f / 1024f;
                float usedGb = usedKb / 1024f / 1024f;
                float percent = (usedGb / totalGb) * 100f;

                return (usedGb, totalGb, percent);
            }
        }
        catch { }

        return (0, 0, 0);
    }

    public (float usedGb, float totalGb, float percentUsed, float freeGb) GetDiskUsage(string driveLetter = "C")
    {
        try
        {
            var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.Name.StartsWith(driveLetter, StringComparison.OrdinalIgnoreCase));
            if (drive != null && drive.IsReady)
            {
                float total = drive.TotalSize / 1_073_741_824f;
                float free = drive.TotalFreeSpace / 1_073_741_824f;
                float used = total - free;
                float percent = (used / total) * 100f;
                return (used, total, percent, free);
            }
        }
        catch { }

        return (0, 0, 0, 0);
    }
}
