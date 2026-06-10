using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace View.Controls
{
    /// <summary>
    /// Пользовательский элемент управления для отображения и редактирования контакта.
    /// </summary>
    public partial class ContactControl : UserControl
    {
        private const int MaxPhoneDigits = 11;

        /// <summary>
        /// Свойство зависимости, определяющее режим только для чтения.
        /// </summary>
        public static readonly DependencyProperty IsEditorReadOnlyProperty =
            DependencyProperty.Register(
                nameof(IsEditorReadOnly),
                typeof(bool),
                typeof(ContactControl),
                new PropertyMetadata(true));

        /// <summary>
        /// Получает или задаёт значение, указывающее, доступны ли поля только для чтения.
        /// </summary>
        public bool IsEditorReadOnly
        {
            get => (bool)GetValue(IsEditorReadOnlyProperty);
            set => SetValue(IsEditorReadOnlyProperty, value);
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ContactControl"/>.
        /// </summary>
        public ContactControl()
        {
            InitializeComponent();
        }

        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not TextBox textBox)
            {
                return;
            }

            string newText = GetTextAfterInput(textBox, e.Text);

            if (!Regex.IsMatch(e.Text, @"^[0-9+\-()\s]+$"))
            {
                e.Handled = true;
                return;
            }

            if (newText.Count(char.IsDigit) > MaxPhoneDigits)
            {
                e.Handled = true;
            }
        }

        private void PhoneTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox textBox ||
                !e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string pastedText = (string)e.DataObject.GetData(typeof(string))!;
            string newText = GetTextAfterInput(textBox, pastedText);

            if (!Regex.IsMatch(pastedText, @"^[0-9+\-()\s]+$") ||
                newText.Count(char.IsDigit) > MaxPhoneDigits)
            {
                e.CancelCommand();
            }
        }

        private void EmailTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[A-Za-z0-9@._\-]+$"))
            {
                e.Handled = true;
            }
        }

        private void EmailTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string pastedText = (string)e.DataObject.GetData(typeof(string))!;

            if (!Regex.IsMatch(pastedText, @"^[A-Za-z0-9@._\-]+$"))
            {
                e.CancelCommand();
            }
        }

        private static string GetTextAfterInput(TextBox textBox, string input)
        {
            string currentText = textBox.Text ?? string.Empty;

            if (textBox.SelectionLength > 0)
            {
                currentText = currentText.Remove(
                    textBox.SelectionStart,
                    textBox.SelectionLength);
            }

            return currentText.Insert(textBox.CaretIndex, input);
        }
    }
}