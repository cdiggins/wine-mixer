namespace WineMixer
{
    public class Session
    {
        public List<State> States { get; set; }
        public List<Transfer> Transfers { get; set; }
        public Configuration Configuration { get; set; }
    }
}