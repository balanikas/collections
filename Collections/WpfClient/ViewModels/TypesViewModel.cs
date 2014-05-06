using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;

namespace WpfClient.ViewModels
{
    public class TypesViewModel : ViewModelBase
    {
        private string _filesPath;
        private string _assemblyPath;
        private readonly TypesProvider _typesProvider;
        private ObservableCollection<LoadedType> _types;
        private LoadedType _selectedType;
        private MethodInfo _selectedMethod;
        private List<MethodInfo> _methods;
        private TextDocument _codeDocument;
        private bool _uiEnabled;

        public TypesViewModel(ILogger logger)
        {
            _typesProvider = new TypesProvider(new DefaultCompilerService());
            _types = new ObservableCollection<LoadedType>();
            _codeDocument = new TextDocument();

            _uiEnabled = true;
        

            CmdLoadTypes = new RelayCommand(() =>
            {
                UIEnabled = false;
                LoadTypes();
                UIEnabled = true;
            });

            CmdTypesSelectionChanged = new RelayCommand<SelectionChangedEventArgs>((e) =>
            {
                if (e.AddedItems.Count == 0)
                {
                    CodeDocument.Text = String.Empty;
                    return;
                }

                CodeDocument.Text = ((LoadedType) e.AddedItems[0]).Source;

                SelectedType = (LoadedType) e.AddedItems[0];

            });
           
            CmdMethodsSelectionChanged = new RelayCommand<SelectionChangedEventArgs>((e) =>
            {
                if (e.AddedItems.Count == 0)
                {
                   
                    return;
                }
                SelectedMethod = (MethodInfo)e.AddedItems[0];
            });

            CmdCompile = new RelayCommand(() =>
            {
                if (Methods.Count == 0)
                {
                    return;
                }

                List<string> errors;
                if (_typesProvider.TryCompileFromSource(CodeDocument.Text, out errors))
                {
                    _typesProvider.SaveType(new LoadedType
                    {
                        FilePath = SelectedType.FilePath,
                        Source = CodeDocument.Text,
                        TypeInfo = SelectedType.TypeInfo
                    });

                    LoadTypes();
                }
                else
                {
                    MessageBox.Show("Compilation error");
                }
            });
        }

  

        public RelayCommand CmdLoadTypes { get; private set; }

        public RelayCommand<SelectionChangedEventArgs> CmdTypesSelectionChanged { get; private set; }
        public RelayCommand<SelectionChangedEventArgs> CmdMethodsSelectionChanged { get; private set; }

        public RelayCommand CmdCompile { get; private set; }

        public ObservableCollection<LoadedType> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                RaisePropertyChanged("Types");
            }
        }

        public bool UIEnabled
        {
            get
            {
                return _uiEnabled;
            }
            set
            {
                _uiEnabled = value;
                RaisePropertyChanged("UIEnabled");
            }
        }

        public TextDocument CodeDocument
        {
            get { return _codeDocument; }
            set
            {
                if (_codeDocument == null)
                {
                    return;
                }
                _codeDocument = value;
                RaisePropertyChanged("CodeDocument");
            }
        }

        public LoadedType SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                _selectedType = value;
                Methods = _selectedType.MethodsInfos;
                RaisePropertyChanged("SelectedType");
            }
        }

        public MethodInfo SelectedMethod
        {
            get
            {
                return _selectedMethod;
            }
            set
            {
                _selectedMethod = value;
               
                RaisePropertyChanged("SelectedMethod");
            }
        }

        public List<MethodInfo> Methods
        {
            get { return _methods; }
            set
            {
                _methods = value;
                RaisePropertyChanged("Methods");
            }
        }

        public string FilesPath
        {
            get { return _filesPath; }
            set
            {
                _filesPath = value;

                this.RaisePropertyChanged("FilesPath");
            }
        }


        public string AssemblyPath
        {
            get { return _assemblyPath; }
            set
            {
                _assemblyPath = value;

                this.RaisePropertyChanged("AssemblyPath");
            }
        }

        private async void LoadTypes()
        {
            
            ViewModelLocator.MainWindow.ProgressBarVisibility = Visibility.Visible;

            var allTypes = new List<LoadedType>();


            if (Utils.IsValidAssembly(AssemblyPath))
            {
                List<LoadedType> types = await _typesProvider.FromAssemblyAsync(AssemblyPath);
                allTypes.AddRange(types);
            }

            if (File.Exists(FilesPath) || Directory.Exists(FilesPath))
            {
                List<LoadedType> types = await _typesProvider.FromDiscAsync(FilesPath);
                allTypes.AddRange(types);
            }

            Types = new ObservableCollection<LoadedType>(allTypes);

            ViewModelLocator.MainWindow.ProgressBarVisibility = Visibility.Hidden;
        }

   
    }
}
