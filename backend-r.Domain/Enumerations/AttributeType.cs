namespace backend_r.Domain.Enumerations
{
    public class AttributeType : Enumeration
    {
        public static AttributeType Region = new AttributeType(1, nameof(Region).ToLowerInvariant());
        public static AttributeType City = new AttributeType(2, nameof(City).ToLowerInvariant());
        public static AttributeType Sub_City_or_Zone = new AttributeType(3, nameof(Sub_City_or_Zone).ToLowerInvariant());
        public static AttributeType Woreda = new AttributeType(4, nameof(Woreda).ToLowerInvariant());
        public static AttributeType Kebele = new AttributeType(5, nameof(Kebele).ToLowerInvariant());
        public static AttributeType House_Number = new AttributeType(6, nameof(House_Number).ToLowerInvariant());
        public static AttributeType TelePhone = new AttributeType(7, nameof(TelePhone).ToLowerInvariant());
        public static AttributeType Mobile = new AttributeType(8, nameof(Mobile).ToLowerInvariant());
        public static AttributeType Email = new AttributeType(9, nameof(Email).ToLowerInvariant());
        public static AttributeType Telegram = new AttributeType(10, nameof(Telegram).ToLowerInvariant());
        public static AttributeType Facebook = new AttributeType(11, nameof(Facebook).ToLowerInvariant());
        public static AttributeType Twitter = new AttributeType(12, nameof(Twitter).ToLowerInvariant());
        public static AttributeType Youtube = new AttributeType(13, nameof(Youtube).ToLowerInvariant());
        public static AttributeType Whatsapp = new AttributeType(14, nameof(Whatsapp).ToLowerInvariant());
        public static IEnumerable<AttributeType> List() => new [] { Region, City, Sub_City_or_Zone, Woreda, Kebele, House_Number,
            TelePhone, Mobile, Email, Telegram, Facebook, Twitter, Youtube, Whatsapp };

        private AttributeType() {}
        private AttributeType(int id, string name) : base(id, name) {}
    }
}