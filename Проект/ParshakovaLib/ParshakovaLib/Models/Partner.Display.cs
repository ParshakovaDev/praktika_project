using System.Linq;

namespace ParshakovaLib.Models
{
    /// <summary>
    /// Вычисляемые свойства для отображения в UI (в т.ч. скидка при пустой коллекции).
    /// </summary>
    public partial class Partner
    {
        /// <summary>
        /// Процент скидки для отображения; 0, если записей о скидке нет.
        /// </summary>
        public double DisplayDiscountPercent => Discount?.FirstOrDefault()?.Percentage ?? 0;
    }
}
