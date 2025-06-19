namespace PolMedUMG.Model
{
    public class Visit
    {
        public string DoctorName { get; set; }
        public string PESEL { get; set; }
        public string PatientName { get; set; }
        public string RoomNumber { get; set; }
        public string CauseOfVisit { get; set; }
        public string AdditionalInfo { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfVisit { get; set; }
        public string ServiceName { get; set; }
    }
}
