using RealSenseWrapper.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gestures
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



        private String _message = "GestureRecognizer disabled";
        public String Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }




        private ObservableCollection<string> _listOfGestures = new ObservableCollection<string>() { "v_sign", "thumb_up", "thumb_down", "fist", "tap", "wave", "full_pinch"};
        public ObservableCollection<string> ListOfGestures
        {
            get
            {
                return _listOfGestures;
            }

            set
            {
                _listOfGestures = value;
                RaisePropertyChanged("ListOfGestures");

            }
        }


        public MainViewModel()
        {


        }

        public void Intialize()
        {
            this.Sensor = new RealSenseSensor();

            Sensor.InitializeColorStrem(RealSenseColorFormat.Color640x480F30);
            Sensor.InitializeGestureRecognition();

            // Sensor.InitializeGestureRecognition(ListOfGestures.ToList());

            this.Manager = new RealSenseManager(new RealSenseWrapper.Core.Model.RealSenseDataService());
            Manager.Initialize(Sensor);
            Manager.EnableColorStream();

            Manager.GeneralGestureRecognizedRaiseEvent += Manager_GeneralGestureRecognizedRaiseEvent;
        }

        private void Manager_GeneralGestureRecognizedRaiseEvent(object sender, GestureEventArgs e)
        {
            Message = e.gestureName + " - " + e.bodySideType;
        }
    }
}
