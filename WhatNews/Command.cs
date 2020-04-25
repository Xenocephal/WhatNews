using System;
using System.Windows.Input;

namespace WhatNews {
    public class Command : ICommand
    {
        public Command(Action action, bool canExecute = true)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public Command(Action<object> parametrizedAction, bool canExecute = true)
        {
            this.paramAction = parametrizedAction;
            this.canExecute = canExecute;
        }

        protected Action action = null;
        protected Action<object> paramAction = null;

        private bool canExecute = false;
        public bool CanExecute
        {
            get => canExecute;
            set {
                if (canExecute != value) {
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        bool ICommand.CanExecute(object parameter)
        {
            return canExecute;
        }

        void ICommand.Execute(object parameter)
        {
            this.DoExecute(parameter);
        }
        public virtual void DoExecute(object param)
        {
            //  Вызывает выполнении команды с возможностью отмены
            CancelCommandEventArgs args = new CancelCommandEventArgs() { Parameter = param, Cancel = false };
            InvokeExecuting(args);
            
            if (args.Cancel) return; //  Если событие было отменено -  останавливаем.

            InvokeAction(param); // Вызываем действие с / без параметров, в зависимости от того. Какое было устанвленно.

            //  Call the executed function.
            InvokeExecuted(new CommandEventArgs() { Parameter = param });
        }

        public event EventHandler CanExecuteChanged;
        public event CancelCommandEventHandler Executing;
        public event CommandEventHandler Executed;

        protected void InvokeAction(object param)
        {
            Action theAction = action;
            Action<object> theParameterizedAction = paramAction;

            if (theAction != null) theAction();
            else theParameterizedAction?.Invoke(param);
        }
        protected void InvokeExecuted(CommandEventArgs args)
        {
            Executed?.Invoke(this, args); //  Вызвать все события
        }

        protected void InvokeExecuting(CancelCommandEventArgs args)
        {
            Executing?.Invoke(this, args); //  Call the executed event.
        }       

        public delegate void CommandEventHandler(object sender, CommandEventArgs args);
        
        public delegate void CancelCommandEventHandler(object sender, CancelCommandEventArgs args);              
             
        public class CommandEventArgs : EventArgs /// CommandEventArgs - simply holds the command parameter.  
        {
            public object Parameter { get; set; }
        }               
       
        public class CancelCommandEventArgs : CommandEventArgs /// CancelCommandEventArgs - just like above but allows the event to be cancelled.   
        {
            /// Gets or sets a value indicating whether this <see cref="CancelCommandEventArgs"/> command should be cancelled.           
            /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
            public bool Cancel { get; set; }
        }
    }
}
