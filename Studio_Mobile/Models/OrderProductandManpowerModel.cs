namespace Soc_Management_Web.Models
{
    public class OrderProductandManpowerModel
    {
        public long Id { get; set; }
        public long SlNo { get; set; }
        public long OrderId { get; set; }
        public long datayn { get; set; }
        public long InqId { get; set; }
        public long ManpowerId { get; set; }
        public long ProductId { get; set; }
        public long JobId { get; set; }
        public long Qty { get; set; }
        public string CatNm { get; set; }
        public string ProdName { get; set; }
        public string Person { get; set; }
        public string Remarks { get; set; }
        public string Venue { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }

    }
}
