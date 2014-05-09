using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro.Controls.Dialogs;

namespace WpfClient.ViewModels
{
    public class TypesViewModel : ViewModelBase
    {
        private readonly TypesProvider _typesProvider;
        private TextDocument _codeDocument;
        private string _filesPath;
        private List<MethodInfo> _methods;
        private MethodInfo _selectedMethod;
        private LoadedType _selectedType;
        private ObservableCollection<LoadedType> _types;


        public TypesViewModel(TypesProvider typesProvider)
        {
            _typesProvider = typesProvider;
            _types = new ObservableCollection<LoadedType>();
            _codeDocument = new TextDocument();


            CmdLoadTypes = new RelayCommand(LoadTypes);

            CmdTypesSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(e =>
            {
                if (e.AddedItems.Count == 0)
                {
                    CodeDocument.Text = String.Empty;
                    return;
                }

                CodeDocument.Text = ((LoadedType) e.AddedItems[0]).Source;

                SelectedType = (LoadedType) e.AddedItems[0];
                SelectedMethod = SelectedType.MethodsInfos[0];
            });

            CmdMethodsSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(e =>
            {
                if (e.AddedItems.Count == 0)
                {
                    return;
                }
                SelectedMethod = (MethodInfo) e.AddedItems[0];
            });

            CmdCompile = new RelayCommand(() =>
            {
                if (Methods.Count == 0)
                {
                    return;
                }

                List<string> errors;
                if (_typesProvider.TryCompileFromText(CodeDocument.Text, out errors) != null)
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
                    foreach (string error in errors)
                    {
                        ViewModelLocator.Logger.ErrorNow(error);
                    }
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
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                Methods = _selectedType.MethodsInfos;

                RaisePropertyChanged("SelectedType");
            }
        }

        public MethodInfo SelectedMethod
        {
            get { return _selectedMethod; }
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

                RaisePropertyChanged("FilesPath");
            }
        }

        public void HighlightMethod(LoadedType type, MethodInfo method)
        {
            //SelectedType = type;
            //SelectedMethod = method;
        }

        private async void LoadTypes()
        {
            ProgressDialogController controller = await MainWindow.ShowProgress(
                "Please wait...",
                "loading types from " + FilesPath);


            _typesProvider.SetActiveCompilerService(Settings.CompilerServiceType);
            var allTypes = new List<LoadedType>();


            if (Utils.IsValidAssembly(FilesPath))
            {
                List<LoadedType> types = await _typesProvider.FromAssemblyAsync(FilesPath);
                allTypes.AddRange(types);
            }
            else if (File.Exists(FilesPath) || Directory.Exists(FilesPath))
            {
                List<LoadedType> types = await _typesProvider.FromDiscAsync(FilesPath);
                allTypes.AddRange(types);
            }

            Types = new ObservableCollection<LoadedType>(allTypes);

            await controller.CloseAsync();
        }
    }
}