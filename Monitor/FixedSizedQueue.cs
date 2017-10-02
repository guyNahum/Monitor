using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Monitor
{
    //todo: add locks if needed
     public class FixedSizedQueue<T> : INotifyPropertyChanged
    {
        #region Fields

        private Queue<T> _queue;
        private readonly int _fixedSize;

        #endregion

        #region Properties

        public IList<T> Items
        {
            get
            {
                List<T> tempList = _queue.ToList();
                tempList.Reverse();

                return tempList;
            }
        }

        #endregion

        #region CTOR

        public FixedSizedQueue(int fixedSize)
        {
            _queue = new Queue<T>();
            _fixedSize = fixedSize;
        }

        #endregion

        #region API

        public void Add(T item)
        {
            _queue.Enqueue(item);

            if (_queue.Count > _fixedSize)
            {
                _queue.Dequeue();
            }

            NotifyPropertyChanged("Items");
        }

        public void Clear()
        {
            _queue.Clear();

            NotifyPropertyChanged("Items");
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
