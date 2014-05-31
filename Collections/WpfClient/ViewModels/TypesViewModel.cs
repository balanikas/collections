using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro.Controls.Dialogs;
using Collections.Utilities;

namespace WpfClient.ViewModels
{
    public class TypesViewModel : ViewModelBase
    {
        private readonly TypesProvider _typesProvider;
        private TextDocument _codeDocument;
        private string _filesPath;
        private MethodInfo _selectedMethod;
        private LoadedType _selectedType;
        private ObservableCollection<LoadedType> _types;
        private object _isLoadButtonEnabled;
        private bool _isCodeDocumentEnabled;


        public TypesViewModel(TypesProvider typesProvider)
        {
            _typesProvider = typesProvider;
            _types = new ObservableCollection<LoadedType>();
            _codeDocument = new TextDocument();


            CmdLoadTypes = new RelayCommand(() =>
            {
                LoadTypes(false);
                
            });

            CmdTypesSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(e =>
            {
                if (e.AddedItems.Count == 0)
                {
                    CodeDocument.Text = String.Empty;
                    IsCodeDocumentEnabled = false;
                    return;
                }

                var type = ((LoadedType) e.AddedItems[0]);
                CodeDocument.Text = type.Source;
                IsCodeDocumentEnabled = type.AllowEditSource;
            });

            CmdMethodsSelectionChanged = new RelayCommand<SelectionChangedEventArgs>(e =>
            {
            });

            CmdCompile = new RelayCommand(() =>
            {
                List<string> errors;
                if (_typesProvider.TryCompileFromText(CodeDocument.Text, out errors).Any())
                {

                    _typesProvider.SaveType(new LoadedType(SelectedType.TypeInfo, SelectedType.FilePath, CodeDocument.Text));

                    ViewModelLocator.Logger.InfoNow("Compilation succeeded");

                    LoadTypes(true);
                }
                else
                {
                    foreach (var error in errors)
                    {
                        ViewModelLocator.Logger.ErrorNow(error);
                    }
                    
                }
               
            });

            _typesProvider.SetActiveCompilerService(Settings.Instance.Get(Settings.Keys.CompilerServiceType));
        }


        public RelayCommand CmdLoadTypes { get; private set; }

        public RelayCommand<SelectionChangedEventArgs> CmdTypesSelectionChanged { get; private set; }
        public RelayCommand<SelectionChangedEventArgs> CmdMethodsSelectionChanged { get; private set; }

        public RelayCommand CmdCompile { get; private set; }

        public ObservableCollection<LoadedType> Types
        {
            get
            {
                return _types;
            }
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
                _codeDocument = value;
                RaisePropertyChanged("CodeDocument");
            }
        }

        public bool IsCodeDocumentEnabled
        {
            get { return _isCodeDocumentEnabled; }
            private set
            {
                _isCodeDocumentEnabled = value;
                RaisePropertyChanged("IsCodeDocumentEnabled");
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

        public string FilesPath
        {
            get { return _filesPath; }
            set
            {
                _filesPath = value;

                RaisePropertyChanged("FilesPath");
            }
        }

        public object IsLoadButtonEnabled
        {
            get { return _isLoadButtonEnabled; }
            set {
                _isLoadButtonEnabled = value;
                RaisePropertyChanged("IsLoadButtonEnabled");
            }
        }


        private async void LoadTypes(bool rememberSelection)
        {
            if (!PathValidator.IsWellFormedPath(FilesPath))
            {
                ViewModelLocator.Logger.ErrorNow(string.Format("malformed path '{0}'",FilesPath));
                return;
            }
            if (PathValidator.DeterminePathType(FilesPath) == PathValidator.PathType.Unknown)
            {
                ViewModelLocator.Logger.ErrorNow(string.Format("'{0}' is empty or not a valid path for a code file, assembly file, or directory containing code files", FilesPath));
                return;
            }

            ProgressDialogController progressDialogController = await MainWindow.ShowProgress(
                "Please wait...",
                "loading types from " + FilesPath);

            string previousSelectedTypeName = null;
            string previousSelectedMethodName = null;
            if (rememberSelection)
            {
                previousSelectedTypeName = SelectedType != null ? SelectedType.TypeInfo.FullName : null;
                previousSelectedMethodName = SelectedMethod != null ? SelectedMethod.Name : null;
            }
           

            _typesProvider.SetActiveCompilerService(Settings.Instance.Get(Settings.Keys.CompilerServiceType));

           
            try
            {
                List<LoadedType> types = null;
                switch (PathValidator.DeterminePathType(FilesPath))
                {
                    case PathValidator.PathType.SourceFile:
                        types = await _typesProvider.FromSourceFileAsync(FilesPath);
                        break;
                    case PathValidator.PathType.SourceFolder:
                        types = await _typesProvider.FromSourceFolderAsync(FilesPath);
                        break;
                    case PathValidator.PathType.AssemblyFile:
                        types = await _typesProvider.FromAssemblyFileAsync(FilesPath);
                        break;
                }
                Types = new ObservableCollection<LoadedType>(types);
                
            }
            catch (Exception e)
            {
                ViewModelLocator.Logger.ErrorNow(e.Message);
                return;
            }

            if (previousSelectedTypeName != null)
            {
                var foundType = Types.FirstOrDefault(t => t.TypeInfo.FullName == previousSelectedTypeName);
                SelectedType = foundType;
                if (foundType != null)
                {
                    SelectedMethod = foundType.MethodsInfos.FirstOrDefault(m => m.Name == previousSelectedMethodName);
                }
            }
            
            await progressDialogController.CloseAsync();
        }
    }
}