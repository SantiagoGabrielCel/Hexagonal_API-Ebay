using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Api_EbayStocks.Core.Entities
{

        public class EbayStock
        {
        [NotMapped]
        public DateTime FechaConvertida
        {
            get
            {
                
                string fechaLimpia = Date.Split(' ')[0]; 

                if (DateTime.TryParseExact(fechaLimpia, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                {
                    return fecha;
                }

                return DateTime.MinValue; 
            }
        }
        public string Date { get; set; }
            public double Open { get; set; }
            public double High { get; set; }
            public double Low { get; set; }
            public double Close { get; set; }
            [Column("Adj Close")] public double AdjClose { get; set; }
            public int Volume { get; set; }
        }
    
}
