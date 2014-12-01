using DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DataAccess
{
    public class Binding<T>
    {
        private IDataStore _store;
        private ObservableCollection<T> _collection;
        private bool _observable;

        public Binding(IDataStore dstore, ObservableCollection<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (dstore == null) throw new ArgumentNullException("dstore");

            _store = dstore;
            _collection = collection;
            _observable = typeof(T).GetInterfaces().Contains(typeof(INotifyPropertyChanged));
            Init();
        }

        private void Init()
        {
            _collection.Clear();
            foreach (var v in _store.LoadEntireTable<T>())
            {
                _collection.Add(v);
                BindItem(v);
            }

            _collection.CollectionChanged += OnChange;
        }

        private void BindItem(object v)
        {
            if (_observable)
            {
                INotifyPropertyChanged item = (INotifyPropertyChanged)v;
                item.PropertyChanged += item_PropertyChanged;
            }
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _store.UpdateObject(sender);
        }

        private void OnChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    _store.InsertObjects(e.NewItems);
                    foreach (object v in e.NewItems)
                        BindItem(v);

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var v in e.OldItems)
                        _store.DeleteObject(v);
                    break;
            }
        }
    }
}
