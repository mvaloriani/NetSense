using RealSenseWrapper.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GreenScreen
{
    public class MainViewModel : INotifyPropertyChanged
    {

        #region INotify
        public void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            var property = (MemberExpression)expression.Body;
            this.RaisePropertyChanged(property.Member.Name);
        }

        public void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        private RealSenseManager _manager;
        public RealSenseManager Manager
        {
            get
            {
                return _manager;
            }

            set
            {
                _manager = value;
                RaisePropertyChanged("Manager");
            }
        }


        private RealSenseSensor _sensor;
        public RealSenseSensor Sensor
        {
            get
            {
                return _sensor;
            }

            set
            {
                _sensor = value;
                RaisePropertyChanged("Sensor");
            }
        }


        public MainViewModel() {

        }

        public void Intialize()
        {
            this.Sensor = new RealSenseSensor();

            Sensor.InitializeColorStrem(RealSenseColorFormat.Color640x480F30);
            Sensor.InitializeDepthStrem(RealSenseDepthFormat.Depth640x480F30);
            Sensor.Inizialize3DSeg();

            this.Manager = new RealSenseManager(new RealSenseWrapper.Core.Model.RealSenseDataService());
            Manager.Initialize(Sensor);

            Manager.EnableColorStream();
            Manager.EnableSegmentedStream();


        }

    }
}
