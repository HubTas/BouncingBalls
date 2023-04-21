using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <param name="propertyName">(optional) The name of the property that changed.
        /// The <see cref="CallerMemberName"/> allows you to obtain the method or property name of the caller to the method.
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
