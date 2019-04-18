using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class ProcessAdapter: INotifyPropertyChanged
    {
        private readonly Process _process;
        public string Name { get; }
        public int ID { get; }
        public string User { get; }
        public string Folder { get; }
        public DateTime Launched { get; }

        public bool Active { get; private set; }
        public double CPU { get; private set; }
        public double Memory { get; private set; }
        public int ThreadsCount { get; private set; }

        public ProcessAdapter(Process process)
        {
            _process = process;
            Name = process.ProcessName;
            ID = process.Id;
            User = process.StartInfo.UserName;
            Folder = process.MainModule.FileName;
            Launched = process.StartTime;
        }

        public void Refresh()
        {
            var cpu = new PerformanceCounter("Process", "% Processor Time", _process.ProcessName, true);
            var ram = new PerformanceCounter("Process", "Private Bytes", _process.ProcessName, true);

            var _ = cpu.NextValue();

            CPU = Math.Round(cpu.NextValue() / Environment.ProcessorCount, 2);
            Memory = Math.Round(ram.NextValue() / (1024 * 1024), 2);
            Active = _process.Responding;
            ThreadsCount = _process.Threads.Count;

            InvokePropertyChanged(nameof(CPU));
            InvokePropertyChanged(nameof(Memory));
            InvokePropertyChanged(nameof(Active));
            InvokePropertyChanged(nameof(ThreadsCount));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, e);
        }
    }
}
