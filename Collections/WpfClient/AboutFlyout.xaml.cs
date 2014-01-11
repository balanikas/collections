﻿using System.Collections.Generic;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Linq;
using CollectionsSOLID;

namespace WpfClient
{

    public partial class AboutFlyout : Flyout, INotifyPropertyChanged
    {


        public AboutFlyout()
        {

            InitializeComponent();
            
        }

     

        private bool canCloseFlyout;

        public bool CanCloseFlyout
        {
            get { return this.canCloseFlyout; }
            set
            {
                if (Equals(value, canCloseFlyout))
                {
                    return;
                }
                canCloseFlyout = value;
                RaisePropertyChanged("CanCloseFlyout");
            }
        }

        private ICommand closeCmd;

        public ICommand CloseCmd
        {
            get
            {
                return this.closeCmd ?? (closeCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => this.CanCloseFlyout,
                    ExecuteDelegate = x => this.IsOpen = false
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    
    }
}