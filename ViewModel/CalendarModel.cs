using System.Collections.ObjectModel;

namespace PolMedUMG.ViewModel
{
    /// <summary>
    /// ViewModel umożliwiający poprawne wyświetlenie kalendarzy w Designerze
    /// </summary>
    public class CalendarModel
    {
        public ObservableCollection<DateTime> Months { get; set; }
        public int year { get; set; }
        public CalendarModel()
        {
            year = DateTime.Now.Year; 
            Months = new ObservableCollection<DateTime>
            {
                new DateTime(year, 1, 1),
                new DateTime(year, 2, 1),
                new DateTime(year, 3, 1),
                new DateTime(year, 4, 1),
                new DateTime(year, 5, 1),
                new DateTime(year, 6, 1),
                new DateTime(year, 7, 1),
                new DateTime(year, 8, 1),
                new DateTime(year, 9, 1),
                new DateTime(year, 10, 1),
                new DateTime(year, 11, 1),
                new DateTime(year, 12, 1)
            };
        }
    
    }
}
