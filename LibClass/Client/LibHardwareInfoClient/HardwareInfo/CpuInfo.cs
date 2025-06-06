using System.Runtime.InteropServices;
using LibHardwareInfoClient.Entities.Abstract;

namespace LibHardwareInfoClient.HardwareInfo;

public class CpuInfo : HardwareInfoBase
{
  public CpuInfo()
  {
    Status = "Active";
    GetCpuInfo();
  }

  private void GetCpuInfo()
  {
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
      GetNameCpu();
      GetCpuUsageWin();
      GetCoreCpu();
      return;
    }

    GetNameCpu();
    GetCpuUsageLinux();
    GetCoreCpu();
  }

  private void GetNameCpu()
  {
    var hardwareInfo = new Hardware.Info.HardwareInfo();

    hardwareInfo.RefreshCPUList();
    var name = hardwareInfo.CpuList.FirstOrDefault();
    Name = name?.Name ?? "Unknown";
  }

  private void GetCoreCpu()
  {
    var hardwareInfo = new Hardware.Info.HardwareInfo();

    hardwareInfo.RefreshCPUList();
    var cpu = hardwareInfo.CpuList.FirstOrDefault();
    TotalCapacity = cpu?.NumberOfCores ?? 0;
  }

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern bool GetSystemTimes(out long lpIdleTime, out long lpKernelTime, out long lpUserTime);

  private long _prevIdleTime, _prevKernelTime, _prevUserTime;

  private void GetCpuUsageWin()
  {
    if (!GetSystemTimes(out long idleTime, out long kernelTime, out long userTime))
      UsagePercentage = 0;

    var idleDiff = idleTime - _prevIdleTime;
    var totalDiff = (kernelTime - _prevKernelTime) + (userTime - _prevUserTime);

    _prevIdleTime = idleTime;
    _prevKernelTime = kernelTime;
    _prevUserTime = userTime;

    UsagePercentage = totalDiff == 0 ? 0 : 100.0 * (1.0 - (double)idleDiff / totalDiff);
  }

  private void GetCpuUsageLinux()
  {
    var cpuStats1 = ReadCpuStatsLinux();
    var idle1 = cpuStats1[3];
    var total1 = cpuStats1.Sum();

    Thread.Sleep(500);

    var cpuStats2 = ReadCpuStatsLinux();
    var idle2 = cpuStats2[3];
    var total2 = cpuStats2.Sum();

    var idleDelta = idle2 - idle1;
    var totalDelta = total2 - total1;

    UsagePercentage = totalDelta == 0 ? 0 : 100.0 * (1.0 - (double)idleDelta / totalDelta);
  }

  private static long[] ReadCpuStatsLinux()
  {
    return File.ReadAllText("/proc/stat")
      .Split('\n')[0]
      .Split(' ', StringSplitOptions.RemoveEmptyEntries)
      .Skip(1)
      .Select(long.Parse)
      .ToArray();
  }
}
