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
        private const int MinPhoneDigits = 10;


        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ContactControl"/>.
        /// </summary>
        public ContactControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Предварительно фильтрует ввод в поле номера телефона.
        /// </summary>
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

            int digitCount = newText.Count(char.IsDigit);
            if (digitCount > MaxPhoneDigits)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Блокирует вставку недопустимых символов в поле номера телефона.
        /// </summary>
        private void PhoneTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox textBox)
            {
                e.CancelCommand();
                return;
            }

            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string pastedText = (string)e.DataObject.GetData(typeof(string))!;
            string newText = GetTextAfterInput(textBox, pastedText);

            if (!Regex.IsMatch(pastedText, @"^[0-9+\-()\s]+$"))
            {
                e.CancelCommand();
                return;
            }

            int digitCount = newText.Count(char.IsDigit);
            if (digitCount > MaxPhoneDigits)
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Предварительно фильтрует ввод в поле электронной почты.
        /// </summary>
        private void EmailTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[A-Za-z0-9@._\-]+$"))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Блокирует вставку недопустимых символов в поле электронной почты.
        /// </summary>
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

        /// <summary>
        /// Возвращает текст, который получится после ввода или вставки.
        /// </summary>
        private static string GetTextAfterInput(TextBox textBox, string input)
        {
            string currentText = textBox.Text ?? string.Empty;

            if (textBox.SelectionLength > 0)
            {
                currentText = currentText.Remove(textBox.SelectionStart, textBox.SelectionLength);
            }

            return currentText.Insert(textBox.CaretIndex, input);
        }
    }
}