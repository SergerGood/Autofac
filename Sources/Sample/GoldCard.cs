namespace Sample
{
    internal class GoldCard : CreditCard
    {
        private string accountId;

        public GoldCard(string accountId)
        {
            this.accountId = accountId;
        }
    }
}