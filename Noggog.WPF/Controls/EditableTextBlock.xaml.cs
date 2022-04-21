using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Noggog.WPF;

/*
 * Adapted from: http://www.codeproject.com/Articles/72544/Editable-Text-Block-in-WPF
 */
public partial class EditableTextBlock : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged([CallerMemberName] string caller = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
    }

    #region Constructor

    public EditableTextBlock()
    {
        InitializeComponent();
        Focusable = true;
        FocusVisualStyle = null;
    }

    #endregion Constructor

    #region Properties
    private bool _IsDefaulted = true;
    public bool IsDefaulted
    {
        get { return _IsDefaulted; }
        set { if (!Object.Equals(value, _IsDefaulted)) { _IsDefaulted = value; RaisePropertyChanged(); } }
    }

    public string DefaultTextContent
    {
        get { return (string)GetValue(DefaultTextContentProperty); }
        set { SetValue(DefaultTextContentProperty, value); }
    }
    public static readonly DependencyProperty DefaultTextContentProperty =
        DependencyProperty.Register(
            "DefaultTextContent",
            typeof(string),
            typeof(EditableTextBlock),
            new PropertyMetadata(""));


    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(EditableTextBlock),
            new FrameworkPropertyMetadata("",
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (o, e) =>
                {
                    if (!(o is EditableTextBlock tb)) return;
                    tb.IsDefaulted = tb.ShouldDefault(tb.Text, tb.DefaultTextContent);
                }));

    public bool IsEditable
    {
        get { return (bool)GetValue(IsEditableProperty); }
        set { SetValue(IsEditableProperty, value); }
    }
    public static readonly DependencyProperty IsEditableProperty =
        DependencyProperty.Register(
            "IsEditable",
            typeof(bool),
            typeof(EditableTextBlock),
            new PropertyMetadata(true));

    public bool IsInEditMode
    {
        get
        {
            if (IsEditable)
                return (bool)GetValue(IsInEditModeProperty);
            else
                return false;
        }
        set
        {
            if (IsEditable)
            {
                SetValue(IsInEditModeProperty, value);
            }
        }
    }

    public static readonly DependencyProperty IsInEditModeProperty =
        DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(EditableTextBlock),
            new PropertyMetadata(false));




    public bool DoubleClickToEdit
    {
        get { return (bool)GetValue(DoubleClickToEditProperty); }
        set { SetValue(DoubleClickToEditProperty, value); }
    }
    public static readonly DependencyProperty DoubleClickToEditProperty =
        DependencyProperty.Register("DoubleClickToEdit", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(true));
    #endregion Properties

    #region Event Handlers

    // Invoked when we exit edit mode.
    void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        IsInEditMode = false;
    }

    // Invoked when the user edits the annotation.
    void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            IsInEditMode = false;
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            AbortEdit();
            e.Handled = true;
        }
    }

    private void EditableControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!(sender is TextBox txt)) return;
        if (txt.Visibility == Visibility.Visible)
        {
            txt.Focus();
            txt.SelectAll();
        }
    }

    public void AbortEdit()
    {
        EditableControl.Text = UneditableControl.Text;
        IsInEditMode = false;
    }

    #endregion Event Handlers

    public bool ShouldDefault(string text, string defaultText)
    {
        return string.IsNullOrWhiteSpace(text) || Object.Equals(defaultText, text);
    }

    private void mainControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DoubleClickToEdit)
        {
            IsInEditMode = true;
            e.Handled = true;
        }
    }
}