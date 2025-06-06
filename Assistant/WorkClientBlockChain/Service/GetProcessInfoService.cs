using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using LibEntitiesClient.Entities;
using WorkClientBlockChain.Interface;

namespace WorkClientBlockChain.Service;

public class GetProcessInfoService : IGetProcessInfo
{
    public Exception GetLastError { get; set; } = new();
    
    public List<ProcessInfo> GetProcessInfo(string nameProcess)
    {
        try
        {
            var processes = Process.GetProcessesByName(nameProcess);

            if (processes.Length == 0) return [];

            var processInfoList = new List<ProcessInfo>();
            var processInfo = new ProcessInfo();

            foreach (var process in processes)
            {
                var pid = process.Id;
                var ports = GetPortsByPid(pid);
                processInfo.Name = process.ProcessName;
                processInfo.Port = ports.FirstOrDefault();
                processInfo.Pid = pid;
                processInfo.ProcessInit = process.Responding;
                processInfo.LastError = GetLastError.Message;

                processInfoList.Add(processInfo);
            }

            return processInfoList;
        }
        catch (Exception e)
        {
            throw new Exception($"It was not possible to obey the process identifier name. Error: {e.Message}");
        }
    }

    public ProcessInfo GetProcessInfo(int port)
    {
        var processInfo = new ProcessInfo();
        var processId = GetProcessIdPort(port);
        if (processId <= -1) return processInfo;

        try
        {
            var process = Process.GetProcessById(processId);
            processInfo.Name = process.ProcessName;
            processInfo.Port = port;
            processInfo.Pid = processId;
            processInfo.ProcessInit = true;
            processInfo.LastError = GetLastError!.Message;
            return processInfo;
        }
        catch (Exception e)
        {
            throw new Exception($"It was not possible to obey the process identifier port. Error: {e.Message}");
        }
    }

    private int GetProcessIdPort(int port)
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? GetProcessIdPortWin(port)
            : GetProcessIdPortLinux(port);
    }

    private int GetProcessIdPortWin(int port)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "netstat.exe",
            Arguments = "-aon",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process is null)
            GetLastError = new ArgumentNullException("The netstat service was not found");

        var output = process!.StandardOutput.ReadToEnd();

        if (string.IsNullOrWhiteSpace(output))
            GetLastError = new ArgumentNullException("No data on the return of the output was found");

        process.WaitForExit();

        foreach (var line in output.Split('\n'))
        {
            if (!line.Contains("TCP") && !line.Contains("UDP")) continue;

            var parts = Regex.Split(line.Trim(), @"\s+");
            if (parts.Length < 5) continue;

            var localAddress = parts[1];
            var pidStr = parts[^1];

            if (localAddress.EndsWith($":{port}") &&
                int.TryParse(pidStr, out var pid))
            {
                return pid;
            }
        }

        return -1;
    }

    private int GetProcessIdPortLinux(int port)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "lsof",
                Arguments = $"-i :{port}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process is null)
                GetLastError = new ArgumentNullException("The lsof service was not found");

            var output = process!.StandardOutput.ReadToEnd();

            if (string.IsNullOrWhiteSpace(output))
                return GetProcNetTcp(port);

            process.WaitForExit();

            foreach (var line in output.Split('\n').Skip(1))
            {
                var match = Regex.Match(line, @"^\S+\s+(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out var pid))
                    return pid;
            }

            return -1;
        }
        catch (Win32Exception e)
        {
            GetLastError = new Win32Exception($"The lsof service was not found. Error:{e.Message}");
            return -5;
        }
    }

    private static int GetProcNetTcp(int port)
    {
        try
        {
            var hexPort = port.ToString("X").PadLeft(4, '0').ToUpper();

            var file = File.ReadAllLines("/proc/net/tcp");

            foreach (var line in file.Skip(1))
            {
                var parts = line.Split((char[]?)[' '], StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4) continue;

                var localAddress = parts[1];
                var state = parts[3];

                var localPort = localAddress.Split(':')[1];
                if (localPort == hexPort && state == "0A")
                    return -2;
            }

            return -1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Error reading the file /proc/net/tcp.");
        }
    }

    private List<int> GetPortsByPid(int pid)
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? GetPortsByPidWindows(pid)
            : GetPortsByPidLinux(pid);
    }

    private List<int> GetPortsByPidWindows(int pid)
    {
        try
        {
            var result = new List<int>();
            var psi = new ProcessStartInfo
            {
                FileName = "netstat.exe",
                Arguments = "-aon",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return result;

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            foreach (var line in output.Split('\n'))
            {
                if (!line.Contains("TCP") && !line.Contains("UDP")) continue;

                var parts = Regex.Split(line.Trim(), @"\s+");
                if (parts.Length < 5) continue;

                var localAddress = parts[1];
                var pidStr = parts[^1];

                if (int.TryParse(pidStr, out var linePid) && linePid == pid)
                {
                    var portStr = localAddress.Split(':').Last();
                    if (int.TryParse(portStr, out var port))
                        result.Add(port);
                }
            }

            return result.Distinct().ToList();
        }
        catch (Exception e)
        {
            GetLastError = new Exception("It was not possible to run the Netstat program ");
            return [-5];
        }
    }

    private static List<int> GetPortsByPidLinux(int pid)
    {
        var result = new List<int>();
        var psi = new ProcessStartInfo
        {
            FileName = "lsof",
            Arguments = @$"-nP -iTCP -a -p {pid}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) return result;

        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        foreach (var line in output.Split('\n').Skip(1))
        {
            var match = Regex.Match(line, @":(\d+)\s+\(LISTEN\)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var port))
                result.Add(port);
        }

        return result.Distinct().ToList();
    }
}
