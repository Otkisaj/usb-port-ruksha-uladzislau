using System;
using System.Collections.Generic;
using System.Management;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Monitoring USB devices. Press Ctrl+C to exit.");

        var insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.PNPDeviceID LIKE 'USB%'");
        ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
        insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInserted);
        insertWatcher.Start();

        var removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity' AND TargetInstance.PNPDeviceID LIKE 'USB%'");
        ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
        removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemoved);
        removeWatcher.Start();

        Console.ReadLine();

        insertWatcher.Stop();
        removeWatcher.Stop();
    }

    private static void DeviceInserted(object sender, EventArrivedEventArgs e)
    {
        var device = (ManagementBaseObject)e.NewEvent["TargetInstance"];
        Console.WriteLine($"Device Inserted: {device["Name"]}, PNP Device ID: {device["PNPDeviceID"]}");
    }

    private static void DeviceRemoved(object sender, EventArrivedEventArgs e)
    {
        var device = (ManagementBaseObject)e.NewEvent["TargetInstance"];
        Console.WriteLine($"Device Removed: {device["Name"]}, PNP Device ID: {device["PNPDeviceID"]}");
    }
}
