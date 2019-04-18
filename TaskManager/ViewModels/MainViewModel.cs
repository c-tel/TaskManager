using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.ViewModels
{
    public class MainViewModel: INotifyPropertyChanged
    {
        public MainModel Model { get; } = new MainModel();
        public ObservableCollection<ProcessAdapter> Processes => Model.Processes;
        public MainViewModel()
        {
            Model.ProcessesChanged += () => InvokePropertyChanged(nameof(Processes));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InvokePropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, e);
        }
    }
}
