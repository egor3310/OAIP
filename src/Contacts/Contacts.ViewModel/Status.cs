using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts.ViewModel
{
    internal class Status
    {
        /// <summary>
        /// Определяет режим работы редактора контактов.
        /// </summary>
        public enum EditorMode
        {
            /// <summary>
            /// Режим просмотра.
            /// </summary>
            None,

            /// <summary>
            /// Режим добавления.
            /// </summary>
            Add,

            /// <summary>
            /// Режим редактирования.
            /// </summary>
            Edit
        }
    }
}
