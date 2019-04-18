using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class MainModel
    {
        private readonly object _syncProcessesUpdates = new object();
        private readonly Thread _listWorker;
        private readonly Thread _propsWorker;

        private bool _stopWorkers;


        public ObservableCollection<ProcessAdapter> Processes { get; private set; } = new ObservableCollection<ProcessAdapter>();
        public event Action ProcessesChanged;

        public MainModel()
        {
            _listWorker = new Thread(RefreshListWorker);
            _propsWorker = new Thread(RefreshExistingWorker);
            _listWorker.Start();
            _propsWorker.Start();
            App.Current.Dispatcher.ShutdownStarted += OnShutDown;
        }

        private void OnShutDown(object sender, EventArgs e)
        {
            _stopWorkers = true;
            _listWorker.Abort();
            _propsWorker.Abort();
        }

        private void RefreshListWorker()
        {
            while(!_stopWorkers)
            {
                lock(_syncProcessesUpdates)
                {
                    var actualProcesses = Process.GetProcesses();
                    var toAdd = actualProcesses
                        .Where(p => !Processes.Any(pa => pa.ID == p.Id));
                    foreach (var proc in toAdd)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            
                            try { Processes.Add(new ProcessAdapter(proc)); } catch(Exception) { }
                        });
                    }
                    var toRemove = Processes.Where(pa => !actualProcesses.Any(p => p.Id == pa.ID));
                    foreach (var proc in toRemove)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            Processes.Remove(proc);
                        });
                    }
                    ProcessesChanged.Invoke();
                }
                
                Thread.Sleep(5000);
            }
        }

        private void RefreshExistingWorker()
        {
            while (!_stopWorkers)
            {
                lock (_syncProcessesUpdates)
                {
                    foreach (var pa in Processes)
                    {
                        try { pa.Refresh(); } catch(Exception) { }
                    }
                    ProcessesChanged.Invoke();
                }
                Thread.Sleep(2000);
            }
        }
    }
}
