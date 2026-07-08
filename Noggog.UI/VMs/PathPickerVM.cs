using DynamicData;
using Newtonsoft.Json;
using Noggog.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Noggog.UI;

[JsonConverter(typeof(PathPickerJsonConverter))]
public class PathPickerVM : ViewModel
{
    public enum PathTypeOptions
    {
        Off,
        Either,
        File,
        Folder
    }

    public enum CheckOptions
    {
        Off,
        IfPathNotEmpty,
        On,
    }

    [Reactive]
    public ICommand SetTargetPathCommand { get; set; }

    public ICommand SetFolderPathCommand { get; set; }

    [Reactive]
    public string TargetPath { get; set; } = string.Empty;

    [Reactive]
    public string PromptTitle { get; set; } = string.Empty;

    [Reactive]
    public PathTypeOptions PathType { get; set; }

    [Reactive]
    public CheckOptions ExistCheckOption { get; set; }
    
    [Reactive]
    public bool BlockMissingInDialog { get; set; }

    [Reactive] 
    public bool MissingIsError { get; set; } = true;

    [Reactive]
    public CheckOptions FilterCheckOption { get; set; } = CheckOptions.IfPathNotEmpty;

    [Reactive]
    public IObservable<ErrorResponse>? AdditionalError { get; set; }

    private readonly ObservableAsPropertyHelper<bool> _exists;
    public bool Exists => _exists.Value;

    private readonly ObservableAsPropertyHelper<ErrorResponse> _errorState;
    public ErrorResponse ErrorState => _errorState.Value;

    private readonly ObservableAsPropertyHelper<bool> _inError;
    public bool InError => _inError.Value;

    private readonly ObservableAsPropertyHelper<string> _errorTooltip;
    public string ErrorTooltip => _errorTooltip.Value;

    public IObservable<GetResponse<string>> PathState() => this.WhenAnyValue(
        x => x.ErrorState,
        x => x.TargetPath,
        (err, p) => err.BubbleResult(p));

    public SourceList<DialogFileFilter> Filters { get; } = new SourceList<DialogFileFilter>();

    public const string FolderDoesNotExistText = "Folder does not exist";
    public const string PathDoesNotExistText = "Path does not exist";
    public const string DoesNotPassFiltersText = "Path does not pass designated filters";

    private readonly IPathPickerDialogProvider? _dialogProvider;

