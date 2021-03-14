namespace OLEDShow
{
    public class InfoText
    {
        string _time = "Unknwon";
        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                RefreshText();
            }
        }

        string _network = "Unknwon";
        public string Network
        {
            get => _network;
            set
            {
                _network = value;
                RefreshText();
            }
        }

        public override string ToString()
        {
            return Time + "\n" + Network;
        }

        public void RefreshText()
        {
            Shared.MainActivity.SetTxvText(ToString());
        }
    }
}