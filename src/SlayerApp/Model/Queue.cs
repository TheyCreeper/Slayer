using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SlayerApp.Model
{
    public class Queue
    {
        public ObservableCollection<Song> queue { get; set; }
        public Queue() { }
    }
}