    public PathPickerVM(ISchedulerProvider schedulerProvider, IPathPickerDialogProvider? dialogProvider)
    {
        _dialogProvider = dialogProvider;
        SetTargetPathCommand = ConstructTypicalPickerCommand();
        SetFolderPathCommand = ReactiveCommand.CreateFromTask(() => OpenPicker(PathTypeOptions.Folder));

        var existsCheckTuple = Observable.CombineLatest(
                this.WhenAnyValue(x => x.ExistCheckOption),
                this.WhenAnyValue(x => x.PathType),
                this.WhenAnyValue(x => x.TargetPath)
                    // Dont want to debounce the initial value, because we know it's null
                    .Skip(1)
                    .Debounce(TimeSpan.FromMilliseconds(200), schedulerProvider.TaskPool)
                    .StartWith(default(string)),
                resultSelector: (existsOption, type, path) => (ExistsOption: existsOption, Type: type, Path: path))
            .StartWith((ExistsOption: ExistCheckOption, Type: PathType, Path: TargetPath))
            .ShareLatest();

        var doExistsCheck = existsCheckTuple
            .Select(t =>
            {
                // Don't do exists type if we don't know what path type we're tracking
                if (t.Type == PathTypeOptions.Off) return false;
                switch (t.ExistsOption)
                {
                    case CheckOptions.Off:
                        return false;
                    case CheckOptions.IfPathNotEmpty:
                        return !string.IsNullOrWhiteSpace(t.Path);
                    case CheckOptions.On:
                        return true;
                    default:
                        throw new NotImplementedException();
                }
            })
            .ShareLatest();

        _exists = Observable.Interval(TimeSpan.FromSeconds(3), schedulerProvider.TaskPool)
            // Only check exists on timer if desired
            .FlowSwitch(doExistsCheck)
            .Unit()
            // Also check though, when fields change
            .Merge(this.WhenAnyValue(x => x.PathType).Unit())
            .Merge(this.WhenAnyValue(x => x.ExistCheckOption).Unit())
            .Merge(this.WhenAnyValue(x => x.TargetPath).Unit())
            // Signaled to check, get latest params for actual use
            .CombineLatest(existsCheckTuple,
                resultSelector: (_, tuple) => tuple)
            // Refresh exists
            .ObserveOn(schedulerProvider.TaskPool)
            .Select(t =>
            {
                switch (t.ExistsOption)
                {
                    case CheckOptions.IfPathNotEmpty:
                        if (string.IsNullOrWhiteSpace(t.Path)) return false;
                        break;
                    case CheckOptions.On:
                        break;
                    case CheckOptions.Off:
                    default:
                        return false;
                }
                switch (t.Type)
                {
                    case PathTypeOptions.Either:
                        return File.Exists(t.Path) || Directory.Exists(t.Path);
                    case PathTypeOptions.File:
                        return File.Exists(t.Path);
                    case PathTypeOptions.Folder:
                        return Directory.Exists(t.Path);
                    case PathTypeOptions.Off:
                    default:
                        return false;
                }
            })
            .DistinctUntilChanged()
            .ObserveOn(schedulerProvider.MainThread)
            .StartWith(false)
            .ToProperty(this, nameof(Exists));

        var passesFilters = Observable.CombineLatest(
                this.WhenAnyValue(x => x.TargetPath),
                this.WhenAnyValue(x => x.PathType),
                this.WhenAnyValue(x => x.FilterCheckOption),
                Filters.Connect().QueryWhenChanged(),
                resultSelector: (target, type, checkOption, query) =>
                {
                    switch (type)
                    {
                        case PathTypeOptions.Either:
                        case PathTypeOptions.File:
                            break;
                        default:
                            return true;
                    }
                    if (query.Count == 0) return true;
                    switch (checkOption)
                    {
                        case CheckOptions.Off:
                            return true;
                        case CheckOptions.IfPathNotEmpty:
                            if (string.IsNullOrWhiteSpace(target)) return true;
                            break;
                        case CheckOptions.On:
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    try
                    {
                        var extension = Path.GetExtension(target);
                        if (extension == null || !extension.StartsWith(".")) return false;
                        extension = extension.Substring(1);
                        if (!query.Any(filter => filter.Extensions.Any(ext => string.Equals(ext, extension)))) return false;
                    }
                    catch (ArgumentException)
                    {
                        return false;
                    }
                    return true;
                })
            .StartWith(true)
            .Select(passed =>
            {
                if (passed) return ErrorResponse.Success;
                return ErrorResponse.Fail(DoesNotPassFiltersText);
            })
            .ShareLatest();

        var errorText = Observable.CombineLatest(
                this.WhenAnyValue(x => x.Exists),
                doExistsCheck,
                this.WhenAnyValue(x => x.MissingIsError),
                resultSelector: (exists, doExists, existsIsError) => !existsIsError || !doExists || exists)
            .WithLatestFrom(
                this.WhenAnyValue(x => x.PathType),
                (exists, type) => (exists, type))
            .Select(i =>
            {
                string errStr;
                if (i.exists)
                {
                    errStr = string.Empty;
                }
                else
                {
                    errStr = i.type switch
                    {
                        PathTypeOptions.Folder => FolderDoesNotExistText,
                        _ => PathDoesNotExistText
                    };
                }
                return ErrorResponse.Create(successful: i.exists, reason: errStr);
            })
            .ShareLatest();

        _errorState = Observable.CombineLatest(
                errorText,
                passesFilters,
                this.WhenAnyValue(x => x.AdditionalError)
                    .Select(x => x ?? Observable.Return<ErrorResponse>(ErrorResponse.Success))
                    .Switch(),
                resultSelector: (existCheck, filter, err) =>
                {
                    if (existCheck.Failed) return existCheck;
                    if (filter.Failed) return filter;
                    return err;
                })
            .ObserveOn(schedulerProvider.MainThread)
            .ToProperty(this, nameof(ErrorState));

        _inError = this.WhenAnyValue(x => x.ErrorState)
            .Select(x => !x.Succeeded)
            .ToProperty(this, nameof(InError));

        // Doesn't derive from ErrorState, as we want to bubble non-empty tooltips,
        // which is slightly different logic
        _errorTooltip = Observable.CombineLatest(
                errorText.Select(x => x.Reason),
                passesFilters
                    .Select(x => x.Reason),
                this.WhenAnyValue(x => x.AdditionalError)
                    .Select(x => x ?? Observable.Return<ErrorResponse>(ErrorResponse.Success))
                    .Switch(),
                resultSelector: (exists, filters, err) =>
                {
                    if (!string.IsNullOrWhiteSpace(exists)) return exists;
                    if (!string.IsNullOrWhiteSpace(filters)) return filters;
                    return err.Reason;
                })
            .ObserveOn(schedulerProvider.MainThread)
            .ToProperty<PathPickerVM, string>(this, nameof(ErrorTooltip));
    }

    public ICommand ConstructTypicalPickerCommand()
    {
        return ReactiveCommand.CreateFromTask(
            execute: async () =>
            {
                var result = await ShowPicker(
                    PathType == PathTypeOptions.Folder,
                    ensureExists: ExistCheckOption != CheckOptions.Off && BlockMissingInDialog);
                if (result == null) return;
                TargetPath = result;
            });
    }

    private async Task OpenPicker(PathTypeOptions type)
    {
        var result = await ShowPicker(
            type == PathTypeOptions.Folder,
            ensureExists: true);
        if (result == null) return;
        TargetPath = result;
    }

    private Task<string?> ShowPicker(bool isFolderPicker, bool ensureExists)
    {
        if (_dialogProvider == null) return Task.FromResult<string?>(null);

        string dirPath;
        if (File.Exists(TargetPath))
        {
            dirPath = Path.GetDirectoryName(TargetPath) ?? string.Empty;
        }
        else
        {
            dirPath = TargetPath;
        }

        return _dialogProvider.ShowPickerAsync(new PathPickerDialogRequest
        {
            Title = PromptTitle,
            IsFolderPicker = isFolderPicker,
            InitialDirectory = dirPath,
            EnsureFileExists = ensureExists,
            EnsurePathExists = ensureExists,
            Filters = Filters.Items.ToList(),
        });
    }

    public class PathPickerJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (!(value is PathPickerVM vm)) throw new ArgumentException();
            writer.WriteValue(vm.TargetPath);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (existingValue is not PathPickerVM vm)
            {
                return new PathPickerVM(new SchedulerProvider(), null);
            }
            if (reader.Value is not string str) throw new ArgumentException();
            vm.TargetPath = str;
            return vm;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PathPickerVM);
        }
    }
}