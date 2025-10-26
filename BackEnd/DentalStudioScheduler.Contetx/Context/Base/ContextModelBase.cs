namespace DentalStudioScheduler.Context.Base
{
    public abstract class ContextModelBase
    {
        public string UserRef { get; set; }
        public string Ref { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateChange { get; set; }

        public ContextModelBase()
        {
            Ref = Environment.MachineName;
            UserRef = Environment.UserName;
            DateCreate = DateTime.Now;
            DateChange = DateTime.Now;
        }

        public void UpdateBase()
        {
            DateChange = DateTime.Now;
            Ref = Environment.MachineName;
            UserRef = Environment.UserName;
        }

        public void UpdateBase(string machineRef = null, string userRef = null)
        {
            DateChange = DateTime.Now;
            Ref = ((!string.IsNullOrWhiteSpace(machineRef)) ? machineRef : Environment.MachineName);
            UserRef = ((!string.IsNullOrWhiteSpace(userRef)) ? userRef : Environment.UserName);
        }
    }
}
