using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeoventasPocho.Controladores
{
    public class RelayCommand : ICommand
    {
        readonly Func<object, object> _execute;
        readonly Predicate<object> _canExecute;
        readonly Predicate<object> _confirmacion;

        public RelayCommand(Func<object, object> execute)
            : this(execute, null, null)
        {
        }

        public RelayCommand(Func<object, object> execute, Predicate<object> canExecute)
            : this(execute, canExecute, null)
        {
        }

        public RelayCommand(Func<object, object> execute,
                            Predicate<object> canExecute,
                            Predicate<object> confirmacion)
        {
            if (execute == null)
                throw new ArgumentNullException("Falta indicar comando Execute");


            _execute = execute;
            _canExecute = canExecute;
            _confirmacion = confirmacion;
        }

        /// <summary>
        /// Funcion para saber si se puede o no ejecutar un determinado comando
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Bool</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Ejecuta el comando
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            object result = null;
            if (_confirmacion == null)
                result = _execute(parameter);
            else
            {
                if (_confirmacion(parameter))
                    result = _execute(parameter);
            }

            this.EvaluarResultado(result);
        }

        /// <summary>
        /// Evalua si el resultado del comando ejecutado. Normalmente esto es comprobar que hizo cierta operacion correctamente
        /// y muestra algun cartel o algo.
        /// </summary>
        /// <param name="resultado"></param>
        public virtual void EvaluarResultado(object resultado)
        {

        }
    }
}
