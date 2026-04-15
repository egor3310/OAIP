using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace View.Converters
{
    /// <summary>
    /// Применяет изменения, внесённые в данные контакта.
    /// </summary>
    /// <param name="parameter">Параметр команды.</param>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует логическое значение в <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value">Исходное значение.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр преобразования.</param>
        /// <param name="culture">Культура преобразования.</param>
        /// <returns>
        /// <see cref="Visibility.Visible"/>, если значение равно <see langword="true"/>; 
        /// иначе <see cref="Visibility.Collapsed"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = value is bool b && b;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Обратное преобразование не поддерживается.
        /// </summary>
        /// <param name="value">Значение для обратного преобразования.</param>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="parameter">Дополнительный параметр преобразования.</param>
        /// <param name="culture">Культура преобразования.</param>
        /// <returns>Результат обратного преобразования.</returns>
        /// <exception cref="NotImplementedException">Метод не реализован.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}